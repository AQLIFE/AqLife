<script setup lang="ts">
import {
  ElRow,
  ElCol,
  ElTable,
  ElTableColumn,
  ElTag,
  ElText,
  ElMessage,
} from 'element-plus'
import { onBeforeMount, ref } from 'vue'
import { TodoApi, type TodoDto } from '@/api'
import { apiConfiguration } from '@/services/api'
import { useTodoStorage } from '@/stores/useTodoStore'

const columnMap: Record<string, string> = {
  uid: '唯一标识',
  desc: '待办内容',
  status: '状态',
  priority: '优先级',
  ftid: '父待办',
}

const todoStore = useTodoStorage()
const proxyTodos = ref<TodoDto[]>([])
const isLoading = ref(true)
const todoApi = new TodoApi(apiConfiguration)

onBeforeMount(async () => {
  isLoading.value = true
  try {
    await todoStore.fetchAllTodos(todoApi)
    proxyTodos.value = [...todoStore.todoList]
  } catch (error) {
    ElMessage.error(error instanceof Error ? error.message : '待办数据加载失败')
  } finally {
    isLoading.value = false
  }
})

const tableColumns = Object.keys(columnMap)
</script>

<template>
  <ElRow>
    <ElTable
      :data="proxyTodos"
      v-loading="isLoading"
      element-loading-text="正在加载待办..."
      empty-text="暂无待办"
      row-key="uid"
      class="fitHeight"
      highlight-current-row
      :tree-props="{ children: 'todoList', hasChildren: 'hasChildren' }"
    >
      <ElTableColumn
        v-for="item in tableColumns"
        :key="item"
        :prop="item"
        :label="columnMap[item]"
      >
        <template #default="scope">
          <ElTag v-if="item === 'status' && scope.row.status">
            {{ scope.row.status }}
          </ElTag>
          <ElText v-else>
            {{ item === 'ftid' ? (scope.row.ftid ?? '无') : scope.row[item] }}
          </ElText>
        </template>
      </ElTableColumn>
    </ElTable>
  </ElRow>
</template>

<style lang="css" scoped>
.fitHeight {
  height: 100%;
}
</style>
