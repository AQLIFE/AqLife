<script setup lang="ts">
import { computed, defineComponent, h, onBeforeUnmount, onMounted, reactive, ref } from 'vue'
import {
  ElButton, ElDialog, ElForm, ElFormItem, ElInput, ElMessage, ElMessageBox,
  ElOption, ElSelect,
} from 'element-plus'
import { Plus, Refresh, Edit, Check, Lock } from '@element-plus/icons-vue'

// Element Plus Icons Vue does not provide Archive or Unlink in the installed version.
// Keep these as lightweight Vue SVG components so they remain compatible with ElButton's icon prop.
const Archive = defineComponent({
  name: 'ArchiveIcon',
  render: () => h('svg', {
    viewBox: '0 0 24 24', fill: 'none', stroke: 'currentColor',
    'stroke-width': 2, 'stroke-linecap': 'round', 'stroke-linejoin': 'round',
    'aria-hidden': 'true',
  }, [
    h('path', { d: 'M3 4h18v4H3z' }),
    h('path', { d: 'M5 8v12h14V8' }),
    h('path', { d: 'M10 12h4' }),
  ]),
})

const Unlink = defineComponent({
  name: 'UnlinkIcon',
  render: () => h('svg', {
    viewBox: '0 0 24 24', fill: 'none', stroke: 'currentColor',
    'stroke-width': 2, 'stroke-linecap': 'round', 'stroke-linejoin': 'round',
    'aria-hidden': 'true',
  }, [
    h('path', { d: 'M10 13a5 5 0 0 0 7.07 0l3-3a5 5 0 0 0-7.07-7.07l-1.72 1.71' }),
    h('path', { d: 'M14 11a5 5 0 0 0-7.07 0l-3 3A5 5 0 0 0 11 21.07l1.71-1.71' }),
    h('path', { d: 'm3 3 18 18' }),
  ]),
})
import {
  TodoApi, TodoStatus,
  type TodoDto, type TodoRoadmapDto,
} from '@/api'
import { apiConfiguration } from '@/services/api'
import { useTodoStore } from '@/stores/todoStore'


const DEFAULT_PLAN_ID = '00000000-0000-0000-0000-000000000001'
const todoStore = useTodoStore()
const todoApi = new TodoApi(apiConfiguration)
const plans = computed(() => todoStore.plans)
const selectedPlanId = computed(() => todoStore.selectedPlanId)
const roadmap = ref<TodoRoadmapDto | null>(null)
const loading = ref(false)
const saving = ref(false)
const selectedId = ref('')
const taskDialog = ref(false)
const planDialog = ref(false)
const dependencyDialog = ref(false)
const editing = ref<TodoDto | null>(null)
const editingPlan = ref(false)
const canvas = ref<HTMLElement | null>(null)
const drag = ref<{ id: string; offsetX: number; offsetY: number } | null>(null)
const localPositions = ref<Record<string, { x: number; y: number }>>({})
const taskForm = reactive({ title: '', description: '', priority: 1, status: TodoStatus.Initial as number, parentId: null as string | null })
const planForm = reactive({ name: '', description: '' })
const dependencyForm = reactive({ predecessorId: '' })

