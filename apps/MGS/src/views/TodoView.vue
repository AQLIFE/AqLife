<script setup lang="ts">
import { notify } from '@aqlife/ui-shared'
import { computed, onMounted, reactive, ref } from 'vue'
import {
  ElButton, ElDialog, ElForm, ElFormItem, ElInput, ElMessageBox,
  ElOption, ElSelect, ElSegmented, ElSwitch, ElTable, ElTableColumn, ElTag,
} from 'element-plus'
import { Plus, Edit, Refresh, FolderAdd } from '@element-plus/icons-vue'
import { TodoApi, TodoStatus, type TodoDto } from '@/api'
import { apiConfiguration } from '@/services/api'
import { useTodoStorage } from '@/stores/useTodoStore'
import MgsPageHeader from '../components/ui/MgsPageHeader.vue'
import MgsToolbar from '@/components/ui/MgsToolbar.vue'
import MgsPageContainer from '@/components/ui/MgsPageContainer.vue'
import { mgsIconRegistry, MgsIconName } from '@aqlife/icons'

const Archive = mgsIconRegistry[MgsIconName.Archive]
const DEFAULT_PLAN_ID = '00000000-0000-0000-0000-000000000001'
const store = useTodoStorage()
const api = new TodoApi(apiConfiguration)
const taskDialog = ref(false)
const planDialog = ref(false)
const saving = ref(false)
const editing = ref<TodoDto | null>(null)
const editingPlan = ref(false)
const statusFilter = ref<number | 'all'>('all')
const showArchived = ref(false)
const task = reactive({ title: '', description: '', sortOrder: 0, status: TodoStatus.Initial as TodoStatus })
const plan = reactive({ name: '', description: '' })

const statuses = [
  { label: '全部状态', value: 'all' },
  { label: '初始', value: TodoStatus.Initial },
  { label: '等待分配', value: TodoStatus.Wait },
  { label: '进行中', value: TodoStatus.Execute },
  { label: '已完成', value: TodoStatus.Completed },
]

function statusValue(status?: string | null): number {
  if (status == null || status === '') return TodoStatus.Initial
  const n = Number(status)
  if (Number.isInteger(n) && n >= 0 && n <= 3) return n
  const s = status.toLowerCase()
  if (s.includes('wait')) return TodoStatus.Wait
  if (s.includes('execut') || s.includes('progress')) return TodoStatus.Execute
  if (s.includes('complet') || s.includes('done')) return TodoStatus.Completed
  return TodoStatus.Initial
}
function statusLabel(status?: string | null) {
  return ['初始', '等待分配', '进行中', '已完成'][statusValue(status)]
}
function statusType(status?: string | null): 'success' | 'warning' | 'info' | undefined {
  return ({
    [TodoStatus.Wait]: 'info',
    [TodoStatus.Execute]: 'warning',
    [TodoStatus.Completed]: 'success',
  } as Record<number, 'success' | 'warning' | 'info'>)[statusValue(status)]
}
const visibleTodos = computed(() => store.todoList.filter(todo => {
  if (!showArchived.value && todo.isArchived) return false
  return statusFilter.value === 'all' || statusValue(todo.status) === statusFilter.value
}))

