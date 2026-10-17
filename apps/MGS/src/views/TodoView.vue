<script setup lang="ts">
import { ElButton, ElCol, ElDialog, ElForm, ElFormItem, ElInput, ElMessage, ElMessageBox, ElOption, ElRow, ElSelect, ElTable, ElTableColumn, ElTag, ElText } from 'element-plus'
import { Plus, Edit, Delete, Refresh } from '@element-plus/icons-vue'
import { computed, h, onBeforeMount, ref } from 'vue'
import { TodoApi, TodoStatus, type CreateTodoCommand, type TodoDto, type UpdateTodoCommand } from '@/api'
import { apiConfiguration } from '@/services/api'
import { useTodoStorage } from '@/stores/useTodoStore'

const todoStore = useTodoStorage()
const todoApi = new TodoApi(apiConfiguration)
const proxyTodos = ref<TodoDto[]>([])
const isLoading = ref(true)
const dialogVisible = ref(false)
const isSaving = ref(false)
const editingTodo = ref<TodoDto | null>(null)
const form = ref<CreateTodoCommand & { status?: number }>({ desc: '', priority: 1, ftid: null, status: TodoStatus.Initial })
const isEditing = computed(() => editingTodo.value !== null)
const parentOptions = computed(() => todoStore.todoList.filter(todo => !todo.ftid && todo.uid !== editingTodo.value?.uid))

function syncTodos() { proxyTodos.value = [...todoStore.todoList] }
async function loadTodos(force = false) {
  isLoading.value = true
  try {
    if (force) todoStore.todoList.splice(0)
    await todoStore.fetchAllTodos(todoApi)
    syncTodos()
  } catch (error) { ElMessage.error(error instanceof Error ? error.message : '待办数据加载失败') }
  finally { isLoading.value = false }
}
onBeforeMount(() => loadTodos())
function openCreate(parent?: TodoDto) {
  editingTodo.value = null
  form.value = { desc: '', priority: 1, ftid: parent?.uid ?? null, status: TodoStatus.Initial }
  dialogVisible.value = true
}
function openEdit(todo: TodoDto) {
  editingTodo.value = todo
  form.value = { desc: todo.desc ?? '', priority: todo.priority ?? 1, ftid: todo.ftid ?? null, status: statusValue(todo.status) }
  dialogVisible.value = true
}
async function saveTodo() {
  if (!form.value.desc?.trim()) { ElMessage.warning('请填写待办内容'); return }
  if (!Number.isInteger(Number(form.value.priority)) || Number(form.value.priority) < 1 || Number(form.value.priority) > 99) { ElMessage.warning('优先级需要是 1 到 99 的整数'); return }
  isSaving.value = true
  try {
    if (editingTodo.value?.uid) {
      await todoApi.apiTodoPatch({ updateTodoCommand: { uid: editingTodo.value.uid, desc: form.value.desc.trim(), status: Number(form.value.status ?? TodoStatus.Initial) as UpdateTodoCommand['status'], priority: Number(form.value.priority) } })
      ElMessage.success('待办已更新')
    } else {
      await todoApi.apiTodoPost({ createTodoCommand: { desc: form.value.desc.trim(), priority: Number(form.value.priority), ftid: form.value.ftid ?? null } })
      ElMessage.success('待办已创建')
    }
    dialogVisible.value = false
    await loadTodos(true)
  } catch (error) { ElMessage.error(error instanceof Error ? error.message : '保存待办失败') }
  finally { isSaving.value = false }
}
async function deleteTodo(todo: TodoDto) {
  const childCount = todo.todoList?.length ?? 0
  try {
    const message = childCount
      ? h('div', { class: 'todo-delete-warning' }, [
        h('p', { class: 'todo-delete-warning__title' }, '⚠ 此待办包含子待办'),
        h('p', {}, `「${todo.desc || '未命名待办'}」下有 ${childCount} 个子待办。删除父待办可能导致子待办失去关联，请谨慎操作。`),
      ])
      : `确定删除待办「${todo.desc || '未命名待办'}」吗？此操作无法撤销。`
    await ElMessageBox.confirm(message, '删除待办确认', {
      type: 'warning',
      confirmButtonText: '确认删除',
      cancelButtonText: '取消',
      distinguishCancelAndClose: true,
    })
    await todoApi.apiTodoDelete({ deleteTodoCommand: { uid: todo.uid } })
    ElMessage.success('待办已删除')
    await loadTodos(true)
  } catch (error) {
    if (error === 'cancel' || error === 'close') return
    ElMessage.error(error instanceof Error ? error.message : '删除待办失败')
  }
}

function statusValue(status?: string | null): number {
  if (status == null || status === '') return TodoStatus.Initial
  const numeric = Number(status)
  if (Number.isInteger(numeric) && numeric >= 0 && numeric <= 3) return numeric
  const normalized = status.toLowerCase()
  if (normalized.includes('wait')) return TodoStatus.Wait
  if (normalized.includes('execut') || normalized.includes('progress')) return TodoStatus.Execute
  if (normalized.includes('complet') || normalized.includes('done')) return TodoStatus.Completed
  return TodoStatus.Initial
}
function statusLabel(status?: string | null) {
  return ({ 0: '初始', 1: '等待分配', 2: '进行中', 3: '已完成' } as Record<number, string>)[statusValue(status)]
}
function statusType(status?: string | null): "primary" | "success" | "warning" | "info" | "danger" | undefined {
  const value = statusValue(status)
  return value === 3 ? 'success' : value === 2 ? 'warning' : value === 1 ? 'info' : undefined
}
</script>

