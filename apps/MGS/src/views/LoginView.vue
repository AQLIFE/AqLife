<script setup lang="ts">
import { AccountApi, type LoginCommand } from '@/api'
import { apiConfiguration } from '@/services/api'
import { useAccountStore } from '@/stores/useAccountStore'
import { tokenStore } from '@aqlife/api-client'
import { validateLoginCommand } from '@aqlife/domain'
import { ElCol, ElForm, ElFormItem, ElInput, ElButton, ElMessage,ElIcon } from 'element-plus'
import { reactive ,type Reactive } from 'vue'
import { useRouter } from 'vue-router'
import { Lock, User } from '@element-plus/icons-vue'

const accountStore = useAccountStore()
const router = useRouter()
const login: Reactive<LoginCommand> = reactive({ accountName: '', secretKey: '' })

const request = new AccountApi(apiConfiguration)

async function submitLogin() {
  const validation = validateLoginCommand(login.accountName, login.secretKey)
  if (!validation.ok) {
    ElMessage.error(validation.message)
    return
  }

  try {
    const token = await request.apiAccountLoginPost({ loginCommand: login })
    tokenStore.set(token)
    accountStore.setToken(token)
    ElMessage.success('登录成功')
    await router.push('/')
  } catch (error) {
    console.error(error)
    ElMessage.error('登录失败，请检查账户名或密码')
  }
}
</script>

<template>
  <ElCol class="loginView">
    <ElForm style="width:30vw;" label-width="55px">
    <ElFormItem label="账户名">
      <ElInput v-model="login.accountName" clearable placeholder="请输入账户名" :prefix-icon="User" />
    </ElFormItem>
    <ElFormItem label="密码">
      <ElInput v-model="login.secretKey" clearable placeholder="请输入密码" show-password :prefix-icon="Lock"/>
    </ElFormItem>
    <ElFormItem>
      <ElButton type="info">Forget</ElButton>
      <ElButton type="primary" @click="submitLogin">Login</ElButton>
    </ElFormItem>
  </ElForm>
  </ElCol>
</template>
<style lang="css" scoped>
.loginView{
  display: flex;flex-direction: row;
  justify-content: center;
  justify-self: center;
  justify-items: center;
  height:100vh;
  align-items: center;
  align-content: center;
  align-self: center;
}
</style>