const nodes = computed(() => roadmap.value?.nodes ?? [])
const dependencies = computed(() => roadmap.value?.dependencies ?? [])
const selected = computed(() => nodes.value.find(x => x.uid === selectedId.value) ?? null)
const selectedPlan = computed(() => todoStore.selectedPlan)
const completedCount = computed(() => nodes.value.filter(x => Number(x.status) === TodoStatus.Completed || x.status === 'Completed').length)
const progress = computed(() => nodes.value.length ? Math.round(completedCount.value / nodes.value.length * 100) : 0)
const nodeById = computed(() => new Map(nodes.value.filter(x => x.uid).map(x => [x.uid!, x])))
const positions = computed(() => {
  const result: Record<string, { x: number; y: number }> = {}
  const layouts = new Map((roadmap.value?.layouts ?? []).map(x => [x.todoId!, { x: x.x ?? 0, y: x.y ?? 0 }]))
  nodes.value.forEach((node, index) => {
    if (!node.uid) return
    result[node.uid] = localPositions.value[node.uid] ?? layouts.get(node.uid) ?? {
      x: 48 + (index % 4) * 290,
      y: 80 + Math.floor(index / 4) * 170,
    }
  })
  return result
})
const canvasWidth = computed(() => Math.max(1240, ...Object.values(positions.value).map(p => p.x + 260)))
const canvasHeight = computed(() => Math.max(680, ...Object.values(positions.value).map(p => p.y + 150)))
const edges = computed(() => dependencies.value.flatMap(edge => {
  const from = positions.value[edge.predecessorId ?? '']
  const to = positions.value[edge.successorId ?? '']
  if (!from || !to) return []
  const blocked = nodeById.value.get(edge.successorId ?? '')?.isBlocked
  const x1 = from.x + 230, y1 = from.y + 58, x2 = to.x, y2 = to.y + 58
  return [{ id: edge.predecessorId + ':' + edge.successorId, d: `M ${x1} ${y1} C ${x1 + 70} ${y1}, ${x2 - 70} ${y2}, ${x2} ${y2}`, blocked }]
}))
const parentOptions = computed(() => nodes.value.filter(x => x.uid && x.uid !== editing.value?.uid && !x.isArchived))
const incoming = computed(() => dependencies.value.filter(x => x.successorId === selectedId.value))
const incomingIds = computed(() => new Set(incoming.value.map(x => x.predecessorId)))
const availablePredecessors = computed(() => nodes.value.filter(x => x.uid !== selectedId.value && x.uid && !incomingIds.value.has(x.uid)))

