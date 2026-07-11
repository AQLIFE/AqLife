<script setup lang="ts">
import { useTagStore } from '@/stores/uuseTagStore'
import {
  ElTable,
  ElTag,
  ElText,
  ElEmpty,
  ElTableColumn,
  ElImage,
  ElDrawer,
  type TableInstance,
  ElMessage,
} from 'element-plus'
import { Plus } from '@element-plus/icons-vue'
import { onBeforeMount, onBeforeUnmount,computed } from 'vue'
import { TagApi } from '@/api'
import { apiConfiguration } from '@/services/api'
const columnMap: Record<string, string> = {
  uid: '唯一标识',
  name: '标签名称',
  aliasName: '标签别名',
  isCategory: '分类标识',
}
const tagStore = useTagStore()
const tagApi = new TagApi(apiConfiguration)
onBeforeMount(async()=>{
  if(tagStore.tags.length==0)tagStore.tags = await tagApi.apiTagGet()
})

const tableColumns = computed(() => {
  // 1. 安全守卫：确保数组存在且不为空 [cite: 16]
  if (!tagStore.tags || tagStore.tags.length === 0) {
    return []
  }
  // 2. 提取键名，过滤掉不需要展示的字段（如 UID）
  return Object.keys(tagStore.tags[0])
})
</script>

<template>
  <ElTable :data="tagStore.tags" class="fitHeight" highlight-current-row v-if="tagStore.tags && tagStore.tags.length > 0">
    <ElTableColumn
      sortable
      :prop="item"
      v-for="(item, index) in tableColumns"
      :key="item"
      :label="columnMap[item] ?? item"
    >
      <template  #default="scope" v-if="index==1">
          <ElTag v-if="scope.row.name">{{ scope.row.name }}</ElTag>
          <ElText v-else>{{ scope.row.name }}</ElText>
        </template>
    </ElTableColumn>
  </ElTable>
  <ElEmpty v-else description="正在加载标签数据..." />
</template>
