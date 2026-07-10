<script setup lang="ts">
import { useTagStore } from '@/stores/uuseTagStore'
import {
  ElTable,
  ElTag,
  ElText,
  ElTableColumn,
  ElImage,
  ElDrawer,
  type TableInstance,
  ElMessage,
} from 'element-plus'
import { Plus } from '@element-plus/icons-vue'
import { onBeforeMount, onBeforeUnmount } from 'vue'
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
  tagStore.tags = await tagApi.apiTagGet()
})
</script>

<template>
  <ElTable :data="tagStore.tags" highlight-current-row>
    <ElTableColumn
      sortable
      :prop="item"
      v-for="(item, index) in Object.keys(tagStore.tags[0])"
      :key="item"
      :label="columnMap[item] ?? item"
    >
      <template  #default="scope" v-if="index==1">
          <ElTag v-if="scope.row.name">{{ scope.row.name }}</ElTag>
          <ElText v-else>{{ scope.row.name }}</ElText>
        </template>
    </ElTableColumn>
  </ElTable>
</template>
