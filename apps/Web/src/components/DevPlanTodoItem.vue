<script setup lang="ts">
import type { TodoDto } from '@/api'
import { computed } from 'vue'
import { ElTag } from 'element-plus'
import { useTodoStore } from '@/stores/todoStore'

const props = defineProps<{ item: TodoDto; index: number }>()
const store = useTodoStore()
const predecessorTitles = computed(() => (props.item.predecessorIds ?? []).map(id =>
  store.todoList.find(todo => todo.uid === id)?.title || '未知任务',
))

function statusLabel(status?: string | null) {
  if (status === 'Completed' || Number(status) === 3) return '已完成'
  if (status === 'Execute' || Number(status) === 2) return '进行中'
  if (status === 'Wait' || Number(status) === 1) return '等待分配'
  return '初始'
}
</script>

<template>
  <article class="todo-node">
    <div class="content-item">
      <span class="serial">{{ index + 1 }}</span>
      <div class="description">
        <strong>{{ item.title || '（未命名任务）' }}</strong>
        <small v-if="item.description">{{ item.description }}</small>
      </div>
      <div class="node-state">
        <ElTag v-if="item.isBlocked" type="danger" size="small">阻塞</ElTag>
        <ElTag v-else-if="statusLabel(item.status) === '已完成'" type="success" size="small">已完成</ElTag>
        <ElTag v-else size="small">{{ statusLabel(item.status) }}</ElTag>
      </div>
    </div>
    <div v-if="predecessorTitles.length" class="predecessors">
      前置依赖：{{ predecessorTitles.join('、') }}
      <span v-if="item.incompleteDependencyCount">（{{ item.incompleteDependencyCount }} 项尚未完成）</span>
    </div>
  </article>
</template>

<style scoped>
.todo-node { width: 100%; }
.content-item { display:flex; align-items:center; gap:12px; min-height:46px; padding:8px 12px; border-bottom:1px dotted var(--back_color_lv3); background:var(--back_color_lv1); }
.serial { width:28px; flex:none; color:var(--el-text-color-secondary); font-size:12px; text-align:center; }
.description { min-width:0; flex:1; display:flex; flex-direction:column; gap:3px; }
.description strong { font-weight:600; }
.description small { color:var(--el-text-color-secondary); white-space:normal; }
.node-state { flex:none; }
.predecessors { padding:6px 16px 8px 52px; color:var(--el-text-color-secondary); font-size:12px; background:var(--back_color_lv1); }
</style>