<template>
  <div class="todo-view-root">
    <ElRow class="todo-page" :gutter="16">
      <ElCol :span="24" class="toolbar">
        <div>
          <h2>待办事项</h2>
          <p>管理任务、优先级与子待办，随时更新进度。</p>
        </div>
        <div class="toolbar-actions">
          <ElButton :icon="Refresh" @click="loadTodos(true)">刷新</ElButton>
          <ElButton type="primary" :icon="Plus" @click="openCreate()">添加待办</ElButton>
        </div>
      </ElCol>
      <ElCol :span="24">
        <ElTable :data="proxyTodos" v-loading="isLoading" element-loading-text="正在加载待办..." empty-text="暂无待办，点击右上角添加第一项"
          row-key="uid" class="todo-table" :tree-props="{ children: 'todoList', hasChildren: 'hasChildren' }">
          <ElTableColumn prop="desc" label="待办内容" min-width="220">
            <template #default="{ row }">
              <div class="todo-title">{{ row.desc || '（未填写）' }}</div>
              <div v-if="row.ftid" class="todo-meta">子待办</div>
            </template>
          </ElTableColumn>
          <ElTableColumn prop="status" label="状态" width="110"><template #default="{ row }">
              <ElTag :type="statusType(row.status)">{{ statusLabel(row.status) }}</ElTag>
            </template>
          </ElTableColumn>
          <ElTableColumn prop="priority" label="优先级" width="100" />
          <ElTableColumn prop="ftid" label="父待办 UID" min-width="160"><template #default="{ row }">
              <ElText size="small">{{ row.ftid || '无' }}</ElText>
            </template></ElTableColumn>
          <ElTableColumn label="操作" width="240" fixed="right">
            <template #default="{ row }">
              <ElButton link type="primary" :icon="Plus" :disabled="!!row.ftid" @click="openCreate(row)">添加子项</ElButton>
              <ElButton link type="primary" :icon="Edit" @click="openEdit(row)">编辑</ElButton>
              <ElButton link type="danger" :icon="Delete" @click="deleteTodo(row)">删除</ElButton>
            </template>
          </ElTableColumn>
        </ElTable>
      </ElCol>
    </ElRow>
    <ElDialog v-model="dialogVisible" :title="isEditing ? '编辑待办' : (form.ftid ? '添加子待办' : '添加待办')"
      width="min(560px, 92vw)" destroy-on-close>
      <ElForm label-position="top">
        <ElFormItem label="待办内容" required>
          <ElInput v-model="form.desc" type="textarea" :rows="3" maxlength="500" show-word-limit
            placeholder="描述要完成的事情" />
        </ElFormItem>
        <ElRow :gutter="16">
          <ElCol :span="12">
            <ElFormItem label="优先级（1–99）" required>
              <ElInput v-model="form.priority" inputmode="numeric" />
            </ElFormItem>
          </ElCol>
          <ElCol v-if="isEditing" :span="12">
            <ElFormItem label="状态">
              <ElSelect v-model="form.status" class="full-width">
                <ElOption label="初始" :value="TodoStatus.Initial" />
                <ElOption label="等待分配" :value="TodoStatus.Wait" />
                <ElOption label="进行中" :value="TodoStatus.Execute" />
                <ElOption label="已完成" :value="TodoStatus.Completed" />
              </ElSelect>
            </ElFormItem>
          </ElCol>
        </ElRow>
        <ElFormItem v-if="!isEditing && !form.ftid" label="父待办（可选）">
          <ElSelect v-model="form.ftid" clearable placeholder="不选择则创建为根待办" class="full-width">
            <ElOption v-for="parent in parentOptions" :key="parent.uid" :label="parent.desc || parent.uid"
              :value="parent.uid!" />
          </ElSelect>
        </ElFormItem>
        <p v-if="form.ftid && !isEditing" class="parent-hint">此待办将作为所选父待办的子项创建。</p>
      </ElForm>
      <template #footer>
        <ElButton @click="dialogVisible = false">取消</ElButton>
        <ElButton type="primary" :loading="isSaving" @click="saveTodo">保存</ElButton>
      </template>
    </ElDialog>
  </div>
</template>

<style scoped>
.todo-page {
  height: 100%;
  align-content: flex-start;
}

.toolbar {
  display: flex;
  justify-content: space-between;
  align-items: center;
  gap: 16px;
  padding: 20px 8px;
}

.toolbar h2 {
  margin: 0;
  color: var(--mgs-text, var(--el-text-color-primary));
  font-size: 22px;
  font-weight: 700;
}

.toolbar p {
  margin: 8px 0 0;
  color: var(--mgs-muted, var(--el-text-color-secondary));
}

.toolbar-actions {
  display: flex;
  gap: 10px;
  flex-wrap: wrap;
}

.todo-table {
  width: 100%;
}

.todo-title {
  font-weight: 600;
}

.todo-meta {
  margin-top: 4px;
  color: var(--el-color-info);
  font-size: 12px;
}

.full-width {
  width: 100%;
}

.parent-hint {
  color: var(--el-color-info);
  margin: 0;
  font-size: 13px;
}

:global(.todo-delete-warning__title) {
  font-size: 16px;
  font-weight: 800;
  color: var(--el-color-danger);
  margin: 0 0 8px;
}

:global(.todo-delete-warning p:last-child) {
  line-height: 1.7;
  margin: 0;
}

@media (max-width:760px) {
  .toolbar {
    align-items: flex-start;
    flex-direction: column;
  }
}
</style>