function titleOf(todo: Partial<TodoDto>) { return todo.title || todo.desc || '未命名任务' }
function statusValue(todo: TodoDto) {
  const value = todo.status
  if (value === 'Completed' || Number(value) === 3) return 3
  if (value === 'Execute' || Number(value) === 2) return 2
  if (value === 'Wait' || Number(value) === 1) return 1
  return 0
}
function statusLabel(todo: TodoDto) { return ['初始', '等待分配', '进行中', '已完成'][statusValue(todo)] }
function statusClass(todo: TodoDto) {
  if (statusValue(todo) === 3) return 'done'
  if (todo.isBlocked) return 'blocked'
  if (statusValue(todo) === 2) return 'active'
  return 'pending'
}
async function loadPlans() {
  await todoStore.fetchPlans()
  if (selectedPlanId.value) await loadRoadmap()
  else roadmap.value = null
}
async function changePlan(planId: string) {
  await todoStore.selectPlan(planId)
  await loadRoadmap()
}
async function refreshViews() {
  await Promise.all([loadRoadmap(), todoStore.refresh()])
}
async function loadRoadmap() {
  if (!selectedPlanId.value) { roadmap.value = null; return }
  loading.value = true
  try {
    roadmap.value = await todoApi.apiTodoRoadmapGet({ planId: selectedPlanId.value })
    localPositions.value = {}
    if (!nodes.value.some(x => x.uid === selectedId.value)) selectedId.value = nodes.value[0]?.uid ?? ''
  } catch (error) {
    ElMessage.error(error instanceof Error ? error.message : '科技树加载失败')
  } finally { loading.value = false }
}
async function refresh() {
  loading.value = true
  try { await loadPlans() }
  catch (error) { ElMessage.error(error instanceof Error ? error.message : '开发计划加载失败') }
  finally { loading.value = false }
}
onMounted(refresh)
function openCreate() {
  editing.value = null
  Object.assign(taskForm, { title: '', description: '', priority: 1, status: TodoStatus.Initial, parentId: null })
  taskDialog.value = true
}
function openEdit(todo: TodoDto) {
  editing.value = todo
  Object.assign(taskForm, { title: titleOf(todo), description: todo.description ?? '', priority: todo.priority ?? 1, status: statusValue(todo), parentId: todo.ftid ?? null })
  taskDialog.value = true
}
async function saveTask() {
  if (!taskForm.title.trim()) { ElMessage.warning('任务标题不能为空'); return }
  if (!Number.isInteger(taskForm.priority) || taskForm.priority < 1 || taskForm.priority > 99) { ElMessage.warning('优先级范围为 1–99'); return }
  saving.value = true
  try {
    if (editing.value?.uid) {
      await todoApi.apiTodoPatch({ updateTodoCommand: {
        uid: editing.value.uid, title: taskForm.title.trim(), description: taskForm.description,
        status: taskForm.status as TodoStatus, priority: taskForm.priority,
      } })
      if (taskForm.parentId !== (editing.value.ftid ?? null))
        await todoApi.apiTodoParentPatch({ setTodoParentCommand: { uid: editing.value.uid, parentId: taskForm.parentId } })
    } else {
      await todoApi.apiTodoPost({ createTodoCommand: {
        planId: selectedPlanId.value, title: taskForm.title.trim(), description: taskForm.description,
        priority: taskForm.priority, parentId: taskForm.parentId, sortOrder: nodes.value.length,
      } })
    }
    taskDialog.value = false
    await refreshViews()
    ElMessage.success('任务已保存')
  } catch (error) {
    ElMessage.error(error instanceof Error ? error.message : '保存任务失败')
    await refreshViews()
  } finally { saving.value = false }
}
async function setStatus(todo: TodoDto, status: number) {
  if (!todo.uid) return
  try {
    await todoApi.apiTodoPatch({ updateTodoCommand: {
      uid: todo.uid, title: titleOf(todo), description: todo.description ?? '',
      status: status as TodoStatus, priority: todo.priority ?? 1,
    } })
    await refreshViews()
  } catch (error) { ElMessage.error(error instanceof Error ? error.message : '更新状态失败') }
}
async function archiveTask(todo: TodoDto) {
  if (!todo.uid) return
  try {
    await ElMessageBox.confirm('归档后任务将从当前科技树隐藏，但历史记录会保留。', '归档任务', { type: 'warning' })
    await todoApi.apiTodoUidArchivePost({ uid: todo.uid })
    await refreshViews()
  } catch (error) {
    if (error !== 'cancel' && error !== 'close') ElMessage.error(error instanceof Error ? error.message : '归档失败')
  }
}
async function addDependency() {
  if (!selectedId.value || !dependencyForm.predecessorId) return
  saving.value = true
  try {
    await todoApi.apiTodoDependenciesPost({ addTodoDependencyCommand: {
      planId: selectedPlanId.value, predecessorId: dependencyForm.predecessorId, successorId: selectedId.value,
    } })
    dependencyDialog.value = false
    dependencyForm.predecessorId = ''
    await refreshViews()
    ElMessage.success('前置依赖已添加')
  } catch (error) { ElMessage.error(error instanceof Error ? error.message : '添加依赖失败') }
  finally { saving.value = false }
}
async function removeDependency(predecessorId?: string, successorId?: string) {
  if (!predecessorId || !successorId) return
  try {
    await todoApi.apiTodoDependenciesDelete({ removeTodoDependencyCommand: {
      planId: selectedPlanId.value, predecessorId, successorId,
    } })
    await refreshViews()
  } catch (error) { ElMessage.error(error instanceof Error ? error.message : '移除依赖失败') }
}
function openPlanDialog(edit = false) {
  editingPlan.value = edit
  Object.assign(planForm, { name: edit ? selectedPlan.value?.name ?? '' : '', description: edit ? selectedPlan.value?.description ?? '' : '' })
  planDialog.value = true
}
async function savePlan() {
  if (!planForm.name.trim()) { ElMessage.warning('计划名称不能为空'); return }
  saving.value = true
  try {
    const currentPlan = selectedPlan.value
    if (editingPlan.value && currentPlan?.uid)
      await todoStore.updatePlan({ ...currentPlan, name: planForm.name.trim(), description: planForm.description })
    else
      await todoStore.createPlan(planForm.name.trim(), planForm.description)
    planDialog.value = false
    await loadRoadmap()
    ElMessage.success('开发计划已保存')
  } catch (error) { ElMessage.error(error instanceof Error ? error.message : '保存计划失败') }
  finally { saving.value = false }
}
async function archivePlan() {
  const currentPlan = selectedPlan.value
  if (!currentPlan?.uid) return
  try {
    await ElMessageBox.confirm('归档前必须先归档该计划中的所有任务。', '归档开发计划', { type: 'warning' })
    await todoStore.archivePlan(currentPlan.uid)
    await loadRoadmap()
  } catch (error) {
    if (error !== 'cancel' && error !== 'close') ElMessage.error(error instanceof Error ? error.message : '归档计划失败')
  }
}
function beginDrag(event: PointerEvent, todo: TodoDto) {
  if (!todo.uid || (event.target as HTMLElement).closest('button')) return
  const point = positions.value[todo.uid]
  drag.value = { id: todo.uid, offsetX: event.clientX - point.x, offsetY: event.clientY - point.y }
  ;(event.currentTarget as HTMLElement).setPointerCapture(event.pointerId)
}
function moveDrag(event: PointerEvent) {
  if (!drag.value || !canvas.value) return
  const rect = canvas.value.getBoundingClientRect()
  localPositions.value = {
    ...localPositions.value,
    [drag.value.id]: {
      x: Math.max(16, event.clientX - rect.left + canvas.value.scrollLeft - drag.value.offsetX),
      y: Math.max(54, event.clientY - rect.top + canvas.value.scrollTop - drag.value.offsetY),
    },
  }
}
async function endDrag() {
  const current = drag.value
  drag.value = null
  if (!current || !localPositions.value[current.id]) return
  const point = localPositions.value[current.id]
  try {
    await todoApi.apiTodoRoadmapLayoutPut({ saveRoadmapNodeLayoutCommand: {
      planId: selectedPlanId.value, todoId: current.id, x: point.x, y: point.y,
    } })
  } catch (error) { ElMessage.error(error instanceof Error ? error.message : '节点位置保存失败') }
}
onMounted(() => {
  window.addEventListener('pointermove', moveDrag)
  window.addEventListener('pointerup', endDrag)
})
onBeforeUnmount(() => {
  window.removeEventListener('pointermove', moveDrag)
  window.removeEventListener('pointerup', endDrag)
})
</script>

