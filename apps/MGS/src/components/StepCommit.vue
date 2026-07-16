<template>
  <ElRow>
    <ElCol :span="8">
      <ElImage :src="registerStore.PreviewUrls.get(registerStore.avatar!.name)"/>
    </ElCol>
    <ElCol :span="16">
      <ElCol>{{ registerStore.profile.name }}</ElCol>
      <ElCol>{{ registerStore.profile.desc}}</ElCol>
    </ElCol>
    <ElCol :span="24">
      <ElCol v-for="(item,index) in registerStore.subscriptions" :key="index" style="display: flex;flex-direction:row;">
        <ElImage :src="registerStore.PreviewUrls.get(registerStore.fileList[index]?.name)" style="width: 30px; height: 30px;"/>
        <ElCol :span="6">{{ item.subscriptionPlatform }}</ElCol>
        <ElCol :span="6">{{ item.aliasName }}</ElCol>
        <ElCol :span="6">{{ item.subscriptionLink }}</ElCol>
      </ElCol>
    </ElCol>
    <ElCol>
      <ElButton @click="$emit('prev')" :icon="ArrowLeft"/>
      <ElButton @click="commit">commit</ElButton>
    </ElCol>
  </ElRow>
</template>

<script setup lang="ts">
import { ElButton, ElCol, ElImage, ElMessage } from 'element-plus'
import {useRegisterStore} from '@/stores/useRegisterStore'
import { ArrowLeft } from '@element-plus/icons-vue';
import { apiConfiguration } from '@/services/api';
import { AccountApi, FileApi } from '@/api';
import { useAccountStore } from '@/stores/useAccountStore';
const registerStore = useRegisterStore()
const accountStore = useAccountStore()
defineEmits(['prev'])
async function commit(){
  const accountApi = new AccountApi(apiConfiguration)
  const fileApi = new FileApi(apiConfiguration)
  const randomName =await accountApi.apiAccountPost({createAccountCommand:registerStore.profile})// 注册后获得随机账户名
  const loginToken = await accountApi.apiAccountLoginPost({loginCommand:{accountName:randomName,secretKey:registerStore.profile.pwd}})
  accountStore.setToken(loginToken)
  await accountApi.apiAccountAvatarPatch({avatar:registerStore.avatar as Blob})
  const gids = await fileApi.apiFileUploadPost({file:registerStore.fileList})
  registerStore.subscriptions.forEach((element,index) => {
    element.subscriptionIcon = gids[index]
  });
  await accountApi.apiAccountSubscriptionsPut({subscriptionDto:registerStore.subscriptions})

  ElMessage.success('提交成功')
  accountStore.systemAccount = await accountApi.apiAccountGet()
  console.log(accountStore.systemAccount.name)

}
</script>
