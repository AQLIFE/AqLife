<template>
  <div class="blog-preview">
    <ElPageHeader class="header" :icon="ArrowLeft" @back="router.push('/blog')">
      <template #content>
        <div class="article-header">
          <span class="article-title">
            {{ fileMeta?.fileName }}
          </span>

          <div v-if="articleTags.length" class="article-tags">
            <ElTag v-for="tag in articleTags" :key="tag.uid!">
              {{ tag.name }}
            </ElTag>
          </div>
        </div>
      </template>

      <template #extra>
        <ElButton :icon="Share" link title="分享文章" />
      </template>
    </ElPageHeader>

    <div class="markdown-content">
      <MarkdownRender :markdown="sourceMarkdown" :baseurl="baseurl" />
    </div>
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
} from 'vue'
import {
  useRoute,
  useRouter,
} from 'vue-router'

import {
  FileApi,
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
const sourceMarkdown = ref('')
const fileMeta = ref<FileDto | null>()

const articleTags = computed(() =>
  fileMeta.value?.tags ?? [],
)

let requestVersion = 0

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

  const [markdown, meta] = await Promise.all([
    getPreview({ uID: id }),
    fileApi.apiFileGet({ uID: id }),
  ])

  // 如果期间路由已经切换，丢弃旧请求结果
  if (currentVersion !== requestVersion) {
    return
  }

  sourceMarkdown.value = markdown
  fileMeta.value = meta[0]

  const file = meta[0]

  if (file) {
    articleStore.blogTitle = file.fileName ?? ''
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
onBeforeUnmount(() => {
  requestVersion++
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

.markdown-content {
  min-width: 0;
  min-height: 0;

  overflow-x: hidden;
  overflow-y: auto;
  scrollbar-width: none;
}
</style>