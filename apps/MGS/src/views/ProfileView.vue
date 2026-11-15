<template>
  <div class="profile-page">
    <div class="profile-card">
      <div class="profile-intro">
        <div><h2>Profile</h2><p>更新你的博客头像、名称和公开简介。</p></div>
        <ElButton :type="isActive ? 'default' : 'primary'" :icon="isActive ? Lock : Unlock" @click="isActive = !isActive">{{ isActive ? 'Unlock' : 'Lock' }}</ElButton>
      </div>
      <ElForm class="profile-form" label-position="top">
        <div class="profile-avatar"><ElFormItem label="Avatar"><ElTooltip content="请上传对应博客头像"><ImageUpload :disabled="isActive" iconSize="112px" :src="preview()" v-model:file="avatarFile" accept=".png,.jpeg,.jpg"/></ElTooltip></ElFormItem></div>
        <div class="profile-fields">
          <ElFormItem label="Name"><ElInput :disabled="isActive" v-model="accountStore.systemAccount!.name" /></ElFormItem>
          <ElFormItem label="Description"><ElInput type="textarea" :rows="6" :disabled="isActive" v-model="accountStore.systemAccount!.desc" /></ElFormItem>
          <div class="form-actions"><span v-if="isActive" class="hint">Unlock the profile to edit.</span><ElButton type="primary" :disabled="isActive" @click="update">Save changes</ElButton></div>
        </div>
      </ElForm>
    </div>

    <div class="profile-card password-card">
      <div class="profile-intro"><div><h2>修改密码</h2><p>使用当前密码验证身份，并设置新密码。</p></div></div>
      <ElForm label-position="top" class="password-form">
        <ElFormItem label="当前密码" required><ElInput v-model="password.oldPassword" type="password" show-password autocomplete="current-password" /></ElFormItem>
        <ElFormItem label="新密码" required><ElInput v-model="password.newPassword" type="password" show-password autocomplete="new-password" /></ElFormItem>
        <ElFormItem label="确认新密码" required><ElInput v-model="password.confirmPassword" type="password" show-password autocomplete="new-password" /></ElFormItem>
        <div class="form-actions"><ElButton type="primary" :loading="passwordSaving" @click="updatePassword">更新密码</ElButton></div>
      </ElForm>
    </div>
  </div>
</template>

<script setup lang="ts">
import { notify } from '@aqlife/ui-shared'
import { ElButton, ElForm, ElFormItem, ElInput, ElTooltip } from 'element-plus'
import { Lock, Unlock } from '@element-plus/icons-vue'
import { reactive, ref, type Ref } from 'vue'
import { useAccountStore } from '@/stores/useAccountStore'
import ImageUpload from '@/components/ImageUpload.vue'
import { AccountApi, type AccountProfile } from '@/api'
import { apiConfiguration } from '@/services/api'
import { useFileStore } from '@/stores/useFileStore'

const isActive = ref(true)
const accountStore = useAccountStore()
const accountApi = new AccountApi(apiConfiguration)
const avatarFile: Ref<File | null> = ref(null)
const passwordSaving = ref(false)
const password = reactive({ oldPassword: '', newPassword: '', confirmPassword: '' })

function preview(): string {
  const avatar = accountStore.systemAccount?.avatar
  if (avatar == null) return ''
  const fileStore = useFileStore()
  return fileStore.previewUrl.get(avatar) ?? (import.meta.env.VITE_API + '/api/File/preview?UID=' + avatar)
}

async function update() {
  try {
    if (avatarFile.value) await accountApi.apiAccountAvatarPatch({ avatar: avatarFile.value })
    const accountProfile: AccountProfile = {
      name: accountStore.systemAccount?.name,
      desc: accountStore.systemAccount?.desc,
    }
    await accountApi.apiAccountProfilePatch({ accountProfile })
    accountStore.systemAccount = await accountApi.apiAccountGet()
    avatarFile.value = null
    isActive.value = true
    notify.success('Profile updated')
  } catch (error) {
    notify.error(error instanceof Error ? error.message : 'Profile update failed')
  }
}

async function updatePassword() {
  if (!password.oldPassword || !password.newPassword || !password.confirmPassword) {
    notify.warning('请完整填写密码信息')
    return
  }
  if (password.newPassword !== password.confirmPassword) {
    notify.warning('两次输入的新密码不一致')
    return
  }

  passwordSaving.value = true
  try {
    await accountApi.apiAccountPwdPatch({
      updatePwdCommand: {
        oldPassword: password.oldPassword,
        newPassword: password.newPassword,
      },
    })
    password.oldPassword = ''
    password.newPassword = ''
    password.confirmPassword = ''
    notify.success('密码已更新')
  } catch (error) {
    notify.error(error instanceof Error ? error.message : '密码更新失败')
  } finally {
    passwordSaving.value = false
  }
}
</script>

<style scoped>
.profile-page { display: grid; gap: 18px; }
.profile-card { max-width: 900px; padding: 24px; background: var(--mgs-surface); border: 1px solid var(--mgs-border); border-radius: 10px; box-shadow: var(--mgs-shadow); }
.profile-intro { display: flex; align-items: flex-start; justify-content: space-between; gap: 16px; margin-bottom: 24px; }
.profile-intro h2 { margin: 0; font-size: 16px; }
.profile-intro p { margin: 5px 0 0; color: var(--mgs-muted); font-size: 12px; }
.profile-form { display: grid; grid-template-columns: 140px minmax(0, 1fr); gap: 28px; }
.profile-avatar :deep(.el-form-item__content) { justify-content: center; }
.profile-fields { min-width: 0; }
.password-form { max-width: 520px; }
.form-actions { display: flex; align-items: center; justify-content: flex-end; gap: 12px; margin-top: 18px; }
.hint { margin-right: auto; color: var(--mgs-muted); font-size: 12px; }
@media (max-width: 640px) {
  .profile-form { grid-template-columns: 1fr; }
  .profile-avatar :deep(.el-form-item__content) { justify-content: flex-start; }
}
</style>
