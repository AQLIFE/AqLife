<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { ElButton, ElOption, ElSelect, ElTag, ElMessage } from 'element-plus'
import { Refresh, Lock } from '@element-plus/icons-vue'
import { TodoApi, TodoStatus, type TodoDto, type TodoRoadmapDto } from '@/api'
import { apiConfiguration } from '@/services/api'
import { useTodoStore } from '@/stores/todoStore'

const store = useTodoStore()
const api = new TodoApi(apiConfiguration)
const roadmap = ref<TodoRoadmapDto | null>(null)
const loading = ref(false)
const selectedId = ref('')
const plans = computed(() => store.plans)
const selectedPlanId = computed(() => store.selectedPlanId)
const nodes = computed(() => roadmap.value?.nodes ?? [])
const dependencies = computed(() => roadmap.value?.dependencies ?? [])
const selected = computed(() => nodes.value.find(x => x.uid === selectedId.value) ?? null)
const nodeById = computed(() => new Map(nodes.value.filter(x => x.uid).map(x => [x.uid!, x])))
const completedCount = computed(() => nodes.value.filter(x => Number(x.status) === TodoStatus.Completed || x.status === 'Completed').length)
const progress = computed(() => nodes.value.length ? Math.round(completedCount.value / nodes.value.length * 100) : 0)
const positions = computed(() => {
  const result: Record<string, { x: number; y: number }> = {}
  const layouts = new Map((roadmap.value?.layouts ?? []).map(x => [x.todoId!, { x: x.x ?? 0, y: x.y ?? 0 }]))
  nodes.value.forEach((node, index) => {
    if (!node.uid) return
    result[node.uid] = layouts.get(node.uid) ?? { x: 40 + (index % 4) * 280, y: 36 + Math.floor(index / 4) * 160 }
  })
  return result
})
const canvasWidth = computed(() => Math.max(1120, ...Object.values(positions.value).map(p => p.x + 250)))
const canvasHeight = computed(() => Math.max(560, ...Object.values(positions.value).map(p => p.y + 140)))
const edges = computed(() => dependencies.value.flatMap(edge => {
  const from = positions.value[edge.predecessorId ?? '']
  const to = positions.value[edge.successorId ?? '']
  if (!from || !to) return []
  const blocked = nodeById.value.get(edge.successorId ?? '')?.isBlocked
  const x1 = from.x + 210, y1 = from.y + 54, x2 = to.x, y2 = to.y + 54
  return [{ id: edge.predecessorId + ':' + edge.successorId, d: `M ${x1} ${y1} C ${x1 + 65} ${y1}, ${x2 - 65} ${y2}, ${x2} ${y2}`, blocked }]
}))
const incoming = computed(() => dependencies.value.filter(x => x.successorId === selectedId.value))

function titleOf(todo: Partial<TodoDto>) { return todo.title || todo.desc || '未命名任务' }
function statusValue(todo: TodoDto) {
  if (todo.status === 'Completed' || Number(todo.status) === TodoStatus.Completed) return 3
  if (todo.status === 'Execute' || Number(todo.status) === TodoStatus.Execute) return 2
  if (todo.status === 'Wait' || Number(todo.status) === TodoStatus.Wait) return 1
  return 0
}
function statusLabel(todo: TodoDto) { return ['初始', '等待分配', '进行中', '已完成'][statusValue(todo)] }
function statusClass(todo: TodoDto) {
  if (statusValue(todo) === 3) return 'done'
  if (todo.isBlocked) return 'blocked'
  if (statusValue(todo) === 2) return 'active'
  return 'pending'
}
async function loadRoadmap() {
  if (!selectedPlanId.value) { roadmap.value = null; selectedId.value = ''; return }
  loading.value = true
  try {
    roadmap.value = await api.apiTodoRoadmapGet({ planId: selectedPlanId.value })
    if (!nodes.value.some(x => x.uid === selectedId.value)) selectedId.value = nodes.value[0]?.uid ?? ''
  } catch (error) {
    ElMessage.error(error instanceof Error ? error.message : '公开路线图加载失败')
  } finally { loading.value = false }
}
async function refresh() {
  loading.value = true
  try {
    await store.fetchPlans()
    await loadRoadmap()
  } catch (error) {
    ElMessage.error(error instanceof Error ? error.message : '公开开发计划加载失败')
  } finally { loading.value = false }
}
async function changePlan(planId: string) {
  await store.selectPlan(planId)
  await loadRoadmap()
}
onMounted(refresh)
</script>

