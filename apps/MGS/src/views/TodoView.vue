<script setup lang="ts">
import { notify } from '@aqlife/ui-shared'
import { computed, onMounted, reactive, ref, watch } from 'vue'
import { ElButton, ElCol, ElDialog, ElForm, ElFormItem, ElInput, ElMessageBox, ElOption, ElRow, ElSelect, ElSegmented, ElSlider, ElSwitch, ElTable, ElTableColumn, ElTag } from 'element-plus'
import { Plus, Edit, Refresh, FolderAdd } from '@element-plus/icons-vue'
import { TodoApi, TodoStatus, type TodoDto } from '@/api'
import { apiConfiguration } from '@/services/api'
import { useTodoStorage } from '@/stores/useTodoStore'
import MgsPageHeader from '../components/ui/MgsPageHeader.vue'
import MgsToolbar from '@/components/ui/MgsToolbar.vue'
import MgsPageContainer from '@/components/ui/MgsPageContainer.vue'
import { mgsIconRegistry,MgsIconName } from '@aqlife/icons'

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
const priorityRange = ref<[number, number]>([1, 99])
const showArchived = ref(false)
const task = reactive({ title: '', description: '', priority: 1, status: TodoStatus.Initial as number, parentId: null as string | null })
const plan = reactive({ name: '', description: '' })
const statuses = [
  { label: '全部状态', value: 'all' }, { label: '初始', value: TodoStatus.Initial },
  { label: '等待分配', value: TodoStatus.Wait }, { label: '进行中', value: TodoStatus.Execute },
  { label: '已完成', value: TodoStatus.Completed },
]