async function refresh() {
  try { await store.fetchPlans() }
  catch (e) { notify.error(e instanceof Error ? e.message : '开发计划加载失败') }
}
onMounted(refresh)
function createTask() {
  editing.value = null
  Object.assign(task, { title: '', description: '', sortOrder: 0, status: TodoStatus.Initial })
  taskDialog.value = true
}
function editTask(todo: TodoDto) {
  editing.value = todo
  Object.assign(task, {
    title: todo.title ?? '',
    description: todo.description ?? '',
    sortOrder: todo.sortOrder ?? 0,
    status: statusValue(todo.status) as TodoStatus,
  })
  taskDialog.value = true
}
async function saveTask() {
  if (!task.title.trim()) { notify.warning('请填写任务标题'); return }
  if (!store.selectedPlanId) { notify.warning('请先选择开发计划'); return }
  if (!Number.isInteger(task.sortOrder) || task.sortOrder < 0) { notify.warning('排序值必须是非负整数'); return }

  saving.value = true
  try {
    const current = editing.value
    if (current?.uid) {
      await api.apiTodoDetailsPatch({
        updateTodoCommand: {
          uid: current.uid,
          planId: store.selectedPlanId,
          title: task.title.trim(),
          description: task.description,
          sortOrder: task.sortOrder,
        },
      })
      if (statusValue(current.status) !== task.status) {
        await api.apiTodoStatusPatch({
          updateTodoStatusCommand: {
            uid: current.uid,
            planId: store.selectedPlanId,
            status: task.status,
          },
        })
      }
    } else {
      await api.apiTodoPost({
        createTodoCommand: {
          planId: store.selectedPlanId,
          title: task.title.trim(),
          description: task.description,
          sortOrder: task.sortOrder,
        },
      })
    }
    taskDialog.value = false
    await store.fetchTodos()
    notify.success('任务已保存')
  } catch (e) {
    notify.error(e instanceof Error ? e.message : '保存任务失败')
    await store.fetchTodos().catch(() => undefined)
  } finally {
    saving.value = false
  }
}
async function archiveTask(todo: TodoDto) {
  if (!todo.uid || !store.selectedPlanId) return
  try {
    await ElMessageBox.confirm('归档后任务仍保留历史记录，确定继续？', '归档任务', { type: 'warning' })
    await api.apiTodoArchivePatch({
      archiveTodoCommand: { uid: todo.uid, planId: store.selectedPlanId },
    })
    await store.fetchTodos()
    notify.success('任务已归档')
  } catch (e) {
    if (e !== 'cancel' && e !== 'close') notify.error(e instanceof Error ? e.message : '归档失败')
  }
}
async function restoreTask(todo: TodoDto) {
  if (!todo.uid || !store.selectedPlanId) return
  try {
    await api.apiTodoRestorePatch({
      restoreTodoCommand: { uid: todo.uid, planId: store.selectedPlanId },
    })
    await store.fetchTodos()
    notify.success('任务已恢复')
  } catch (e) {
    notify.error(e instanceof Error ? e.message : '恢复失败')
  }
}
function createPlan() {
  editingPlan.value = false
  Object.assign(plan, { name: '', description: '' })
  planDialog.value = true
}
function editPlan() {
  if (!store.selectedPlan) return
  editingPlan.value = true
  Object.assign(plan, { name: store.selectedPlan.name ?? '', description: store.selectedPlan.description ?? '' })
  planDialog.value = true
}
async function savePlan() {
  if (!plan.name.trim()) { notify.warning('请填写计划名称'); return }
  saving.value = true
  try {
    const currentPlan = store.selectedPlan
    if (editingPlan.value && currentPlan?.uid)
      await store.updatePlan({ ...currentPlan, name: plan.name.trim(), description: plan.description })
    else
      await store.createPlan(plan.name.trim(), plan.description)
    planDialog.value = false
    notify.success('开发计划已保存')
  } catch (e) {
    notify.error(e instanceof Error ? e.message : '保存计划失败')
  } finally {
    saving.value = false
  }
}
async function archivePlan() {
  if (!store.selectedPlan?.uid) return
  try {
    await ElMessageBox.confirm('请先归档计划中的所有任务，再归档计划。确定继续？', '归档开发计划', { type: 'warning' })
    await store.archivePlan(store.selectedPlan.uid)
    notify.success('计划已归档')
  } catch (e) {
    if (e !== 'cancel' && e !== 'close') notify.error(e instanceof Error ? e.message : '归档计划失败')
  }
}
</script>

