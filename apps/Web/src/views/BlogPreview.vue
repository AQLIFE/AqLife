<template>
  <div class="blog-preview">
    <ElPageHeader class="header" :icon="ArrowLeft" @back="handleBack">
      <template #content>
        <div class="article-header">
          <div class="article-title-row">
            <span v-if="articleStore.isLoading" class="article-title-skeleton" />
            <span v-else class="article-title">
              {{ fileMeta?.fileName }}
            </span>

            <div v-if="articleTags.length" class="article-tags">
              <ElTag v-for="tag in articleTags" :key="tag.uid!">
                {{ tag.name }}
              </ElTag>
            </div>
          </div>

          <div v-if="fileMeta?.uploadTime" class="article-upload-time">
            发布于 {{ fileMeta.uploadTime }}
          </div>
        </div>
      </template>

      <template #extra>
        <ElButton :icon="Share" link title="分享文章" @click="shareArticle" />
        <ElButton v-if="!isFullscreen" :icon="FullScreen" link title="全屏阅读" @click="enterFullscreen" />
        <ElButton v-else :icon="Close" link title="退出全屏" @click="exitFullscreen" />
      </template>
    </ElPageHeader>

    <ElScrollbar class="markdown-scrollbar" @scroll="handleMarkdownScroll" @end-reached="handleEndReached">
      <div v-if="articleStore.isLoading" class="preview-skeleton" aria-label="正在加载文章">
        <div class="skeleton-meta"><span /><span /></div>
        <div class="skeleton-line skeleton-line-title" />
        <div class="skeleton-line skeleton-line-wide" />
        <div class="skeleton-line skeleton-line-medium" />
        <div class="skeleton-block" />
        <div class="skeleton-line skeleton-line-wide" />
        <div class="skeleton-line skeleton-line-medium" />
        <div class="skeleton-line skeleton-line-short" />
        <div class="skeleton-block skeleton-block-large" />
        <div class="skeleton-line skeleton-line-wide" />
        <div class="skeleton-line skeleton-line-medium" />
      </div>
      <div v-else ref="markdownContainer" class="markdown-content" v-loading="markdownRenderRef?.loading">
        <MarkdownRender ref="markdownRenderRef" :markdown="sourceMarkdown" :baseurl="baseurl" :file-resolver="markdownFileResolver" />
      </div>
    </ElScrollbar>
  </div>
</template>

<script setup lang="ts">
import {
  ElButton,
  ElPageHeader,
  ElTag,
  ElMessage,
} from 'element-plus'
import {
  ArrowLeft,
  Share,
  FullScreen,
  Close,
} from '@element-plus/icons-vue'
import {
  onBeforeUnmount,
  ref,
  computed,
  watch,
  onMounted,
  nextTick,
} from 'vue'
import {
  useRoute,
  useRouter,
} from 'vue-router'

import {
  FileApi,
  FileScope,
  type ApiFileDownloadGetRequest,
  type FileDto,
} from '@/api'
import { apiConfiguration } from '@/services/api'
import { useArticleStore } from '@/stores/articleStore'
import { useBlogStore } from '@/stores/fileStore'
import { MarkdownRender } from '@aqlife/ui-shared'
import { extractTocTree } from '@aqlife/domain'

const route = useRoute()
const router = useRouter()
const articleStore = useArticleStore()
const blogStore = useBlogStore()
const fileApi = new FileApi(apiConfiguration)
const baseurl = import.meta.env.VITE_API
const isFullscreen = computed(() => articleStore.isFullscreen)

function enterFullscreen() {
  articleStore.isFullscreen = true
}

function exitFullscreen() {
  articleStore.isFullscreen = false
}

function handleKeydown(event: KeyboardEvent) {
  if (event.key === 'Escape' && isFullscreen.value) {
    exitFullscreen()
  }
}

