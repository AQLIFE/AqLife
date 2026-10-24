<template>
  <div class="blog-preview">
    <ElPageHeader class="header" :icon="ArrowLeft" @back="router.push('/blog')">
      <template #content>
        <div class="article-header">
          <div class="article-title-row">
            <span class="article-title">
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
        <ElButton :icon="Share" link title="分享文章" />
      </template>
    </ElPageHeader>

    <ElScrollbar class="markdown-scrollbar" @scroll="handleMarkdownScroll" @end-reached="handleEndReached">
      <div ref="markdownContainer" class="markdown-content" v-loading="markdownRenderRef?.loading">
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
} from 'element-plus'
import {
  ArrowLeft,
  Share,
} from '@element-plus/icons-vue'
import {
  onBeforeUnmount,
  ref,
  computed,
  watch,
  onMounted,
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
import { MarkdownRender } from '@aqlife/ui-shared'
import { extractTocTree } from '@aqlife/domain'

const route = useRoute()
const router = useRouter()
const articleStore = useArticleStore()
const fileApi = new FileApi(apiConfiguration)
const baseurl = import.meta.env.VITE_API

const markdownFileResolver = {
  async getFile(uid: string): Promise<FileDto | undefined> {
    const result = await fileApi.apiFileGet({ uID: uid,scope:FileScope.Image })
    return result.items?.[0]
  },
}
const sourceMarkdown = ref('')
const fileMeta = ref<FileDto | null>(null)

const articleTags = computed(() =>
  fileMeta.value?.tags ?? [],
)

const hasViewed = ref(false)
const isViewing = ref(false)

async function handleEndReached() {
  if (hasViewed.value || isViewing.value) {
    return
  }

  isViewing.value = true

  try {
    await fileApi.apiFilePreviewCompletePatch({uID:fileMeta.value?.uid})

    hasViewed.value = true
  } finally {
    isViewing.value = false
  }
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

    if (file) {
      articleStore.blogTitle = file.fileName ?? ''
    } else {
      articleStore.blogTitle = ''
    }
  } catch (error) {
    if (currentVersion !== requestVersion) {
      return
    }

    console.error('获取文章信息失败:', error)
    sourceMarkdown.value = ''
    fileMeta.value = null
    articleStore.blogTitle = ''
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
  markdown => {
    articleStore.markdown = markdown
    articleStore.toc = extractTocTree(markdown)
  },
  {
    immediate: true,
  },
)
let scrollFrame: number | null = null

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

  const containerTop = container.getBoundingClientRect().bottom/* container.getBoundingClientRect().top */

  const visibleHeadings = headings.filter(heading => {
    const rect = heading.getBoundingClientRect()

    return (
      rect.bottom > containerTop &&
      rect.top < container.getBoundingClientRect().bottom
    )
  })

  if (!visibleHeadings.length) {
    return
  }

  let closest = visibleHeadings[0]
  let closestDistance = Math.abs(
    closest.getBoundingClientRect().top - containerTop,
  )

  for (const heading of visibleHeadings.slice(1)) {
    const distance = Math.abs(
      heading.getBoundingClientRect().top - containerTop,
    )

    if (distance < closestDistance) {
      closest = heading
      closestDistance = distance
    }
  }

  articleStore.activeAnchor = closest.id
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

onMounted(() => {
  // markdownContainer.value?.addEventListener(
  //   'scroll',
  //   handleMarkdownScroll,
  //   { passive: true },
  // )

  updateActiveAnchor()
})

onBeforeUnmount(() => {
  requestVersion++

  // markdownContainer.value?.removeEventListener(
  //   'scroll',
  //   handleMarkdownScroll,
  // )

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