<template>
  <div class="roadmap-page">
    <header class="roadmap-header">
      <div><div class="eyebrow">MONIREPO / PUBLIC DEVELOPMENT ROADMAP</div><h1>开发路线图</h1><p>公开浏览开发计划、任务进度与前置依赖关系。</p></div>
      <div class="header-actions">
        <ElSelect :model-value="selectedPlanId" placeholder="选择开发计划" class="plan-select" @change="changePlan">
          <ElOption v-for="plan in plans" :key="plan.uid" :label="plan.name ?? '未命名计划'" :value="plan.uid!" />
        </ElSelect>
        <ElButton :icon="Refresh" @click="refresh">刷新</ElButton>
      </div>
    </header>
    <section class="stats-row">
      <div><span>当前计划</span><strong>{{ store.selectedPlan?.name ?? '未选择' }}</strong></div>
      <div><span>任务节点</span><strong>{{ nodes.length }}</strong></div>
      <div><span>已完成</span><strong>{{ completedCount }} / {{ nodes.length }}</strong></div>
      <div class="progress-stat"><span>完成进度</span><strong>{{ progress }}%</strong><div class="progress-track"><i :style="{ width: progress + '%' }" /></div></div>
    </section>
    <main class="roadmap-workspace" v-loading="loading || store.loadingPlans">
      <section class="canvas-viewport">
        <div class="roadmap-canvas" :style="{ width: canvasWidth + 'px', height: canvasHeight + 'px' }">
          <svg class="edge-layer" :width="canvasWidth" :height="canvasHeight">
            <defs><marker id="public-roadmap-arrow" markerWidth="8" markerHeight="8" refX="6" refY="3" orient="auto"><path d="M0,0 L0,6 L6,3 Z" fill="#80949a" /></marker></defs>
            <path v-for="edge in edges" :key="edge.id" :d="edge.d" :class="['dependency-edge', { blocked: edge.blocked }]" marker-end="url(#public-roadmap-arrow)" />
          </svg>
          <article v-for="node in nodes" :key="node.uid" class="roadmap-node"
            :class="[statusClass(node), { selected: selectedId === node.uid }]"
            :style="{ left: (positions[node.uid!]?.x ?? 0) + 'px', top: (positions[node.uid!]?.y ?? 0) + 'px' }"
            @click="selectedId = node.uid ?? ''">
            <div class="node-top"><span>{{ node.isBlocked ? '等待前置' : statusLabel(node) }}</span><span>优先级 {{ node.priority ?? 1 }}</span></div>
            <h3>{{ titleOf(node) }}</h3>
            <p>{{ node.description || '暂无任务说明' }}</p>
            <div class="node-bottom"><span v-if="node.ftid">子任务</span><span v-if="node.incompleteDependencyCount">{{ node.incompleteDependencyCount }} 项前置未完成</span><span v-if="node.hasChildren">含子任务</span></div>
          </article>
          <div v-if="!nodes.length" class="empty-canvas">当前没有可公开展示的任务节点。</div>
        </div>
      </section>
      <aside class="detail-panel" v-if="selected">
        <div class="detail-kicker">PUBLIC TASK</div>
        <h2>{{ titleOf(selected) }}</h2>
        <p class="detail-description">{{ selected.description || '暂无任务说明' }}</p>
        <div class="detail-state" :class="statusClass(selected)">{{ selected.isBlocked ? '被前置任务阻塞' : statusLabel(selected) }}</div>
        <div class="detail-meta"><span>优先级</span><strong>{{ selected.priority ?? 1 }}</strong></div>
        <div class="detail-meta"><span>父任务</span><strong>{{ selected.ftid ? titleOf(nodeById.get(selected.ftid) ?? {}) : '无' }}</strong></div>
        <div class="detail-meta"><span>未完成前置</span><strong>{{ selected.incompleteDependencyCount ?? 0 }}</strong></div>
        <div class="dependency-section">
          <div class="section-heading"><strong>前置依赖</strong></div>
          <div v-if="incoming.length === 0" class="muted">没有前置依赖</div>
          <div v-for="edge in incoming" :key="edge.predecessorId" class="dependency-item">
            <span>{{ titleOf(nodeById.get(edge.predecessorId ?? '') ?? {}) }}</span>
            <ElTag v-if="nodeById.get(edge.predecessorId ?? '')?.status === 'Completed' || Number(nodeById.get(edge.predecessorId ?? '')?.status) === 3" type="success" size="small">已完成</ElTag>
            <ElTag v-else type="info" size="small">未完成</ElTag>
          </div>
          <p class="hint">依赖关系仅用于说明任务解锁条件，不代表父子归属。</p>
        </div>
      </aside>
      <aside class="detail-panel empty-detail" v-else><Lock /><p>选择一个任务节点查看详情</p></aside>
    </main>
  </div>
</template>

