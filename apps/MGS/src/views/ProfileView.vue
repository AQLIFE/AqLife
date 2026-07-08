<template>
  <ElCol>
    <ElForm class="form">
      <ElFormItem>
        <ElTooltip content="请上传对应博客头像" style="min-width: 100%; display: block">
          <ImageUpload
            :status="isActive"
            iconSize="10vw"
            :url="preview"
            v-model:model-value="avatarFile"
          />
        </ElTooltip>
      </ElFormItem>
      <ElFormItem>
        <ElInput :disabled="isActive" v-model="accountStore.systemAccount!.name" />
      </ElFormItem>
      <ElFormItem>
        <ElInput type="textarea" :disabled="isActive" v-model="accountStore.systemAccount!.desc" />
      </ElFormItem>
      <ElFormItem class="form-item-button">
        <ElButton @click="isActive = !isActive" :title="isActive ? '解锁' : '锁定'">
          <ElIcon><component :is="isActive ? Unlock : Lock" /></ElIcon>
        </ElButton>
        <ElButton type="warning" @click="async()=>await update()">Update</ElButton>
      </ElFormItem>
    </ElForm>
  </ElCol>
</template>
<script setup lang="ts">
import {
  ElForm,
  ElFormItem,
  ElImage,
  ElIcon,
  ElUpload,
  ElTooltip,
  ElInput,
  ElButton,
  ElMessage,
} from 'element-plus'
import { Plus, Lock, Unlock } from '@element-plus/icons-vue'
import { ref,type Ref } from 'vue'
import { useAccountStore } from '@/stores/useAccountStore'
import ImageUpload  from '@/components/ImageUpload.vue'
import { AccountApi, type ISimpleAccountInfo } from '@/api'
import { apiConfiguration } from '@/services/api'
import {type ApiAccountAvatarPatchRequest} from '@aqlife/api-contract'
// import type { File } from 'node:buffer'
// import {router } from '@/router'

const isActive = ref(true)
const accountStore = useAccountStore()
const preview: string =
  accountStore.systemAccount?.avatar != null
    ? `${import.meta.env.VITE_API}/api/File/preview?UID=${accountStore.systemAccount!.avatar}`
    : ''

    const avatarFile:Ref<File | null> = ref<File | null>(null)
async function update(){
  const accountApi = new AccountApi(apiConfiguration)
  console.log(avatarFile.value == null,accountStore.systemAccount)
  if(avatarFile.value) await accountApi.apiAccountAvatarPatch({avatar:avatarFile.value as File})

  const SimpleAccountInfo:ISimpleAccountInfo ={
    name:accountStore.systemAccount?.name,
    desc:accountStore.systemAccount?.desc
  }
  await accountApi.apiAccountProfilePatch({iSimpleAccountInfo:SimpleAccountInfo})
  ElMessage.info('已发起更新')
  accountStore.systemAccount = await accountApi.apiAccountGet()
  ElMessage.success('更新完成')
  // router.push('/account/profile');
}
</script>

<style lang="css" scoped>
.el-col {
  display: flex;
  text-align: center;
  justify-content: center;
}
.form-item-button :deep(.el-form-item__content) {
  /* display: flex; */
  justify-content: space-between;
}
</style>
