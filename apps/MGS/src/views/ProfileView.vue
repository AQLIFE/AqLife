<template>
  <ElCol class="flex">
    <ElForm>
      <ElFormItem>
        <ElTooltip content="请上传对应博客头像" style="min-width: 100%; display: block">
          <ImageUpload
            :disabled="isActive"
            iconSize="10vw"
            :src="preview()"
            v-model:file="avatarFile"
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
        <ElButton type="warning" @click="update">Update</ElButton>
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
import { ref, type Ref } from 'vue'
import { useAccountStore } from '@/stores/useAccountStore'
import ImageUpload from '@/components/ImageUpload.vue'
import { AccountApi, type AccountProfile } from '@/api'
import { apiConfiguration } from '@/services/api'
import { type ApiAccountAvatarPatchRequest } from '@aqlife/api-contract'
import { useFileStore } from '@/stores/useFileStore'

const isActive = ref(true)
const accountStore = useAccountStore()

function preview ():string {
  const avatar = accountStore.systemAccount?.avatar
  if (avatar != null) {
    if (useFileStore().previewUrl.has(avatar)) return useFileStore().previewUrl.get(avatar)!
    else
      return `${import.meta.env.VITE_API}/api/File/preview?UID=${avatar}`
  }
  return ''
}
const avatarFile: Ref<File | null> = ref<File | null>(null)
async function update() {
  const accountApi = new AccountApi(apiConfiguration)
  // console.log(avatarFile.value == null, accountStore.systemAccount)
  if (avatarFile.value) await accountApi.apiAccountAvatarPatch({ avatar: avatarFile.value as File })

  const SimpleAccountInfo: AccountProfile = {
    name: accountStore.systemAccount?.name,
    desc: accountStore.systemAccount?.desc,
  }
  await accountApi.apiAccountProfilePatch({ accountProfile: SimpleAccountInfo })
  ElMessage.info('已发起更新')
  accountStore.systemAccount = await accountApi.apiAccountGet()
  ElMessage.success('更新完成')
  // router.push('/account/profile');
  isActive.value = !isActive.value
}
</script>

<style lang="css" scoped>
.flex {
  display: flex;
  flex-direction:column;
  text-align: center;
  align-items: center;
  height:inherit;
  justify-content: center;
}
.form-item-button :deep(.el-form-item__content) {
  /* display: flex; */
  justify-content: space-between;
}
</style>