<template>
  <div class="roadmap-page">
    <header class="roadmap-header">
      <div><div class="eyebrow">MONIREPO / DEVELOPMENT ROADMAP</div><h1>开发路线图</h1><p>任务是节点，依赖决定解锁；拖动节点即可保存布局。</p></div>
      <div class="header-actions">
        <ElSelect :model-value="selectedPlanId" placeholder="选择开发计划" class="plan-select" @change="changePlan">
          <ElOption v-for="plan in plans" :key="plan.uid" :label="plan.name ?? '未命名计划'" :value="plan.uid!" />
        </ElSelect>
        <ElButton :icon="Refresh" @click="refresh">刷新</ElButton>
        <ElButton :icon="Edit" :disabled="!selectedPlan || selectedPlan.isArchived" @click="openPlanDialog(true)">编辑计划</ElButton>
        <ElButton :icon="Archive" :disabled="!selectedPlan || selectedPlan.isArchived || selectedPlan.uid === DEFAULT_PLAN_ID" @click="archivePlan">归档</ElButton>
        <ElButton type="primary" :icon="Plus" :disabled="!selectedPlan || selectedPlan.isArchived" @click="openCreate">新建节点</ElButton>
        <ElButton @click="openPlanDialog(false)">新建计划</ElButton>
      </div>
    </header>
    <section class="stats-row">
      <div><span>当前计划</span><strong>{{ selectedPlan?.name ?? '未选择' }}</strong></div>
      <div><span>任务节点</span><strong>{{ nodes.length }}</strong></div>
      <div><span>已完成</span><strong>{{ completedCount }} / {{ nodes.length }}</strong></div>
      <div class="progress-stat"><span>完成进度</span><strong>{{ progress }}%</strong><div class="progress-track"><i :style="{ width: progress + '%' }" /></div></div>
    </section>
    <main class="roadmap-workspace" v-loading="loading">
      <section ref="canvas" class="canvas-viewport">
        <div class="roadmap-canvas" :style="{ width: canvasWidth + 'px', height: canvasHeight + 'px' }">
        <svg class="edge-layer" :width="canvasWidth" :height="canvasHeight">
          <defs><marker id="roadmap-arrow" markerWidth="8" markerHeight="8" refX="6" refY="3" orient="auto"><path d="M0,0 L0,6 L6,3 Z" fill="#80949a" /></marker></defs>
          <path v-for="edge in edges" :key="edge.id" :d="edge.d" :class="['dependency-edge', { blocked: edge.blocked }]" marker-end="url(#roadmap-arrow)" />
        </svg>
        <article v-for="node in nodes" :key="node.uid" class="roadmap-node"
          :class="[statusClass(node), { selected: selectedId === node.uid }]"
          :style="{ left: (positions[node.uid!]?.x ?? 0) + 'px', top: (positions[node.uid!]?.y ?? 0) + 'px' }"
          @pointerdown="beginDrag($event, node)" @click="selectedId = node.uid ?? ''">
          <div class="node-top"><span>{{ node.isBlocked ? '等待前置' : statusLabel(node) }}</span><span>优先级 {{ node.priority ?? 1 }}</span></div>
          <h3>{{ titleOf(node) }}</h3>
          <p>{{ node.description || '暂无任务说明' }}</p>
          <div class="node-bottom"><span v-if="node.ftid">子任务</span><span v-if="node.incompleteDependencyCount">{{ node.incompleteDependencyCount }} 项未解锁</span><span v-if="node.hasChildren">含子任务</span></div>
        </article>
        <div v-if="!nodes.length" class="empty-canvas">这个计划还没有任务节点，点击「新建节点」开始构建路线图。</div>
        </div>
      </section>
      <aside class="detail-panel" v-if="selected">
        <div class="detail-kicker">SELECTED TASK</div>
        <h2>{{ titleOf(selected) }}</h2>
        <p class="detail-description">{{ selected.description || '暂无任务说明' }}</p>
        <div class="detail-state" :class="statusClass(selected)">{{ selected.isBlocked ? '被前置任务阻塞' : statusLabel(selected) }}</div>
        <div class="detail-meta"><span>优先级</span><strong>{{ selected.priority ?? 1 }}</strong></div>
        <div class="detail-meta"><span>父任务</span><strong>{{ selected.ftid ? titleOf(nodeById.get(selected.ftid) ?? {}) : '无' }}</strong></div>
        <div class="detail-meta"><span>未完成前置</span><strong>{{ selected.incompleteDependencyCount ?? 0 }}</strong></div>
        <div class="detail-actions">
          <ElButton :icon="Edit" @click="openEdit(selected)" :disabled="!!selected.isArchived || !!selectedPlan?.isArchived">编辑</ElButton>
          <ElButton v-if="statusValue(selected) !== 3" :icon="Check" type="success" :disabled="!!selected.isBlocked || !!selectedPlan?.isArchived" @click="setStatus(selected, 3)">标记完成</ElButton>
          <ElButton v-else @click="setStatus(selected, 1)" :disabled="!!selectedPlan?.isArchived">重新打开</ElButton>
          <ElButton :icon="Archive" type="warning" plain @click="archiveTask(selected)" :disabled="!!selected.isArchived || !!selectedPlan?.isArchived">归档</ElButton>
        </div>
        <div class="dependency-section">
          <div class="section-heading"><strong>前置依赖</strong><ElButton link type="primary" :icon="Plus" @click="dependencyDialog = true" :disabled="!!selectedPlan?.isArchived || !!selected.isArchived">添加</ElButton></div>
          <div v-if="incoming.length === 0" class="muted">没有前置依赖</div>
          <div v-for="edge in incoming" :key="edge.predecessorId" class="dependency-item">
            <span>{{ titleOf(nodeById.get(edge.predecessorId ?? '') ?? {}) }}</span>
            <ElButton link type="danger" :icon="Unlink" @click="removeDependency(edge.predecessorId, edge.successorId)" />
          </div>
          <p class="hint">只有全部前置任务完成后，该节点才会解除阻塞。</p>
        </div>
      </aside>
      <aside class="detail-panel empty-detail" v-else><Lock /><p>选择一个任务节点查看详情</p></aside>
    </main>
    <ElDialog v-model="taskDialog" :title="editing ? '编辑任务节点' : '新建任务节点'" width="min(620px, 94vw)">
      <ElForm label-position="top">
        <ElFormItem label="标题" required><ElInput v-model="taskForm.title" maxlength="500" /></ElFormItem>
        <ElFormItem label="说明"><ElInput v-model="taskForm.description" type="textarea" :rows="4" maxlength="10000" /></ElFormItem>
        <ElFormItem label="父任务（归属关系）"><ElSelect v-model="taskForm.parentId" clearable filterable class="full-width">
          <ElOption v-for="parent in parentOptions" :key="parent.uid" :label="titleOf(parent)" :value="parent.uid!" />
        </ElSelect></ElFormItem>
        <ElFormItem label="优先级"><ElInput v-model.number="taskForm.priority" /></ElFormItem>
        <ElFormItem v-if="editing" label="状态"><ElSelect v-model="taskForm.status" class="full-width">
          <ElOption label="初始" :value="TodoStatus.Initial" /><ElOption label="等待分配" :value="TodoStatus.Wait" />
          <ElOption label="进行中" :value="TodoStatus.Execute" /><ElOption label="已完成" :value="TodoStatus.Completed" />
        </ElSelect></ElFormItem>
      </ElForm>
      <template #footer><ElButton @click="taskDialog = false">取消</ElButton><ElButton type="primary" :loading="saving" @click="saveTask">保存</ElButton></template>
    </ElDialog>
    <ElDialog v-model="dependencyDialog" title="添加前置依赖" width="min(500px, 94vw)">
      <p>选择一个必须先完成的任务。该关系不改变任务的父子归属。</p>
      <ElSelect v-model="dependencyForm.predecessorId" filterable class="full-width" placeholder="选择前置任务">
        <ElOption v-for="node in availablePredecessors" :key="node.uid" :label="titleOf(node)" :value="node.uid!" />
      </ElSelect>
      <template #footer><ElButton @click="dependencyDialog = false">取消</ElButton><ElButton type="primary" :loading="saving" @click="addDependency">添加依赖</ElButton></template>
    </ElDialog>
    <ElDialog v-model="planDialog" :title="editingPlan ? '编辑开发计划' : '新建开发计划'" width="min(560px, 94vw)">
      <ElForm label-position="top">
        <ElFormItem label="计划名称" required><ElInput v-model="planForm.name" maxlength="120" /></ElFormItem>
        <ElFormItem label="计划说明"><ElInput v-model="planForm.description" type="textarea" :rows="3" maxlength="2000" /></ElFormItem>
      </ElForm>
      <template #footer><ElButton @click="planDialog = false">取消</ElButton><ElButton type="primary" :loading="saving" @click="savePlan">保存</ElButton></template>
    </ElDialog>
  </div>
