<template>
  <div class="blog-page">
    <header class="blog-page-header">
      <div>
        <div class="blog-page-eyebrow">
          ARTICLES
        </div>

        <h1 class="blog-page-title">
          文章
        </h1>
      </div>

      <span class="blog-page-count">
        {{ blogStore.cacheBlogList.length }}
      </span>
    </header>

    <ElScrollbar @end-reached="loadBlogs" height="100%" class="blog-scrollbar">
      <div class="blog-grid" v-loading="isLoading" element-loading-text="正在加载文章...">
        <BlogCard v-for="item in blogStore.cacheBlogList" :key="item.uid" :blog="item" />
      </div>
    </ElScrollbar>
  </div>
</template>

<script setup lang="ts">
import { FileApi } from '@/api'
import BlogCard from '@/components/BlogCard.vue'
import { apiConfiguration } from '@/services/api'
import { useBlogStore } from '@/stores/fileStore'
import { ElMessage, ElScrollbar } from 'element-plus'
import { onBeforeMount, ref } from 'vue'

const blogStore = useBlogStore()
const isLoading = ref(false)
const page = ref<number>(1)

async function loadBlogs() {
  const fileApi = new FileApi(apiConfiguration)
  const files = blogStore.cacheBlogList ?? []
  // const pageSize = 10: 请求时默认为10

  if (blogStore.hasMore || blogStore.cacheBlogList.length == 0) {
    isLoading.value = true
    const result = await fileApi.apiFileGet({ page: page.value })
    files.push(...(result.items ?? []))
    page.value = (result.page ?? 0) + 1
    blogStore.hasMore = result.hasMore ?? false
    blogStore.setBlogList(files)
    isLoading.value = false
  }

}

onBeforeMount(async () => {
  if (blogStore.cacheBlogList.length > 0) {
    return
  }

  isLoading.value = true

  try {
    await loadBlogs()
  } catch {
    blogStore.clearBlogList()
    ElMessage.warning('请求数据失败')
  } finally {
    isLoading.value = false
  }
})
</script>

<style lang="css" scoped>
.blog-page {
  width: 100%;
  height: 100%;

  min-width: 0;
  min-height: 0;

  display: flex;
  flex-direction: column;

  overflow: hidden;

  padding: 28px 32px 32px;

  box-sizing: border-box;
}

.blog-page-header {
  flex: 0 0 auto;

  display: flex;
  padding:0 20px;

  align-items: flex-end;

  justify-content: space-between;

  margin-bottom: 22px;
}

.blog-page-eyebrow {
  margin-bottom: 5px;

  color: var(--el-color-primary);

  font-size: 11px;
  font-weight: 600;

  letter-spacing: 0.12em;
}

.blog-page-title {
  margin: 0;

  color: var(--el-text-color-primary);

  font-size: 24px;
  font-weight: 650;

  line-height: 1.3;
}

.blog-page-count {
  color: var(--el-text-color-placeholder);

  font-size: 12px;
}

.blog-grid {
  flex: 1 1 auto;

  min-width: 0;
  min-height: 0;
  height:inherit;

  display: grid;
  grid-template-columns: repeat(2, minmax(0, 1fr));

  align-content: start;

  gap: 16px;

  overflow-y: auto;

  padding: 2px;

  box-sizing: border-box;
  scrollbar-width: none;
}
.blog-scrollbar{
  padding:0 20px;
}
</style>
