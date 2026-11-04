import { DevelopmentPlanApi, TodoApi, type DevelopmentPlanDto, type TodoDto } from '@/api'
import { defineStore } from 'pinia'
import { computed, ref } from 'vue'
import { apiConfiguration } from '@/services/api'

const PAGE_SIZE = 500

export const useTodoStorage = defineStore('todo', () => {
  const plans = ref<DevelopmentPlanDto[]>([])
  const selectedPlanId = ref('')
  const todoList = ref<TodoDto[]>([])
  const loadingPlans = ref(false)
  const loadingTodos = ref(false)
  const includeArchived = ref(false)
  const error = ref<unknown>(null)
  const todoApi = new TodoApi(apiConfiguration)
  const planApi = new DevelopmentPlanApi(apiConfiguration)
  const selectedPlan = computed(() => plans.value.find(x => x.uid === selectedPlanId.value) ?? null)

  async function fetchPlans(showArchived = false) {
    loadingPlans.value = true
    error.value = null
    try {
      plans.value = await planApi.apiDevelopmentPlansGet({ includeArchived: showArchived })
      if (!plans.value.some(x => x.uid === selectedPlanId.value && !x.isArchived)) {
        selectedPlanId.value = plans.value.find(x => !x.isArchived)?.uid ?? ''
      }
      if (selectedPlanId.value) await fetchTodos()
      else todoList.value = []
    } catch (e) {
      error.value = e
      throw e
    } finally {
      loadingPlans.value = false
    }
  }

  async function fetchTodos() {
    if (!selectedPlanId.value) { todoList.value = []; return }
    loadingTodos.value = true
    error.value = null
    try {
      const result: TodoDto[] = []
      let page = 1
      while (true) {
        const response = await todoApi.apiTodoGet({
          planId: selectedPlanId.value, page, pageSize: PAGE_SIZE, isTree: true,
          includeCompleted: true, includeArchived: includeArchived.value,
        })
        result.push(...(response.items ?? []))
        if (!response.hasMore) break
        page = (response.page ?? page) + 1
      }
      todoList.value = result
    } catch (e) {
      error.value = e
      throw e
    } finally {
      loadingTodos.value = false
    }
  }

  async function selectPlan(planId: string) {
    selectedPlanId.value = planId
    await fetchTodos()
  }

  async function createPlan(name: string, description?: string) {
    const uid = await planApi.apiDevelopmentPlansPost({ createDevelopmentPlanCommand: { name, description } })
    await fetchPlans()
    if (uid && plans.value.some(x => x.uid === uid)) await selectPlan(uid)
  }

  async function updatePlan(plan: DevelopmentPlanDto) {
    if (!plan.uid) return
    await planApi.apiDevelopmentPlansPatch({
      updateDevelopmentPlanCommand: { uid: plan.uid, name: plan.name ?? '', description: plan.description ?? '' },
    })
    await fetchPlans()
  }

  async function archivePlan(planId: string) {
    await planApi.apiDevelopmentPlansUidArchivePost({ uid: planId })
    await fetchPlans()
  }

  return {
    plans, selectedPlanId, selectedPlan, todoList, loadingPlans, loadingTodos,
    includeArchived, error, fetchPlans, fetchTodos, selectPlan,
    createPlan, updatePlan, archivePlan,
  }
})
