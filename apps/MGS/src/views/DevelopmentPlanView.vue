<script setup lang="ts">
import { computed, onBeforeUnmount, onMounted, reactive, ref } from 'vue'
import {
  ElButton, ElDialog, ElMessage, ElOption, ElSelect,
} from 'element-plus'
import { Plus, Refresh, Lock } from '@element-plus/icons-vue'
import MgsPageHeader from '@/components/ui/MgsPageHeader.vue'
import MgsToolbar from '@/components/ui/MgsToolbar.vue'
import MgsPageContainer from '@/components/ui/MgsPageContainer.vue'
import MgsPageState from '@/components/ui/MgsPageState.vue'

import { TodoApi, TodoStatus, type TodoDependencyDto, type TodoDto, type TodoRoadmapDto } from '@/api'
import { apiConfiguration } from '@/services/api'
import { useTodoStorage } from '@/stores/useTodoStore'

const store = useTodoStorage()
const todoApi = new TodoApi(apiConfiguration)
const plans = computed(() => store.plans)
const selectedPlanId = computed(() => store.selectedPlanId)
const roadmap = ref<TodoRoadmapDto | null>(null)
const loading = ref(true)
const loadError = ref('')
const saving = ref(false)
const selectedId = ref('')
const dependencyDialog = ref(false)
const canvas = ref<HTMLElement | null>(null)
const drag = ref<{ id: string; offsetX: number; offsetY: number; moved: boolean } | null>(null)
const localPositions = ref<Record<string, { x: number; y: number }>>({})
const dependencyForm = reactive({ predecessorId: '' })

const nodes = computed(() => roadmap.value?.nodes ?? [])
const dependencies = computed(() => roadmap.value?.dependencies ?? [])
const selected = computed(() => nodes.value.find(x => x.uid === selectedId.value) ?? null)
const selectedPlan = computed(() => store.selectedPlan)
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
const incoming = computed(() => dependencies.value.filter(x => x.successorId === selectedId.value))
const incomingIds = computed(() => new Set(incoming.value.map(x => x.predecessorId)))
const availablePredecessors = computed(() => nodes.value.filter(x => x.uid !== selectedId.value && x.uid && !incomingIds.value.has(x.uid)))

function titleOf(todo: Partial<TodoDto>) { return todo.title || '未命名任务' }
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
  await store.fetchPlans()
  if (selectedPlanId.value) await loadRoadmap()
  else roadmap.value = null
}
async function changePlan(planId: string) {
  await store.selectPlan(planId)
  await loadRoadmap()
}
async function refreshViews() {
  await Promise.all([loadRoadmap(), store.fetchTodos()])
}
async function loadRoadmap() {
  if (!selectedPlanId.value) { roadmap.value = null; loadError.value = ''; return }
  loading.value = true
  loadError.value = ''
  try {
    roadmap.value = await todoApi.apiTodoRoadmapGet({ planId: selectedPlanId.value })
    localPositions.value = {}
    if (!nodes.value.some(x => x.uid === selectedId.value)) selectedId.value = nodes.value[0]?.uid ?? ''
  } catch (error) {
    loadError.value = error instanceof Error ? error.message : '路线图加载失败'
  } finally { loading.value = false }
}
async function refresh() {
  loading.value = true
  try { await loadPlans() }
  catch (error) { ElMessage.error(error instanceof Error ? error.message : '开发计划加载失败') }
  finally { loading.value = false }
}
onMounted(refresh)
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

