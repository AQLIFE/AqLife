import {  TodoApi, type TodoDto } from '@/api'
import { apiConfiguration } from '@/services/api'
import { usePaginatedList } from '@aqlife/ui-shared'
import { defineStore } from 'pinia'
import { ref,reactive } from 'vue'

export const useTodoStore = defineStore('dev-plan', () => {
  const todoList = reactive<TodoDto[]>([])
  const todoApi = new TodoApi(apiConfiguration)
  const page = 1
  const pageSize = 50
  const hasMore = ref(false)
  const action= usePaginatedList(()=>todoApi.apiTodoGet() )

 
  return { todoList, hasMore }
})
