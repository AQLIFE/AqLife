import { DevelopmentPlanApi, TodoApi, type DevelopmentPlanDto, type TodoDto } from '@/api'
import { apiConfiguration } from '@/services/api'
import { defineStore } from 'pinia'
import { computed, ref } from 'vue'

const PAGE_SIZE = 500

// Public Web client: deliberately read-only. All mutations belong to MGS.
export const useTodoStore = defineStore('dev-plan', () => {
  const plans = ref<DevelopmentPlanDto[]>([])
  const selectedPlanId = ref('')
  const todoList = ref<TodoDto[]>([])
  const loading = ref(false)
  const loadingPlans = ref(false)
  const error = ref<unknown>(null)
  const selectedPlan = computed(() => plans.value.find(x => x.uid === selectedPlanId.value) ?? null)
  const todoApi = new TodoApi(apiConfiguration)
  const planApi = new DevelopmentPlanApi(apiConfiguration)

  async function fetchTodos() {
    if (!selectedPlanId.value) { todoList.value = []; return }
    loading.value = true
    error.value = null
    try {
      const items: TodoDto[] = []
      let page = 1
      while (true) {
        const response = await todoApi.apiTodoGet({
          planId: selectedPlanId.value,
          page,
          pageSize: PAGE_SIZE,
        })
        items.push(...(response.items ?? []))
        if (!response.hasMore) break
        page = (response.page ?? page) + 1
      }
      todoList.value = items
    } catch (e) {
      error.value = e
      throw e
    } finally {
      loading.value = false
    }
  }

  async function fetchPlans() {
    loadingPlans.value = true
    error.value = null
    try {
      plans.value = await planApi.apiDevelopmentPlansGet({})
      if (!plans.value.some(x => x.uid === selectedPlanId.value && !x.isArchived))
        selectedPlanId.value = plans.value.find(x => !x.isArchived)?.uid ?? ''
      await fetchTodos()
    } catch (e) {
      error.value = e
      throw e
    } finally {
      loadingPlans.value = false
    }
  }

  async function selectPlan(planId: string) {
    if (planId === selectedPlanId.value) return
    selectedPlanId.value = planId
    await fetchTodos()
  }

  const refresh = () => fetchTodos()

  return {
    plans, selectedPlanId, selectedPlan, todoList, loading, loadingPlans, error,
    fetchPlans, fetchTodos, selectPlan, refresh,
  }
})
