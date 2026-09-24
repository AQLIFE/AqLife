import type { TodoApi, TodoDto } from '@/api'
import { defineStore } from 'pinia'
import { reactive } from 'vue'

export const useTodoStorage = defineStore('todo', () => {
  const todoList = reactive<TodoDto[]>([])

  async function fetchAllTodos(todoApi: TodoApi) {
    if (todoList.length > 0) return

    let page = 1
    const pageSize = 50

    while (true) {
      const result = await todoApi.apiTodoGet({
        page,
        pageSize,
        isTree: true,
      })

      todoList.push(...(result.items ?? []))

      if (!result.hasMore) break
      page = (result.page ?? page) + 1
    }
  }

  return { todoList, fetchAllTodos }
})
