<script setup lang="ts">
import { ElCol, ElRow } from 'element-plus'
import { onBeforeMount } from 'vue'
import { useTodoStore } from '@/stores/todoStore'
import DevPlanTodoItem from './DevPlanTodoItem.vue'

const todoStore = useTodoStore()
const devPlanTitle = '开发计划'

onBeforeMount(() => {
  if (todoStore.todoList.length === 0) {
    return todoStore.refresh()
  }
})
</script>

<template>
  <ElCol id="title">{{ devPlanTitle }}</ElCol>
  <ElCol id="contents">
    <ElRow flex="column">
      <DevPlanTodoItem
        v-for="(item, index) in todoStore.todoList"
        :key="item.uid ?? index"
        :item="item"
        :index="index"
        :level="0"
      />
    </ElRow>
  </ElCol>
</template>

<style scoped>
#title {
  text-align: center;
  background-color: var(--topColor);
  height: var(--inlineCenter);
  line-height: var(--inlineCenter);
}

#contents .el-row {
  height: auto;
}
</style>
