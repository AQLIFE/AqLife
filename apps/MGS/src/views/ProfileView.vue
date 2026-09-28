<template>
  <div class="profile-page"><div class="profile-card">
    <div class="profile-intro"><div><h2>Profile</h2><p>更新你的博客头像、名称和公开简介。</p></div>
      <ElButton :type="isActive ? 'default' : 'primary'" :icon="isActive ? Lock : Unlock" @click="isActive = !isActive">{{ isActive ? 'Unlock' : 'Lock' }}</ElButton>
    </div>
    <ElForm class="profile-form" label-position="top">
      <div class="profile-avatar"><ElFormItem label="Avatar"><ElTooltip content="请上传对应博客头像"><ImageUpload :disabled="isActive" iconSize="112px" :src="preview()" v-model:file="avatarFile" /></ElTooltip></ElFormItem></div>
      <div class="profile-fields">
        <ElFormItem label="Name"><ElInput :disabled="isActive" v-model="accountStore.systemAccount!.name" /></ElFormItem>
        <ElFormItem label="Description"><ElInput type="textarea" :rows="6" :disabled="isActive" v-model="accountStore.systemAccount!.desc" /></ElFormItem>
        <div class="form-actions"><span v-if="isActive" class="hint">Unlock the profile to edit.</span><ElButton type="primary" :disabled="isActive" @click="update">Save changes</ElButton></div>
      </div>
    </ElForm>
  </div></div>
</template>
<script setup lang="ts">
import { ElButton, ElForm, ElFormItem, ElInput, ElMessage, ElTooltip } from 'element-plus'
import { Lock, Unlock } from '@element-plus/icons-vue'
import { ref, type Ref } from 'vue'
import { useAccountStore } from '@/stores/useAccountStore'
import ImageUpload from '@/components/ImageUpload.vue'
import { AccountApi, type AccountProfile } from '@/api'
import { apiConfiguration } from '@/services/api'
import { useFileStore } from '@/stores/useFileStore'
const isActive=ref(true)
const accountStore=useAccountStore()
const avatarFile:Ref<File|null>=ref(null)
function preview():string{const avatar=accountStore.systemAccount?.avatar;if(avatar==null)return '';const fileStore=useFileStore();return fileStore.previewUrl.get(avatar) ?? (import.meta.env.VITE_API + '/api/File/preview?UID=' + avatar)}
async function update(){const accountApi=new AccountApi(apiConfiguration);try{if(avatarFile.value)await accountApi.apiAccountAvatarPatch({avatar:avatarFile.value});const accountProfile:AccountProfile={name:accountStore.systemAccount?.name,desc:accountStore.systemAccount?.desc};await accountApi.apiAccountProfilePatch({accountProfile});accountStore.systemAccount=await accountApi.apiAccountGet();avatarFile.value=null;isActive.value=true;ElMessage.success('Profile updated')}catch(error){ElMessage.error(error instanceof Error?error.message:'Profile update failed')}}
</script>
<style scoped>
.profile-card{max-width:900px;padding:24px;background:var(--mgs-surface);border:1px solid var(--mgs-border);border-radius:10px;box-shadow:var(--mgs-shadow)}.profile-intro{display:flex;align-items:flex-start;justify-content:space-between;gap:16px;margin-bottom:24px}.profile-intro h2{margin:0;font-size:16px}.profile-intro p{margin:5px 0 0;color:var(--mgs-muted);font-size:12px}.profile-form{display:grid;grid-template-columns:140px minmax(0,1fr);gap:28px}.profile-avatar :deep(.el-form-item__content){justify-content:center}.profile-fields{min-width:0}.form-actions{display:flex;align-items:center;justify-content:flex-end;gap:12px;margin-top:18px}.hint{margin-right:auto;color:var(--mgs-muted);font-size:12px}@media(max-width:640px){.profile-form{grid-template-columns:1fr}.profile-avatar :deep(.el-form-item__content){justify-content:flex-start}}
</style>