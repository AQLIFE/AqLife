<template>
  <ElCol>
    <template v-if="registerStore.steps[currentStep]?.status === 'process'">
      <component :is="stepComponents[currentStep]" @next="handleNext" @prev="handlePrev" />
    </template>
    <div v-else>
      <p>当前步骤未激活或已完成</p>
    </div>
  </ElCol>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useRegisterStore } from '@/stores/stepData'
import { ElCol } from 'element-plus'

// 引入拆分后的子组件
import StepAccount from '@/components/StepAccount.vue'
import StepAvatar from '@/components/StepAvatar.vue'
import StepSocial from '@/components/StepSocial.vue'
import StepCommit from '@/components/stepCommit.vue'

const registerStore = useRegisterStore()
const currentStep = ref(0)

// 步骤组件映射表
const stepComponents = [StepAccount, StepAvatar, StepSocial,StepCommit]

function handleNext(): void {
  registerStore.$patch((state) => {
    state.steps[currentStep.value]!.status = 'finish'
    currentStep.value++
    if (currentStep.value < state.steps.length) {
      state.steps[currentStep.value]!.status = 'process'
    }
  })
}

function handlePrev(): void {
  if (currentStep.value > 0) {
    currentStep.value--
    registerStore.$patch((state)=>{
      state.steps[currentStep.value]!.status = 'process'
      state.steps[currentStep.value+1]!.status = 'wait'
    })
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
