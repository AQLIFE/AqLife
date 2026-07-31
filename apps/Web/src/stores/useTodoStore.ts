import { TodoApi, type TodoDto } from '@/api'
import { apiConfiguration } from '@/services/api'
import { handle } from '@/utils/request'
import { defineStore } from 'pinia'
import { ref } from 'vue'

export const useTodoStore = defineStore('dev-plan', () => {
  const todoList = ref<TodoDto[]>([])
  const api = new TodoApi(apiConfiguration)
  const getAll = async () => {
    const [response, status] = await handle(api.apiTodoGet())
    if (status) {
      todoList.value = response!
    }
  }
  return { todoList, getAll }
})