async function shareArticle() {
  const url = window.location.href

  try {
    if (navigator.clipboard?.writeText) {
      await navigator.clipboard.writeText(url)
    } else {
      const textarea = document.createElement('textarea')
      textarea.value = url
      textarea.setAttribute('readonly', '')
      textarea.style.position = 'fixed'
      textarea.style.opacity = '0'
      document.body.appendChild(textarea)
      textarea.select()
      const copied = document.execCommand('copy')
      document.body.removeChild(textarea)
      if (!copied) throw new Error('浏览器不支持复制')
    }

    ElMessage.success('文章链接已复制')
  } catch (error) {
    console.error('复制文章链接失败:', error)
    ElMessage.error('复制失败，请手动复制当前页面地址')
  }
}

const markdownFileResolver = {
  async getFile(uid: string): Promise<FileDto | undefined> {
    const image = await fileApi.apiFileGet({ uID: uid, scope: FileScope.Image })
    if (image.items?.[0]) return image.items[0]

    const video = await fileApi.apiFileGet({ uID: uid, scope: FileScope.Video })
    return video.items?.[0]
  },
}
const sourceMarkdown = ref('')
const fileMeta = ref<FileDto | null>(null)

const articleTags = computed(() =>
  fileMeta.value?.tags ?? [],
)

const hasViewed = ref(false)
let viewingPromise: Promise<void> | null = null

function completePreview(): Promise<void> {
  const uid = fileMeta.value?.uid

  if (!uid || hasViewed.value) {
    return Promise.resolve()
  }

  // 如果滚动到底部时已经开始请求，Back 需要等待同一个请求完成。
  if (viewingPromise) {
    return viewingPromise
  }

  viewingPromise = (async () => {
    try {
      await fileApi.apiFilePreviewCompletePatch({ uID: uid })
      hasViewed.value = true

      if (fileMeta.value?.uid === uid) {
        fileMeta.value.viewCount = (fileMeta.value.viewCount ?? 0) + 1
      }
    } catch (error) {
      console.error('更新文章阅览量失败:', error)
    } finally {
      viewingPromise = null
    }
  })()

  return viewingPromise
}

async function handleEndReached() {
  await completePreview()
}

async function handleBack() {
  await completePreview()

  const uid = fileMeta.value?.uid
  if (uid && hasViewed.value) {
    blogStore.cacheBlogList = blogStore.cacheBlogList.map(blog =>
      blog.uid === uid
        ? { ...blog, viewCount: fileMeta.value?.viewCount }
        : blog,
    )
  }

  await router.push('/blog')
}
const markdownContainer = ref<HTMLElement | null>(null)
let requestVersion = 0

const markdownRenderRef = ref<{
  loading: boolean
} | null>(null)

async function getPreview(
  params: ApiFileDownloadGetRequest,
): Promise<string> {
  try {
    const responseWrapper =
      await fileApi.apiFilePreviewGetRaw(params)

    const response = responseWrapper.raw

    if (response.status === 204) {
      return ''
    }

    if (!response.ok) {
      console.error(await response.text())
      return ''
    }

    return await response.text()
  } catch (error) {
    console.error('获取文章内容失败:', error)
    return ''
  }
}

async function loadArticle(id: string) {
  const currentVersion = ++requestVersion
  articleStore.isLoading = true
  articleStore.activeAnchor = ''
  articleStore.toc = []

  try {
    const [markdown, meta] = await Promise.all([
      getPreview({ uID: id }),
      fileApi.apiFileGet({ uID: id }),
    ])

    if (currentVersion !== requestVersion) {
      return
    }

    const file = meta.items?.[0] ?? null

    sourceMarkdown.value = markdown
    fileMeta.value = file
    hasViewed.value = false
    articleStore.activeAnchor = ''

    if (file) {
      articleStore.blogTitle = file.fileName ?? ''
    } else {
      articleStore.blogTitle = ''
    }

    await nextTick()
    if (currentVersion === requestVersion) articleStore.isLoading = false
  } catch (error) {
    if (currentVersion !== requestVersion) {
      return
    }

    console.error('获取文章信息失败:', error)
    sourceMarkdown.value = ''
    fileMeta.value = null
    articleStore.blogTitle = ''
    articleStore.activeAnchor = ''
    articleStore.toc = []
    articleStore.isLoading = false
  }
}