<template>
  <div class="todo-view-root">
    <MgsPageHeader title="开发计划" description="管理平铺任务、执行状态与前置依赖关系。">
      <template #actions>
        <ElButton :icon="Refresh" @click="refresh">刷新</ElButton>
        <ElButton :icon="FolderAdd" @click="createPlan">新建计划</ElButton>
        <ElButton type="primary" :icon="Plus" :disabled="!store.selectedPlan || store.selectedPlan.isArchived" @click="createTask">添加任务</ElButton>
      </template>
    </MgsPageHeader>
    <MgsToolbar>
      <div class="plan-toolbar">
        <ElSelect :model-value="store.selectedPlanId" placeholder="选择开发计划" @change="store.selectPlan">
          <ElOption v-for="item in store.plans" :key="item.uid" :label="item.name ?? '未命名计划'" :value="item.uid!" />
        </ElSelect>
        <ElButton :disabled="!store.selectedPlan || store.selectedPlan.isArchived" :icon="Edit" @click="editPlan">编辑计划</ElButton>
        <ElButton type="danger" plain :disabled="!store.selectedPlan || store.selectedPlan.isArchived || store.selectedPlan.uid === DEFAULT_PLAN_ID" :icon="Archive" @click="archivePlan">归档计划</ElButton>
      </div>
      <ElSegmented v-model="statusFilter" :options="statuses" />
      <ElSwitch v-model="showArchived" active-text="显示归档任务" />
    </MgsToolbar>
    <MgsPageContainer>
      <ElTable :data="visibleTodos" v-loading="store.loadingPlans || store.loadingTodos" row-key="uid" class="todo-table" empty-text="此开发计划还没有任务">
        <ElTableColumn label="任务" min-width="280">
          <template #default="{ row }">
            <div class="task-title">{{ row.title || '（未命名任务）' }}</div>
            <div v-if="row.description" class="task-description">{{ row.description }}</div>
            <div class="task-meta">
              <span v-if="row.isBlocked" class="blocked">阻塞中 · {{ row.incompleteDependencyCount ?? 0 }} 项前置未完成</span>
              <span v-else-if="row.predecessorIds?.length">前置依赖 {{ row.predecessorIds.length }} 项</span>
              <span v-if="row.isArchived">已归档</span>
            </div>
          </template>
        </ElTableColumn>
        <ElTableColumn label="状态" width="110">
          <template #default="{ row }"><ElTag :type="statusType(row.status)">{{ statusLabel(row.status) }}</ElTag></template>
        </ElTableColumn>
        <ElTableColumn prop="sortOrder" label="排序" width="90" />
        <ElTableColumn label="操作" min-width="180" fixed="right">
          <template #default="{ row }">
            <ElButton link type="primary" :icon="Edit" :disabled="row.isArchived || store.selectedPlan?.isArchived" @click="editTask(row)">编辑</ElButton>
            <ElButton v-if="!row.isArchived" link type="warning" :icon="Archive" :disabled="store.selectedPlan?.isArchived" @click="archiveTask(row)">归档</ElButton>
            <ElButton v-else link type="success" @click="restoreTask(row)">恢复</ElButton>
          </template>
        </ElTableColumn>
      </ElTable>
    </MgsPageContainer>

    <ElDialog v-model="taskDialog" :title="editing ? '编辑任务' : '创建任务'" width="min(620px, 94vw)" destroy-on-close>
      <ElForm label-position="top">
        <ElFormItem label="任务标题" required><ElInput v-model="task.title" maxlength="500" show-word-limit /></ElFormItem>
        <ElFormItem label="任务说明"><ElInput v-model="task.description" type="textarea" :rows="4" maxlength="10000" show-word-limit /></ElFormItem>
        <ElFormItem label="排序值"><ElInput v-model.number="task.sortOrder" /></ElFormItem>
        <ElFormItem v-if="editing" label="状态">
          <ElSelect v-model="task.status" class="full-width">
            <ElOption label="初始" :value="TodoStatus.Initial" />
            <ElOption label="等待分配" :value="TodoStatus.Wait" />
            <ElOption label="进行中" :value="TodoStatus.Execute" />
            <ElOption label="已完成" :value="TodoStatus.Completed" />
          </ElSelect>
        </ElFormItem>
      </ElForm>
      <template #footer><ElButton @click="taskDialog = false">取消</ElButton><ElButton type="primary" :loading="saving" @click="saveTask">保存</ElButton></template>
    </ElDialog>

    <ElDialog v-model="planDialog" :title="editingPlan ? '编辑开发计划' : '新建开发计划'" width="min(560px, 94vw)">
      <ElForm label-position="top">
        <ElFormItem label="计划名称" required><ElInput v-model="plan.name" maxlength="120" /></ElFormItem>
        <ElFormItem label="计划说明"><ElInput v-model="plan.description" type="textarea" :rows="3" maxlength="2000" /></ElFormItem>
      </ElForm>
      <template #footer><ElButton @click="planDialog = false">取消</ElButton><ElButton type="primary" :loading="saving" @click="savePlan">保存</ElButton></template>
    </ElDialog>
  </div>
</template>

<style scoped>
.todo-view-root { min-height: 100%; }
.plan-toolbar { display: flex; align-items: center; gap: 8px; flex-wrap: wrap; }
.plan-toolbar :deep(.el-select) { width: 240px; }
.todo-table { width: 100%; }
.task-title { font-weight: 650; }
.task-description { margin-top: 4px; color: var(--el-text-color-secondary); font-size: 12px; white-space: pre-wrap; }
.task-meta { display: flex; gap: 10px; margin-top: 5px; color: var(--el-text-color-secondary); font-size: 12px; }
.blocked { color: var(--el-color-danger); }
.full-width { width: 100%; }
</style>
