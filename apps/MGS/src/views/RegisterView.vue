<template>
  <ElCol>
    <component :is="stepComponents[currentStep]" @next="handleNext" @prev="handlePrev"/>
  </ElCol>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useRegisterStore } from '@/stores/useRegisterStore'
import { ElCol } from 'element-plus'

// 引入拆分后的子组件
import StepAccount from '@/components/StepAccount.vue'
import StepAvatar from '@/components/StepAvatar.vue'
import StepSocial from '@/components/StepSocial.vue'
import StepCommit from '@/components/StepCommit.vue'

const registerStore = useRegisterStore()
const currentStep = ref(0)

// 步骤组件映射表
const stepComponents = [StepAccount, StepAvatar, StepSocial,StepCommit]

function handleNext(): void {
  registerStore.$patch((state) => {
    if(currentStep.value < state.steps.length - 1) {
      currentStep.value++
      state.steps[currentStep.value]!.status = 'process'
    }
  })
}

function handlePrev(): void {
  if (currentStep.value > 0) {
    currentStep.value--
  }
}
</script>

<style scoped>
.el-form {
  width: 30vw;
  text-align: center;
  justify-content: center;
  align-items: center;
  align-self: center;
  align-content: center;
  justify-self: center;
  justify-items: center;
}

.el-col {
  display: flex;
  flex-direction: row;
  justify-content: center;
}


</style>