function statusValue(status?: string | null): number {
  if (status == null || status === '') return 0
  const n = Number(status)
  if (Number.isInteger(n) && n >= 0 && n <= 3) return n
  const s = status.toLowerCase()
  if (s.includes('wait')) return 1
  if (s.includes('execut') || s.includes('progress')) return 2
  if (s.includes('complet') || s.includes('done')) return 3
  return 0
}
function titleOf(todo: TodoDto) { return todo.title || todo.desc || '（未命名任务）' }
function statusLabel(status?: string | null) { return ['初始', '等待分配', '进行中', '已完成'][statusValue(status)] }
function statusType(status?: string | null): 'success' | 'warning' | 'info' | undefined {
  return ({ 1: 'info', 2: 'warning', 3: 'success' } as Record<number, 'success' | 'warning' | 'info'>)[statusValue(status)]
}
function flatten(items: TodoDto[], depth = 0): Array<{ todo: TodoDto; depth: number }> {
  return items.flatMap(todo => [{ todo, depth }, ...flatten(todo.todoList ?? [], depth + 1)])
}
const parentOptions = computed(() => flatten(store.todoList).filter(x => x.todo.uid && x.todo.uid !== editing.value?.uid && !x.todo.isArchived))
const visibleTodos = computed(() => {
  const filter = (items: TodoDto[]): TodoDto[] => items.reduce<TodoDto[]>((result, todo) => {
    const children = filter(todo.todoList ?? [])
    const priority = todo.priority ?? 1
    const matches = (statusFilter.value === 'all' || statusValue(todo.status) === statusFilter.value)
      && priority >= priorityRange.value[0] && priority <= priorityRange.value[1]
    if (matches || children.length) result.push({ ...todo, todoList: children, hasChildren: children.length > 0 })
    return result
  }, [])
  return filter(store.todoList)
})
async function refresh() {
  try { await store.fetchPlans() }
  catch (e) { notify.error(e instanceof Error ? e.message : '开发计划加载失败') }
}
onMounted(refresh)
watch(showArchived, async value => {
  store.includeArchived = value
  try { await store.fetchTodos() }
  catch (e) { notify.error(e instanceof Error ? e.message : '任务加载失败') }
})
function createTask(parent?: TodoDto) {
  editing.value = null
  Object.assign(task, { title: '', description: '', priority: 1, status: TodoStatus.Initial, parentId: parent?.uid ?? null })
  taskDialog.value = true
}
function editTask(todo: TodoDto) {
  editing.value = todo
  Object.assign(task, { title: titleOf(todo), description: todo.description ?? '', priority: todo.priority ?? 1, status: statusValue(todo.status), parentId: todo.ftid ?? null })
  taskDialog.value = true
}
async function saveTask() {
  if (!task.title.trim()) { notify.warning('请填写任务标题'); return }
  if (!store.selectedPlanId) { notify.warning('请先选择开发计划'); return }
  if (!Number.isInteger(task.priority) || task.priority < 1 || task.priority > 99) { notify.warning('优先级必须为 1 到 99'); return }
  saving.value = true
  try {
    if (editing.value?.uid) {
      await api.apiTodoPatch({ updateTodoCommand: {
        uid: editing.value.uid, title: task.title.trim(), description: task.description,
        status: task.status as TodoStatus, priority: task.priority,
      } })
      if (task.parentId !== (editing.value.ftid ?? null))
        await api.apiTodoParentPatch({ setTodoParentCommand: { uid: editing.value.uid, parentId: task.parentId } })
    } else {
      await api.apiTodoPost({ createTodoCommand: {
        planId: store.selectedPlanId, title: task.title.trim(), description: task.description,
        priority: task.priority, parentId: task.parentId, sortOrder: 0,
      } })
    }
    taskDialog.value = false
    await store.fetchTodos()
    notify.success('任务已保存')
  } catch (e) {
    notify.error(e instanceof Error ? e.message : '保存任务失败')
    await store.fetchTodos().catch(() => undefined)
  } finally { saving.value = false }
}
async function archiveTask(todo: TodoDto) {
  if (!todo.uid) return
  try {
    await ElMessageBox.confirm('归档后任务仍保留历史记录，确定继续？', '归档任务', { type: 'warning' })
    await api.apiTodoUidArchivePost({ uid: todo.uid })
    await store.fetchTodos()
    notify.success('任务已归档')
  } catch (e) {
    if (e !== 'cancel' && e !== 'close') notify.error(e instanceof Error ? e.message : '归档失败')
  }
}
async function restoreTask(todo: TodoDto) {
  if (!todo.uid) return
  try { await api.apiTodoUidRestorePost({ uid: todo.uid }); await store.fetchTodos() }
  catch (e) { notify.error(e instanceof Error ? e.message : '恢复失败') }
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
    else await store.createPlan(plan.name.trim(), plan.description)
    planDialog.value = false
    notify.success('开发计划已保存')
  } catch (e) { notify.error(e instanceof Error ? e.message : '保存计划失败') }
  finally { saving.value = false }
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
    <MgsPageHeader title="开发计划" description="管理开发任务、层级归属与执行状态。">
      <template #actions>
        <ElButton :icon="Refresh" @click="refresh">刷新</ElButton>
        <ElButton :icon="FolderAdd" @click="createPlan">新建计划</ElButton>
        <ElButton type="primary" :icon="Plus" :disabled="!store.selectedPlan || store.selectedPlan.isArchived" @click="createTask()">添加任务</ElButton>
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
      <div class="priority-filter"><span>优先级</span><ElSlider v-model="priorityRange" range :min="1" :max="99" /><span>{{ priorityRange[0] }}–{{ priorityRange[1] }}</span></div>
      <ElSwitch v-model="showArchived" active-text="显示归档任务" />
    </MgsToolbar>
    <MgsPageContainer>
    <ElTable :data="visibleTodos" v-loading="store.loadingPlans || store.loadingTodos" row-key="uid" class="todo-table" empty-text="此开发计划还没有任务" :tree-props="{ children: 'todoList', hasChildren: 'hasChildren' }">
      <ElTableColumn label="任务" min-width="260">
        <template #default="{ row }">
          <div class="task-title">{{ titleOf(row) }}</div>
          <div v-if="row.description" class="task-description">{{ row.description }}</div>
          <div class="task-meta">
            <span v-if="row.isBlocked" class="blocked">阻塞中 · {{ row.incompleteDependencyCount }} 项前置未完成</span>
            <span v-if="row.ftid">子任务</span><span v-if="row.isArchived">已归档</span>
          </div>
        </template>
      </ElTableColumn>
      <ElTableColumn label="状态" width="110"><template #default="{ row }"><ElTag :type="statusType(row.status)">{{ statusLabel(row.status) }}</ElTag></template></ElTableColumn>
      <ElTableColumn prop="priority" label="优先级" width="90" />
      <ElTableColumn prop="sortOrder" label="排序" width="80" />
      <ElTableColumn label="操作" min-width="230" fixed="right">
        <template #default="{ row }">
          <ElButton link type="primary" :icon="Plus" :disabled="row.isArchived || store.selectedPlan?.isArchived" @click="createTask(row)">添加子项</ElButton>
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
        <ElRow :gutter="16">
          <ElCol :span="12"><ElFormItem label="优先级 1–99"><ElInput v-model.number="task.priority" /></ElFormItem></ElCol>
          <ElCol v-if="editing" :span="12"><ElFormItem label="状态"><ElSelect v-model="task.status" class="full-width">
            <ElOption label="初始" :value="TodoStatus.Initial" /><ElOption label="等待分配" :value="TodoStatus.Wait" />
            <ElOption label="进行中" :value="TodoStatus.Execute" /><ElOption label="已完成" :value="TodoStatus.Completed" />
          </ElSelect></ElFormItem></ElCol>
        </ElRow>
        <ElFormItem label="父任务（仅表示归属，不代表前置依赖）">
          <ElSelect v-model="task.parentId" clearable filterable class="full-width" placeholder="根任务">
            <ElOption v-for="option in parentOptions" :key="option.todo.uid" :label="'　'.repeat(option.depth) + titleOf(option.todo)" :value="option.todo.uid!" />
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
.priority-filter { display: flex; align-items: center; gap: 10px; min-width: 240px; flex: 1; }
.priority-filter > span:first-child { white-space: nowrap; color: var(--el-text-color-regular); }
.todo-table { width: 100%; }
.task-title { font-weight: 650; }
.task-description { margin-top: 4px; color: var(--el-text-color-secondary); font-size: 12px; white-space: pre-wrap; }
.task-meta { display: flex; gap: 10px; margin-top: 5px; color: var(--el-text-color-secondary); font-size: 12px; }
.blocked { color: var(--el-color-danger); }
.full-width { width: 100%; }
@media (max-width: 760px) { .priority-filter { min-width: 100%; } }
</style>
