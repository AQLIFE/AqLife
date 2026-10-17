<script setup lang="ts">
import { AccountApi, type LoginCommand } from '@/api'
import { apiConfiguration } from '@/services/api'
import { useAccountStore } from '@/stores/useAccountStore'
import { tokenStore } from '@aqlife/api-client'
import { validateLoginCommand } from '@aqlife/domain'
import { ElForm, ElFormItem, ElInput, ElButton, ElMessage, ElIcon } from 'element-plus'
import { reactive, ref, type Reactive } from 'vue'
import { useRouter } from 'vue-router'
import { Lock, User, ArrowRight } from '@element-plus/icons-vue'

const accountStore = useAccountStore()
const router = useRouter()
const login: Reactive<LoginCommand> = reactive({ accountName: '', secretKey: '' })
const submitting = ref(false)
const request = new AccountApi(apiConfiguration)

async function submitLogin() {
  if (submitting.value) return

  const validation = validateLoginCommand(login.accountName, login.secretKey)
  if (!validation.ok) {
    ElMessage.error(validation.message)
    return
  }

  submitting.value = true
  try {
    const token = await request.apiAccountLoginPost({ loginCommand: login })
    tokenStore.set(token)
    accountStore.setToken(token)
    ElMessage.success('登录成功')
    await router.push('/')
  } catch (error) {
    console.error(error)
    ElMessage.error('登录失败，请检查账户名或密码')
  } finally {
    submitting.value = false
  }
}
</script>

<template>
  <main class="auth-page">
    <section class="auth-card login-card">
      <header class="auth-brand">
        <div class="brand-mark" aria-hidden="true">M</div>
        <div>
          <p class="eyebrow">MONIREPO · MGS</p>
          <p class="brand-name">MoniRepo</p>
        </div>
      </header>

      <div class="auth-heading">
        <h1>欢迎回来</h1>
        <p>登录你的博客管理工作区，继续管理内容与发布。</p>
      </div>

      <ElForm class="login-form" @submit.prevent="submitLogin">
        <ElFormItem label="账户名">
          <ElInput
            v-model="login.accountName"
            clearable
            autocomplete="username"
            placeholder="请输入账户名"
            :prefix-icon="User"
            size="large"
            @keyup.enter="submitLogin"
          />
        </ElFormItem>
        <ElFormItem label="密码">
          <ElInput
            v-model="login.secretKey"
            clearable
            autocomplete="current-password"
            placeholder="请输入密码"
            show-password
            :prefix-icon="Lock"
            size="large"
            @keyup.enter="submitLogin"
          />
        </ElFormItem>
        <ElButton
          class="submit-button"
          type="primary"
          size="large"
          native-type="submit"
          :loading="submitting"
        >
          登录工作区
          <ElIcon class="button-icon"><ArrowRight /></ElIcon>
        </ElButton>
      </ElForm>

      <footer class="auth-footer">
        <span>还没有完成系统初始化？</span>
        <button type="button" class="text-link" @click="router.push('/register')">创建账户</button>
      </footer>
    </section>
    <p class="page-footnote">专注内容创作 · 让发布流程更简单</p>
  </main>
</template>

<style scoped>
.auth-page {
  min-height: 100vh;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  gap: 18px;
  padding: 36px 20px 24px;
  background:
    radial-gradient(ellipse at 12% 8%, rgba(63, 111, 229, .09), transparent 34%),
    radial-gradient(ellipse at 90% 88%, rgba(63, 111, 229, .06), transparent 32%),
    var(--mgs-bg);
}
.auth-card {
  width: min(100%, 440px);
  padding: 32px;
  border: 1px solid var(--mgs-border);
  border-radius: 16px;
  background: var(--mgs-surface);
  box-shadow: 0 20px 60px rgba(23, 25, 28, .08);
}
.auth-brand { display: flex; align-items: center; gap: 12px; }
.brand-mark {
  width: 42px; height: 42px; display: grid; place-items: center;
  border-radius: 12px; background: var(--mgs-accent); color: #fff;
  font-size: 21px; font-weight: 750; letter-spacing: -.04em;
  box-shadow: 0 7px 16px rgba(63, 111, 229, .22);
}
.eyebrow { margin: 0 0 3px; color: var(--mgs-muted); font-size: 10px; font-weight: 700; letter-spacing: .14em; }
.brand-name { margin: 0; color: var(--mgs-text); font-size: 15px; font-weight: 700; letter-spacing: -.02em; }
.auth-heading { margin: 34px 0 24px; }
.auth-heading h1 { margin: 0; color: var(--mgs-text); font-size: 27px; font-weight: 700; line-height: 1.25; letter-spacing: -.035em; }
.auth-heading p { margin: 9px 0 0; color: var(--mgs-secondary); font-size: 13px; line-height: 1.7; }
.login-form :deep(.el-form-item) { margin-bottom: 19px; }
.login-form :deep(.el-form-item__label) { color: var(--mgs-secondary); font-size: 12px; font-weight: 600; }
.login-form :deep(.el-input__wrapper) { min-height: 44px; border-radius: 9px; box-shadow: 0 0 0 1px var(--mgs-border) inset; }
.login-form :deep(.el-input__wrapper:hover) { box-shadow: 0 0 0 1px var(--mgs-border-strong) inset; }
.login-form :deep(.el-input__wrapper.is-focus) { box-shadow: 0 0 0 1px var(--mgs-accent) inset; }
.submit-button { width: 100%; height: 46px; margin-top: 4px; border-radius: 9px; font-weight: 650; }
.button-icon { margin-left: 8px; }
.auth-footer { display: flex; flex-wrap: wrap; justify-content: center; gap: 5px; margin-top: 24px; padding-top: 20px; border-top: 1px solid var(--mgs-border); color: var(--mgs-muted); font-size: 12px; }
.text-link { padding: 0; border: 0; background: none; color: var(--mgs-accent); font: inherit; font-weight: 650; cursor: pointer; }
.text-link:hover { text-decoration: underline; }
.page-footnote { margin: 0; color: var(--mgs-muted); font-size: 11px; text-align: center; }
@media (max-width: 480px) {
  .auth-page { justify-content: flex-start; padding: 28px 14px 20px; }
  .auth-card { padding: 25px 21px; border-radius: 13px; }
  .auth-heading { margin-top: 28px; }
  .auth-heading h1 { font-size: 24px; }
}
</style>