<style scoped>
.roadmap-page { display:flex; flex-direction:column; gap:18px; padding:24px; height:100%; min-height:0; box-sizing:border-box; overflow-x:hidden; overflow-y:auto; -webkit-overflow-scrolling:touch; overscroll-behavior-y:contain; color:var(--el-text-color-primary); }
.roadmap-header { display:flex; justify-content:space-between; align-items:flex-end; gap:20px; flex-wrap:wrap; }
.eyebrow,.detail-kicker { color:var(--el-text-color-secondary); font-size:10px; letter-spacing:1.6px; }
.roadmap-header h1 { margin:7px 0; font-size:26px; }.roadmap-header p { margin:0; color:var(--el-text-color-secondary); font-size:13px; }
.header-actions { display:flex; gap:8px; flex-wrap:wrap; }.plan-select { width:240px; }
.stats-row { display:grid; grid-template-columns:repeat(4,minmax(130px,1fr)); gap:10px; }
.stats-row > div { padding:12px 15px; background:var(--el-bg-color); border:1px solid var(--el-border-color-light); border-radius:8px; display:flex; flex-direction:column; gap:5px; }
.stats-row span { color:var(--el-text-color-secondary); font-size:12px; }.stats-row strong { font-size:18px; }
.progress-track { height:4px; border-radius:5px; background:var(--el-fill-color); overflow:hidden; }.progress-track i { display:block; height:100%; background:var(--el-color-success); }
.roadmap-workspace { display:grid; grid-template-columns:minmax(0,1fr) 300px; min-height:600px; border:1px solid var(--el-border-color); border-radius:10px; overflow:hidden; background:var(--el-bg-color-page); }
.canvas-viewport { min-width:0; min-height:600px; overflow:auto;scrollbar-width: none; }.roadmap-canvas { position:relative; overflow:visible; min-width:0; min-height:560px; background-image:linear-gradient(var(--el-border-color-lighter) 1px,transparent 1px),linear-gradient(90deg,var(--el-border-color-lighter) 1px,transparent 1px); background-size:28px 28px; }
.edge-layer { position:absolute; inset:0; pointer-events:none; overflow:visible; }.dependency-edge { fill:none; stroke:var(--el-color-primary); stroke-width:2.2; opacity:.8; }.dependency-edge.blocked { stroke:var(--el-color-warning); stroke-dasharray:6 5; }
.roadmap-node { position:absolute; width:210px; min-height:108px; padding:12px; border:1px solid var(--el-border-color); border-left:4px solid var(--el-color-info); border-radius:8px; background:var(--el-bg-color); box-shadow:0 3px 12px #00000012; cursor:pointer; user-select:none; z-index:1; }
.roadmap-node.selected { outline:2px solid var(--el-color-primary); outline-offset:2px; }.roadmap-node.done { border-left-color:var(--el-color-success); }.roadmap-node.active { border-left-color:var(--el-color-warning); }.roadmap-node.blocked { border-left-color:var(--el-color-danger); }
.node-top,.node-bottom { display:flex; justify-content:space-between; gap:8px; color:var(--el-text-color-secondary); font-size:10px; }.roadmap-node h3 { margin:9px 0 5px; font-size:14px; }.roadmap-node p { margin:0 0 8px; font-size:11px; color:var(--el-text-color-secondary); overflow:hidden; display:-webkit-box; -webkit-line-clamp:2; -webkit-box-orient:vertical; }
.empty-canvas { position:absolute; inset:0; display:grid; place-items:center; color:var(--el-text-color-secondary); }.detail-panel { padding:20px 16px; background:var(--el-bg-color); border-left:1px solid var(--el-border-color); }
.detail-panel h2 { font-size:19px; margin:9px 0; overflow-wrap:anywhere; }.detail-description { color:var(--el-text-color-secondary); font-size:13px; line-height:1.65; white-space:pre-wrap; }
.detail-state { display:inline-block; margin:8px 0 12px; padding:4px 8px; border-radius:5px; font-size:12px; background:var(--el-fill-color); }.detail-state.done { color:var(--el-color-success); }.detail-state.blocked { color:var(--el-color-danger); }.detail-state.active { color:var(--el-color-warning); }
.detail-meta { display:flex; justify-content:space-between; gap:8px; padding:8px 0; border-bottom:1px solid var(--el-border-color-lighter); font-size:12px; }.detail-meta span,.muted,.hint { color:var(--el-text-color-secondary); }
.dependency-section { border-top:1px solid var(--el-border-color); padding-top:12px; margin-top:14px; }.section-heading { display:flex; align-items:center; justify-content:space-between; }.dependency-item { display:flex; justify-content:space-between; align-items:center; gap:8px; padding:7px 0; font-size:12px; }.muted,.hint { font-size:12px; }.hint { line-height:1.6; }
.empty-detail { display:flex; flex-direction:column; align-items:center; justify-content:center; color:var(--el-text-color-secondary); }
@media (max-width:1050px) { .roadmap-workspace { grid-template-columns:minmax(0,1fr); }.detail-panel { border-left:0; border-top:1px solid var(--el-border-color); }.stats-row { grid-template-columns:repeat(2,minmax(120px,1fr)); } }
@media (max-width:650px) { .roadmap-header { align-items:stretch; }.header-actions { width:100%; }.plan-select { width:100%; }.stats-row { grid-template-columns:repeat(2,minmax(0,1fr)); }.roadmap-page { padding:14px; } }
</style>