</template>

<style scoped>
.roadmap-page { min-height: 100%; color: var(--el-text-color-primary); }
.roadmap-header { display: flex; justify-content: space-between; align-items: center; gap: 18px; flex-wrap: wrap; margin-bottom: 18px; }
.eyebrow,.detail-kicker { color: var(--el-text-color-secondary); font-size: 10px; letter-spacing: 2px; }
.roadmap-header h1 { margin: 6px 0; font-size: 25px; }
.roadmap-header p { margin: 0; color: var(--el-text-color-secondary); font-size: 13px; }
.header-actions { display: flex; gap: 8px; flex-wrap: wrap; align-items: center; }
.plan-select { width: 220px; }
.stats-row { display: grid; grid-template-columns: repeat(4,minmax(120px,1fr)); gap: 10px; margin-bottom: 14px; }
.stats-row > div { padding: 12px 15px; background: var(--el-bg-color); border: 1px solid var(--el-border-color-light); border-radius: 8px; display: flex; flex-direction: column; gap: 5px; }
.stats-row span { color: var(--el-text-color-secondary); font-size: 12px; }
.stats-row strong { font-size: 18px; }
.progress-track { height: 4px; border-radius: 5px; background: var(--el-fill-color); overflow: hidden; }
.progress-track i { display: block; height: 100%; background: var(--el-color-success); }
.roadmap-workspace { display: grid; grid-template-columns: minmax(0,1fr) 300px; min-height: 680px; border: 1px solid var(--el-border-color); border-radius: 10px; overflow: hidden; background: var(--el-bg-color-page); }
.canvas-viewport { min-width: 0; min-height: 680px; overflow: auto; }
.roadmap-canvas { position: relative; overflow: visible; min-width: 0; min-height: 680px; background-image: linear-gradient(var(--el-border-color-lighter) 1px,transparent 1px),linear-gradient(90deg,var(--el-border-color-lighter) 1px,transparent 1px); background-size: 28px 28px; touch-action: pan-x pan-y; }
.edge-layer { position: absolute; inset: 0; pointer-events: none; overflow: visible; }
.dependency-edge { fill: none; stroke: var(--el-color-primary); stroke-width: 2.2; opacity: .8; }
.dependency-edge.blocked { stroke: var(--el-color-warning); stroke-dasharray: 6 5; }
.roadmap-node { position: absolute; width: 230px; min-height: 116px; padding: 12px; border: 1px solid var(--el-border-color); border-left: 4px solid var(--el-color-info); border-radius: 8px; background: var(--el-bg-color); box-shadow: 0 3px 12px #00000012; cursor: grab; user-select: none; z-index: 1; }
.roadmap-node:active { cursor: grabbing; }
.roadmap-node.selected { outline: 2px solid var(--el-color-primary); outline-offset: 2px; }
.roadmap-node.done { border-left-color: var(--el-color-success); }
.roadmap-node.active { border-left-color: var(--el-color-warning); }
.roadmap-node.blocked { border-left-color: var(--el-color-danger); }
.node-top,.node-bottom { display: flex; justify-content: space-between; gap: 8px; color: var(--el-text-color-secondary); font-size: 10px; }
.roadmap-node h3 { margin: 9px 0 5px; font-size: 14px; }
.roadmap-node p { margin: 0 0 8px; font-size: 11px; color: var(--el-text-color-secondary); overflow: hidden; display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; }
.empty-canvas { position: absolute; inset: 0; display: grid; place-items: center; color: var(--el-text-color-secondary); }
.detail-panel { padding: 20px 16px; background: var(--el-bg-color); border-left: 1px solid var(--el-border-color); }
.detail-panel h2 { font-size: 19px; margin: 9px 0; overflow-wrap: anywhere; }
.detail-description { color: var(--el-text-color-secondary); font-size: 13px; line-height: 1.65; white-space: pre-wrap; }
.detail-state { display: inline-block; margin: 8px 0 12px; padding: 4px 8px; border-radius: 5px; font-size: 12px; background: var(--el-fill-color); }
.detail-state.done { color: var(--el-color-success); }.detail-state.blocked { color: var(--el-color-danger); }.detail-state.active { color: var(--el-color-warning); }
.detail-meta { display: flex; justify-content: space-between; gap: 8px; padding: 8px 0; border-bottom: 1px solid var(--el-border-color-lighter); font-size: 12px; }
.detail-meta span { color: var(--el-text-color-secondary); }
.detail-actions { display: flex; flex-wrap: wrap; gap: 6px; margin: 14px 0; }
.dependency-section { border-top: 1px solid var(--el-border-color); padding-top: 12px; }
.section-heading { display: flex; align-items: center; justify-content: space-between; }
.dependency-item { display: flex; justify-content: space-between; align-items: center; gap: 8px; padding: 7px 0; font-size: 12px; }
.muted,.hint { color: var(--el-text-color-secondary); font-size: 12px; }
.hint { line-height: 1.6; }
.empty-detail { display: flex; flex-direction: column; align-items: center; justify-content: center; color: var(--el-text-color-secondary); }
.full-width { width: 100%; }
@media (max-width: 1050px) { .roadmap-workspace { grid-template-columns: minmax(0,1fr); }.detail-panel { border-left: 0; border-top: 1px solid var(--el-border-color); }.stats-row { grid-template-columns: repeat(2,minmax(120px,1fr)); } }
@media (max-width: 650px) { .roadmap-header { align-items: stretch; }.header-actions { width: 100%; }.plan-select { width: 100%; }.stats-row { grid-template-columns: repeat(2,minmax(0,1fr)); } }
</style>
