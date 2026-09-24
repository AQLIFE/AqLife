<script setup lang="ts">
import {
  ElEmpty,
  ElRow,
  ElCol,
  ElTable,
  ElTableColumn,
  ElTag,
  ElText,
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
const todoApi = new TodoApi(apiConfiguration)

onBeforeMount(async () => {
  await todoStore.fetchAllTodos(todoApi)
  proxyTodos.value = [...todoStore.todoList]
})

const tableColumns = Object.keys(columnMap)
</script>

<template>
  <ElRow>
    <ElTable
      :data="proxyTodos"
      row-key="uid"
      class="fitHeight"
      highlight-current-row
      v-if="todoStore.todoList.length > 0"
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

    <ElEmpty v-else description="正在加载待办数据..." />
  </ElRow>
</template>

<style lang="css" scoped>
.fitHeight {
  height: 100%;
}
</style>
