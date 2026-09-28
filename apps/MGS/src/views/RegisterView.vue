<template>
  <div class="register-page">
    <section class="register-card">
      <header class="register-header">
        <div>
          <p class="eyebrow">MGS Setup</p>
          <h1>注册博客系统</h1>
          <p class="description">按步骤完成账户、头像、个人主页和最终确认。</p>
        </div>
      </header>

      <RegisterStep :current-step="currentStep" />

      <main class="register-content">
        <component
          :is="stepComponents[currentStep]"
          @next="handleNext"
          @prev="handlePrev"
        />
      </main>
    </section>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useRegisterStore } from '@/stores/useRegisterStore'
import RegisterStep from '@/components/RegisterStep.vue'
import StepAccount from '@/components/StepAccount.vue'
import StepAvatar from '@/components/StepAvatar.vue'
import StepSocial from '@/components/StepSocial.vue'
import StepCommit from '@/components/StepCommit.vue'

const registerStore = useRegisterStore()
const currentStep = ref(0)

const stepComponents = [StepAccount, StepAvatar, StepSocial, StepCommit]

function handleNext(): void {
  if (currentStep.value >= stepComponents.length - 1) return

  currentStep.value += 1
  registerStore.steps[currentStep.value]!.status = 'process'
}

function handlePrev(): void {
  if (currentStep.value <= 0) return

  currentStep.value -= 1

  registerStore.steps.forEach((step, index) => {
    step.status = index < currentStep.value ? 'finish' : index === currentStep.value ? 'process' : 'wait'
  })
}
</script>

<style scoped>
.register-page {
  min-height: 100vh;
  display: flex;
  justify-content: center;
  align-items: flex-start;
  padding: 48px 24px;
  background: var(--mgs-bg);
}

.register-card {
  width: min(920px, 100%);
  padding: 28px 32px 32px;
  background: var(--mgs-surface);
  border: 1px solid var(--mgs-border);
  border-radius: 12px;
  box-shadow: var(--mgs-shadow);
}

.register-header {
  margin-bottom: 24px;
}

.eyebrow {
  margin: 0 0 6px;
  color: var(--mgs-accent);
  font-size: 12px;
  font-weight: 600;
  letter-spacing: .08em;
  text-transform: uppercase;
}

.register-header h1 {
  margin: 0;
  font-size: 24px;
  line-height: 1.3;
}

.description {
  margin: 7px 0 0;
  color: var(--mgs-secondary);
  font-size: 13px;
}

.register-content {
  width: min(680px, 100%);
  margin: 32px auto 0;
}

.register-content :deep(.el-form) {
  width: 100%;
}

@media (max-width: 720px) {
  .register-page {
    padding: 20px 12px;
  }

  .register-card {
    padding: 20px 16px 24px;
  }

  .register-content {
    margin-top: 24px;
  }
}
</style>
