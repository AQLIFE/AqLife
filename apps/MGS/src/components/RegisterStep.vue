<template>
  <ElSteps :active="props.currentStep" simple finish-status="success">
    <ElStep
      v-for="(item, index) in registerStore.steps"
      :key="index"
      :title="item.title"
      :icon="item.icon"
    />
  </ElSteps>
</template>

<script setup lang="ts">
import { ElStep, ElSteps } from 'element-plus'
import { useRegisterStore } from '@/stores/useRegisterStore'

const props = withDefaults(defineProps<{
  currentStep?: number
}>(), {
  currentStep: 0,
})

const registerStore = useRegisterStore()
</script>