watch(
  () => route.params.id,
  id => {
    if (typeof id !== 'string' || !id) {
      return
    }

    loadArticle(id)
  },
  {
    immediate: true,
  },
)
watch(
  () => sourceMarkdown.value,
  async markdown => {
    articleStore.markdown = markdown
    articleStore.toc = extractTocTree(markdown)
    articleStore.activeAnchor = ''
    await nextTick()
    handleMarkdownScroll()
  },
  {
    immediate: true,
  },
)
let scrollFrame: number | null = null
let markdownResizeObserver: ResizeObserver | null = null
let markdownMutationObserver: MutationObserver | null = null

function getScrollViewport(): HTMLElement | null {
  return markdownContainer.value
    ?.closest('.el-scrollbar')
    ?.querySelector<HTMLElement>('.el-scrollbar__wrap') ?? null
}

function updateActiveAnchor() {
  const container = markdownContainer.value

  if (!container) {
    return
  }

  const headings = Array.from(
    container.querySelectorAll<HTMLElement>(
      'h2[id], h3[id], h4[id], h5[id], h6[id]',
    ),
  )

  if (!headings.length) {
    articleStore.activeAnchor = ''
    return
  }

  // Element Plus 的实际滚动容器是 scrollbar__wrap，不是 Markdown 内容节点。
  const viewportRect = (
    getScrollViewport() ?? container
  ).getBoundingClientRect()
  const activationLine = viewportRect.top + 32

  // 当前章节取滚动激活线之前最近的标题；尚未滚到第一个标题时取第一个可见标题。
  let activeHeading: HTMLElement | undefined

  for (const heading of headings) {
    if (heading.getBoundingClientRect().top <= activationLine) {
      activeHeading = heading
    } else {
      break
    }
  }

  if (!activeHeading) {
    activeHeading = headings.find(
      heading => heading.getBoundingClientRect().bottom > viewportRect.top,
    )
  }

  if (activeHeading) {
    articleStore.activeAnchor = activeHeading.id
  }
}

function handleMarkdownScroll() {
  if (scrollFrame !== null) {
    return
  }

  scrollFrame = requestAnimationFrame(() => {
    scrollFrame = null
    updateActiveAnchor()
  })
}

onMounted(async () => {
  window.addEventListener('keydown', handleKeydown)

  await nextTick()
  const container = markdownContainer.value

  if (container) {
    // Markdown 节点可能异步替换，图片加载也会改变标题位置。
    markdownMutationObserver = new MutationObserver(handleMarkdownScroll)
    markdownMutationObserver.observe(container, {
      childList: true,
      subtree: true,
    })

    if (typeof ResizeObserver !== 'undefined') {
      markdownResizeObserver = new ResizeObserver(handleMarkdownScroll)
      markdownResizeObserver.observe(container)

      const viewport = getScrollViewport()
      if (viewport) {
        markdownResizeObserver.observe(viewport)
      }
    }
  }

  handleMarkdownScroll()
})

onBeforeUnmount(() => {
  window.removeEventListener('keydown', handleKeydown)
  articleStore.isFullscreen = false
  articleStore.isLoading = false
  requestVersion++

  markdownResizeObserver?.disconnect()
  markdownMutationObserver?.disconnect()
  markdownResizeObserver = null
  markdownMutationObserver = null

  if (scrollFrame !== null) {
    cancelAnimationFrame(scrollFrame)
    scrollFrame = null
  }
})
</script>