async function removeDependency(edge: TodoDependencyDto) {
  if (!edge.uid || !selectedPlanId.value) return
  saving.value = true
  try {
    await todoApi.apiTodoDependenciesDelete({ removeTodoDependencyCommand: {
      uid: edge.uid,
      planId: selectedPlanId.value,
    } })
    await refreshViews()
    ElMessage.success('前置依赖已移除')
  } catch (error) { ElMessage.error(error instanceof Error ? error.message : '移除依赖失败') }
  finally { saving.value = false }
}
function beginDrag(event: PointerEvent, todo: TodoDto) {
  if (!todo.uid || (event.target as HTMLElement).closest('button')) return
  const point = positions.value[todo.uid]
  const rect = canvas.value?.getBoundingClientRect()
  if (!rect) return
  const pointerX = event.clientX - rect.left + canvas.value!.scrollLeft
  const pointerY = event.clientY - rect.top + canvas.value!.scrollTop
  drag.value = { id: todo.uid, offsetX: pointerX - point.x, offsetY: pointerY - point.y, moved: false }
  event.preventDefault()
  ;(event.currentTarget as HTMLElement).setPointerCapture(event.pointerId)
}
function moveDrag(event: PointerEvent) {
  if (!drag.value || !canvas.value) return
  const rect = canvas.value.getBoundingClientRect()
  drag.value.moved = true
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
  if (!current?.moved || !localPositions.value[current.id]) return
  const point = localPositions.value[current.id]
  try {
    await todoApi.apiTodoRoadmapLayoutPut({ saveRoadmapNodeLayoutCommand: {
      planId: selectedPlanId.value, todoId: current.id, x: point.x, y: point.y,
    } })
    ElMessage.success('节点布局已保存')
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
    <MgsPageHeader
      title="开发路线图"
      description="选中节点后可设置前置依赖；拖动节点并松开即可保存手动布局。"
    >
      <template #actions>
        <ElButton type="primary" :icon="Plus" :disabled="!selected || !!selectedPlan?.isArchived || !!selected.isArchived" @click="dependencyDialog = true">为选中节点添加前置依赖</ElButton>
        <ElButton :icon="Refresh" @click="refresh">刷新</ElButton>
      </template>
    </MgsPageHeader>
    <MgsToolbar>
      <ElSelect :model-value="selectedPlanId" placeholder="选择开发计划" class="plan-select" @change="changePlan">
        <ElOption v-for="plan in plans" :key="plan.uid" :label="plan.name ?? '未命名计划'" :value="plan.uid!" />
      </ElSelect>
    </MgsToolbar>
    <MgsPageContainer>
    <MgsPageState v-if="loading && !roadmap" state="loading" description="正在加载路线图..." />
    <MgsPageState v-else-if="loadError" state="error" :description="loadError" retryable @retry="refresh" />
    <template v-else>
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
          <div class="node-top"><span>{{ node.isBlocked ? '等待前置' : statusLabel(node) }}</span><span>排序 {{ node.sortOrder ?? 0 }}</span></div>
          <h3>{{ titleOf(node) }}</h3>
          <p>{{ node.description || '暂无任务说明' }}</p>
          <div class="node-bottom"><span>{{ dependencies.filter(edge => edge.successorId === node.uid).length }} 项前置</span><span v-if="node.incompleteDependencyCount">{{ node.incompleteDependencyCount }} 项未完成</span></div>
        </article>
        <div v-if="!nodes.length" class="empty-canvas"><MgsPageState state="empty" description="当前计划还没有任务节点，请先在 Todo 管理页创建任务。" compact /></div>
        </div>
      </section>
      <aside class="detail-panel" v-if="selected">
        <div class="detail-kicker">SELECTED TASK</div>
        <h2>{{ titleOf(selected) }}</h2>
        <p class="detail-description">{{ selected.description || '暂无任务说明' }}</p>
        <div class="detail-state" :class="statusClass(selected)">{{ selected.isBlocked ? '被前置任务阻塞' : statusLabel(selected) }}</div>
        <div class="detail-meta"><span>排序</span><strong>{{ selected.sortOrder ?? 0 }}</strong></div>
        <div class="detail-meta"><span>未完成前置</span><strong>{{ selected.incompleteDependencyCount ?? 0 }}</strong></div>
        <div class="dependency-section">
          <div class="section-heading"><strong>前置依赖（{{ incoming.length }}）</strong><ElButton link type="primary" :icon="Plus" @click="dependencyDialog = true" :disabled="!!selectedPlan?.isArchived || !!selected.isArchived">添加前置任务</ElButton></div>
          <div v-if="incoming.length === 0" class="muted">没有前置依赖</div>
          <div v-for="edge in incoming" :key="edge.uid" class="dependency-item">
            <span>{{ titleOf(nodeById.get(edge.predecessorId ?? '') ?? {}) }}</span>
            <ElButton
              link
              type="danger"
              :disabled="!!selectedPlan?.isArchived || !!selected.isArchived || saving"
              @click="removeDependency(edge)"
            >移除</ElButton>
          </div>
          <p class="hint">只有全部前置任务完成后，该节点才会解除阻塞。移除前置依赖后，阻塞状态会根据剩余依赖重新计算。</p>
        </div>
      </aside>
      <aside class="detail-panel empty-detail" v-else><Lock /><p>选择一个任务节点查看详情</p></aside>
    </main>
    </template>
    </MgsPageContainer>
    <ElDialog v-model="dependencyDialog" title="添加前置依赖" width="min(500px, 94vw)">
      <p>当前选中节点「{{ titleOf(selected ?? {}) }}」将等待所选前置任务完成后才解除阻塞。父子层级不会自动产生依赖。</p>
      <ElSelect v-model="dependencyForm.predecessorId" filterable class="full-width" placeholder="选择必须先完成的任务">
        <ElOption v-for="node in availablePredecessors" :key="node.uid" :label="titleOf(node)" :value="node.uid!" />
      </ElSelect>
      <template #footer><ElButton @click="dependencyDialog = false">取消</ElButton><ElButton type="primary" :loading="saving" :disabled="!dependencyForm.predecessorId" @click="addDependency">确认添加前置依赖</ElButton></template>
    </ElDialog>
  </div>
</template>

<style scoped>
.roadmap-page { min-height: 100%; color: var(--el-text-color-primary); }
.detail-kicker { color: var(--el-text-color-secondary); font-size: 10px; letter-spacing: 2px; }
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
.dependency-section { border-top: 1px solid var(--el-border-color); padding-top: 12px; }
.section-heading { display: flex; align-items: center; justify-content: space-between; }
.dependency-item { display: flex; justify-content: space-between; align-items: center; gap: 8px; padding: 7px 0; font-size: 12px; }
.muted,.hint { color: var(--el-text-color-secondary); font-size: 12px; }
.hint { line-height: 1.6; }
.empty-detail { display: flex; flex-direction: column; align-items: center; justify-content: center; color: var(--el-text-color-secondary); }
.full-width { width: 100%; }
@media (max-width: 1050px) { .roadmap-workspace { grid-template-columns: minmax(0,1fr); }.detail-panel { border-left: 0; border-top: 1px solid var(--el-border-color); }.stats-row { grid-template-columns: repeat(2,minmax(120px,1fr)); } }
@media (max-width: 650px) { .plan-select { width: 100%; }.stats-row { grid-template-columns: repeat(2,minmax(0,1fr)); } }
</style>
