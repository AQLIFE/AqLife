import { TodoApi, type TodoDto } from '@/api'
import { apiConfiguration } from '@/services/api'
import { defineStore } from 'pinia'
import { ref } from 'vue'

export const useTodoStore = defineStore('dev-plan', () => {
  const todoList = ref<TodoDto[]>([])
  const hasMore = ref(true)
  const loading = ref(false)
  const error = ref<unknown>(null)

  const api = new TodoApi(apiConfiguration)

  const load = async (reset = false) => {
    if (loading.value) return

    if (reset) {
      todoList.value = []
      hasMore.value = true
    }

    if (!hasMore.value) return

    loading.value = true
    error.value = null

    try {
      const page = reset ? 1 : Math.floor(todoList.value.length / 50) + 1
      const response = await api.apiTodoGet({
        page,
        pageSize: 50,
        isTree: true,
      })

      const items = response.items ?? []
      todoList.value = reset ? items : [...todoList.value, ...items]
      hasMore.value = response.hasMore ?? false
    } catch (e) {
      error.value = e
    } finally {
      loading.value = false
    }
  }

  const refresh = () => load(true)
  const loadMore = () => load(false)

  return {
    todoList,
    hasMore,
    loading,
    error,
    load,
    refresh,
    loadMore,
  }
})
