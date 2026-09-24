<script setup lang="ts">
import type { TodoDto } from '@/api'
import { computed, ref } from 'vue'
import { ElButton, ElCol } from 'element-plus'
import { ArrowRight } from '@element-plus/icons-vue'

const props = defineProps<{
  item: TodoDto
  index: number
  level: number
}>()

const expanded = ref(true)
const children = computed(() => props.item.todoList ?? [])
const hasChildren = computed(() => props.item.hasChildren === true || children.value.length > 0)
</script>

<template>
  <div class="todo-node">
    <div class="content-item" :style="{ paddingLeft: `${level * 20 + 10}px` }">
      <ElCol :span="2" class="serial">{{ index + 1 }}</ElCol>
      <ElCol :span="18" class="description">{{ item.desc }}</ElCol>
      <ElCol :span="4" class="serial">
        <ElButton
          v-if="hasChildren"
          :icon="ArrowRight"
          link
          :class="{ expanded }"
          :aria-label="expanded ? '折叠子任务' : '展开子任务'"
          :aria-expanded="expanded"
          @click="expanded = !expanded"
        />
      </ElCol>
    </div>

    <div v-if="hasChildren && expanded" class="children">
      <DevPlanTodoItem
        v-for="(child, childIndex) in children"
        :key="child.uid ?? childIndex"
        :item="child"
        :index="childIndex"
        :level="level + 1"
      />
    </div>
  </div>
</template>

<style scoped>
.todo-node {
  width: 100%;
}

.content-item {
  display: flex;
  align-items: center;
  min-height: 42px;
  padding-top: 10px;
  padding-right: 10px;
  padding-bottom: 10px;
  border-bottom: 1px dotted var(--back_color_lv3);
  background-color: var(--back_color_lv1);
}

.serial {
  display: flex;
  align-items: center;
  justify-content: center;
}

.description {
  min-width: 0;
}

.content-item > :deep(.el-col > .el-button) {
  margin: 5%;
  transition: transform 0.15s ease;
}

.content-item > :deep(.el-col > .el-button.expanded) {
  transform: rotate(90deg);
}

.children {
  width: 100%;
}
</style>
