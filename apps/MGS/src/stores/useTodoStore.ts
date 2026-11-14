import { DevelopmentPlanApi, TodoApi, type DevelopmentPlanDto, type TodoDto } from '@/api'
import { defineStore } from 'pinia'
import { computed, ref } from 'vue'
import { apiConfiguration } from '@/services/api'
import { useLoadingTask } from '@aqlife/ui-shared'

const PAGE_SIZE = 500

export const useTodoStorage = defineStore('todo', () => {
  const plans = ref<DevelopmentPlanDto[]>([])
  const selectedPlanId = ref('')
  const todoList = ref<TodoDto[]>([])
  const plansLoading = useLoadingTask('mgs:todo:plans')
  const todosLoading = useLoadingTask('mgs:todo:todos')
  const loadingPlans = plansLoading.isLoading
  const loadingTodos = todosLoading.isLoading
  const error = ref<unknown>(null)
  const todoApi = new TodoApi(apiConfiguration)
  const planApi = new DevelopmentPlanApi(apiConfiguration)
  const selectedPlan = computed(() => plans.value.find(x => x.uid === selectedPlanId.value) ?? null)

  async function fetchPlans(showArchived = false) {
    error.value = null
    await plansLoading.run(async () => {
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
      }
    })
  }

  async function fetchTodos() {
    if (!selectedPlanId.value) {
      todoList.value = []
      return
    }

    error.value = null
    await todosLoading.run(async () => {
      try {
        const result: TodoDto[] = []
        let page = 1
        while (true) {
          const response = await todoApi.apiTodoGet({
            planId: selectedPlanId.value,
            page,
            pageSize: PAGE_SIZE,
          })
          result.push(...(response.items ?? []))
          if (!response.hasMore) break
          page = (response.page ?? page) + 1
        }
        todoList.value = result
      } catch (e) {
        error.value = e
        throw e
      }
    })
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
    error, fetchPlans, fetchTodos, selectPlan,
    createPlan, updatePlan, archivePlan,
  }
})
