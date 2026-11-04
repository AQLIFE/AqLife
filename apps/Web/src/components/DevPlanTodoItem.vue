<script setup lang="ts">
import type { TodoDto } from '@/api'
import { computed, ref } from 'vue'
import { ElButton, ElTag } from 'element-plus'
import { ArrowRight } from '@element-plus/icons-vue'

defineOptions({ name: 'DevPlanTodoItem' })
const props = withDefaults(defineProps<{ item: TodoDto; index: number; level?: number }>(), { level: 0 })
const expanded = ref(true)
const children = computed(() => props.item.todoList ?? [])
const hasChildren = computed(() => props.item.hasChildren === true || children.value.length > 0)
const title = computed(() => props.item.title || props.item.desc || '（未命名任务）')
function statusLabel(status?: string | null) {
  if (status === 'Completed' || Number(status) === 3) return '已完成'
  if (status === 'Execute' || Number(status) === 2) return '进行中'
  if (status === 'Wait' || Number(status) === 1) return '等待分配'
  return '初始'
}
</script>

<template>
  <div class="todo-node" :style="{ '--node-level': level }">
    <div class="content-item">
      <span class="serial">{{ index + 1 }}</span>
      <div class="description">
        <strong>{{ title }}</strong>
        <small v-if="item.description">{{ item.description }}</small>
      </div>
      <div class="node-state">
        <ElTag v-if="item.isBlocked" type="danger" size="small">阻塞</ElTag>
        <ElTag v-else-if="statusLabel(item.status) === '已完成'" type="success" size="small">已完成</ElTag>
        <ElTag v-else size="small">{{ statusLabel(item.status) }}</ElTag>
      </div>
      <ElButton v-if="hasChildren" :icon="ArrowRight" link :class="{ expanded }"
        :aria-label="expanded ? '折叠子任务' : '展开子任务'"
        :aria-expanded="expanded" @click="expanded = !expanded" />
    </div>
    <div v-if="hasChildren && expanded" class="children">
      <DevPlanTodoItem v-for="(child, childIndex) in children" :key="child.uid ?? childIndex"
        :item="child" :index="childIndex" :level="level + 1" />
    </div>
  </div>
</template>

<style scoped>
.todo-node { width: 100%; }
.content-item { display:flex; align-items:center; gap:12px; min-height:46px; padding:8px 12px 8px calc(12px + var(--node-level) * 18px); border-bottom:1px dotted var(--back_color_lv3); background:var(--back_color_lv1); }
.serial { width:28px; flex:none; color:var(--el-text-color-secondary); font-size:12px; text-align:center; }
.description { min-width:0; flex:1; display:flex; flex-direction:column; gap:3px; }
.description strong { font-weight:600; }
.description small { color:var(--el-text-color-secondary); white-space:normal; }
.node-state { flex:none; }
.children { width:100%; }
.content-item :deep(.el-button) { transition:transform .15s ease; }
.content-item :deep(.el-button.expanded) { transform:rotate(90deg); }
</style>
