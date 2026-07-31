import type { TodoDto } from '@/api'
import { defineStore } from 'pinia'
import { reactive ,type Ref} from 'vue'

export const useTodoStorage = defineStore('todo', () => {
  const todoList = reactive<TodoDto[]>([])
  return { todoList  }
})