<style scoped>
.blog-preview {
  display: grid;

  width: 100%;
  height: 100%;
  min-height: 0;

  grid-template-rows: auto minmax(0, 1fr);
}

.header :deep(.el-page-header__header) {
  height: 50px;
  min-height: 50px;
  border-bottom: 1px solid #ddd;
  padding: 0 20px;
}

.article-header {
  display: flex;
  flex-direction: column;
  justify-content: center;
  gap: 2px;

  min-width: 0;
}

.article-title-skeleton {
  display: block;
  width: min(260px, 45vw);
  height: 16px;
  border-radius: 4px;
  background: linear-gradient(90deg, var(--el-fill-color-light) 25%, var(--el-fill-color) 50%, var(--el-fill-color-light) 75%);
  background-size: 200% 100%;
  animation: skeleton-shimmer 1.4s ease-in-out infinite;
}
.preview-skeleton { max-width: 920px; margin: 0 auto; padding: 30px 32px 48px; }
.skeleton-meta { display: flex; gap: 10px; margin-bottom: 28px; }
.skeleton-meta span, .skeleton-line, .skeleton-block {
  display: block; border-radius: 5px;
  background: linear-gradient(90deg, var(--el-fill-color-light) 25%, var(--el-fill-color) 50%, var(--el-fill-color-light) 75%);
  background-size: 200% 100%;
  animation: skeleton-shimmer 1.4s ease-in-out infinite;
}
.skeleton-meta span { width: 64px; height: 18px; }
.skeleton-line { height: 14px; margin: 16px 0; }
.skeleton-line-title { width: 48%; height: 26px; margin-bottom: 26px; }
.skeleton-line-wide { width: 92%; }
.skeleton-line-medium { width: 72%; }
.skeleton-line-short { width: 38%; }
.skeleton-block { width: 100%; height: 190px; margin: 28px 0; }
.skeleton-block-large { height: 260px; }
@keyframes skeleton-shimmer { 0% { background-position: 100% 0; } 100% { background-position: -100% 0; } }

.article-title-row {
  display: flex;
  align-items: center;
  gap: 20px;

  min-width: 0;
}

.article-title {
  overflow: hidden;

  text-overflow: ellipsis;
  white-space: nowrap;
}

.article-tags {
  display: flex;
  align-items: center;
  gap: 8px;
}

.article-upload-time {
  font-size: 12px;
  line-height: 16px;
  color: #909399;
}

.markdown-scrollbar {
  min-width: 0;
  min-height: 0;
}

.markdown-content {
  min-width: 0;
  min-height: 0;

  overflow-x: hidden;
}

@media (max-width: 900px) {
  .header :deep(.el-page-header__header) {
    min-height: 56px;
    height: auto;
    padding: 8px 12px;
    align-items: flex-start;
  }

  .header :deep(.el-page-header__left) {
    margin-right: 8px;
  }

  .header :deep(.el-page-header__content) {
    min-width: 0;
    flex: 1;
  }

  .header :deep(.el-page-header__extra) {
    margin-left: 4px;
  }

  .preview-skeleton { padding: 22px 14px 32px; }
  .skeleton-block { height: 150px; }
  .skeleton-block-large { height: 210px; }
  .article-title-row {
    align-items: flex-start;
    flex-direction: column;
    gap: 6px;
  }

  .article-title {
    max-width: 100%;
    white-space: normal;
    overflow-wrap: anywhere;
    line-height: 1.4;
  }

  .article-tags {
    flex-wrap: wrap;
    gap: 4px;
  }

  .markdown-content {
    padding: 0 14px 20px;
    overflow-wrap: anywhere;
  }

  .markdown-content :deep(img),
  .markdown-content :deep(video),
  .markdown-content :deep(table) {
    max-width: 100%;
  }

  .markdown-content :deep(table) {
    display: block;
    overflow-x: auto;
  }

  .markdown-content :deep(pre) {
    max-width: 100%;
    overflow-x: auto;
  }
}
</style>