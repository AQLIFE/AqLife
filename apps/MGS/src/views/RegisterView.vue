<template>
  <div class="register-page">
    <section class="register-card">
      <header class="register-header">
        <div class="brand-mark" aria-hidden="true">M</div>
        <div>
          <p class="eyebrow">MONIREPO · MGS SETUP</p>
          <h1>初始化你的博客工作区</h1>
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
  padding: 36px 20px;
  background:
    radial-gradient(ellipse at 12% 8%, rgba(63, 111, 229, .09), transparent 34%),
    radial-gradient(ellipse at 90% 88%, rgba(63, 111, 229, .06), transparent 32%),
    var(--mgs-bg);
}

.register-card {
  width: min(920px, 100%);
  padding: 30px 34px 34px;
  background: var(--mgs-surface);
  border: 1px solid var(--mgs-border);
  border-radius: 16px;
  box-shadow: 0 20px 60px rgba(23, 25, 28, .08);
}

.register-header {
  display: flex;
  align-items: center;
  gap: 14px;
  margin-bottom: 24px;
}

.brand-mark {
  width: 42px;
  height: 42px;
  flex: 0 0 42px;
  display: grid;
  place-items: center;
  border-radius: 12px;
  background: var(--mgs-accent);
  color: #fff;
  font-size: 21px;
  font-weight: 750;
  box-shadow: 0 7px 16px rgba(63, 111, 229, .22);
}

.eyebrow {
  margin: 0 0 5px;
  color: var(--mgs-accent);
  font-size: 10px;
  font-weight: 700;
  letter-spacing: .14em;
  text-transform: uppercase;
}

.register-header h1 {
  margin: 0;
  color: var(--mgs-text);
  font-size: 24px;
  line-height: 1.3;
  letter-spacing: -.03em;
}

.description {
  margin: 7px 0 0;
  color: var(--mgs-secondary);
  font-size: 13px;
  line-height: 1.7;
}

.register-content {
  width: min(680px, 100%);
  margin: 30px auto 0;
}

.register-content :deep(.el-form) { width: 100%; }
.register-content :deep(.el-form-item__label) { color: var(--mgs-secondary); font-size: 12px; font-weight: 600; }
.register-content :deep(.el-input__wrapper),
.register-content :deep(.el-textarea__inner) {
  border-radius: 9px;
  box-shadow: 0 0 0 1px var(--mgs-border) inset;
}
.register-content :deep(.el-input__wrapper:hover),
.register-content :deep(.el-textarea__inner:hover) {
  box-shadow: 0 0 0 1px var(--mgs-border-strong) inset;
}
.register-content :deep(.el-input__wrapper.is-focus),
.register-content :deep(.el-textarea__inner:focus) {
  box-shadow: 0 0 0 1px var(--mgs-accent) inset;
}
.register-content :deep(.el-button) { min-height: 36px; border-radius: 8px; font-weight: 600; }
.register-content :deep(.el-button--primary) { box-shadow: 0 5px 12px rgba(63, 111, 229, .12); }
.register-card :deep(.el-steps--simple) { padding: 14px 18px; border-radius: 10px; background: var(--mgs-surface-soft); }
.register-card :deep(.el-step__title) { font-size: 12px; }

@media (max-width: 720px) {
  .register-page { padding: 18px 12px; }
  .register-card { padding: 22px 16px 24px; border-radius: 13px; }
  .register-header { align-items: flex-start; }
  .register-header h1 { font-size: 21px; }
  .register-content { margin-top: 24px; }
  .register-card :deep(.el-steps--simple) { padding: 12px 10px; }
  .register-card :deep(.el-step__title) { font-size: 11px; }
}
</style>
