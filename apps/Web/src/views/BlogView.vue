<template>
  <ElRow class="layout" :gutter="10" v-lodding="isLodding">
    <ElCol v-for="item in blogStore.cacheBlogList" :key="item.uid" :span="12">
      <BlogCard  :blog="item"/>
    </ElCol>
  </ElRow>
</template>

<script setup lang="ts">
import { FileApi } from '@/api'
import BlogCard from '@/components/BlogCard.vue'
import { apiConfiguration } from '@/services/api'
import { useBlogStore } from '@/stores/useBlogStore'
import { ElRow, ElCol, ElMessage } from 'element-plus'
import { onBeforeMount,ref } from 'vue'

const blogStore = useBlogStore()
const isLodding = ref<boolean>(!blogStore.cacheBlogList)
onBeforeMount(async () => {
  const fileApi = new FileApi(apiConfiguration)
  try {
    
    if(blogStore.cacheBlogList.length === 0)
      blogStore.cacheBlogList = await fileApi.apiFileGet()
    // else ElMessage.success('使用缓存')
    } catch {
      blogStore.cacheBlogList = []
      ElMessage.warning('请求数据失败')
    }
})
</script>

<style lang="css" scoped>
.layout {
  height: 100%;
  /* 自动填满 #content 分配给它的 1fr 空间 */
  overflow-y: auto;
  /* 开启内部滚动 */
  align-content: start;
  /*避免头部遮挡*/
  justify-content: flex-start;
}

.layout::-webkit-scrollbar {
  display: none;
}

.el-card {
  margin-bottom: 10px;
}

.el-card .header {
  display: grid;
  grid-template-columns: 3fr 1fr;
  text-align: start;
}

.el-card .el-tree {
  padding-left: 10px;
}

.el-card__footer > div {
  font-size: 12px;
  text-align: right;
  color: var(--back_color_lv2);
}
</style>
