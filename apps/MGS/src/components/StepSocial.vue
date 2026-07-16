<template>
  <ElForm>
    <ElFormItem :label="'社交主页关联配置' + index" v-for="(item, index) in registerStore.subscriptions" :key="index" required>
      <ElTooltip content="请上传对应平台的logo">
        <ImageUpload v-model:file="registerStore.fileList[index]" :src="registerStore.PreviewUrls.get(registerStore.fileList[index]?.name)" @change="handleFileChange" iconSize="30px"/>
      </ElTooltip>

      <ElTooltip content="请填写对应平台的社交账户名称">
        <ElInput clearable placeholder="平台账户名" v-model="item.aliasName" />
      </ElTooltip>

      <ElTooltip content="请填写对应的平台名称">
        <ElInput clearable placeholder="平台名" v-model="item.subscriptionPlatform" />
      </ElTooltip>

      <ElTooltip content="请填写对应平台的个人社交主页链接(仅允许https://前缀)">
        <ElInput
          type="url"
          clearable
          placeholder="平台个人主页链接"
          v-model="item.subscriptionLink"
        >
          <template #prepend>Https://</template>
        </ElInput>
      </ElTooltip>

      <ElTooltip content="点击此处即删除该项配置">
        <ElButton @click="removeSubscription(index)" :icon="Delete" type="danger" />
      </ElTooltip>
    </ElFormItem>

    <ElFormItem>
      <ElTooltip content="点击返回上一步骤">
        <ElButton :icon="ArrowLeft" @click="$emit('prev')" type="info"/>
      </ElTooltip>
      <ElTooltip content="点击添加其他社交平台配置">
        <ElButton :icon="Plus" @click="addSubscription" />
      </ElTooltip>

      <ElTooltip content="最后确认">
        <ElButton :icon="ArrowRight" @click="next" type="success" />
      </ElTooltip>
    </ElFormItem>
  </ElForm>
</template>
<script setup lang="ts">
import { ElForm, ElFormItem, ElButton, ElInput, ElImage, ElTooltip, ElIcon, ElUpload, ElMessage,} from 'element-plus'
import {Delete,Plus,ArrowLeft,ArrowRight} from '@element-plus/icons-vue'
import type { UploadFile } from 'element-plus'
import ImageUpload from './ImageUpload.vue';
import { useRegisterStore } from '@/stores/useRegisterStore';
import { validateSubscriptionList } from '@aqlife/domain';


const emit =defineEmits(['prev', 'next'])
const registerStore = useRegisterStore()

function removeSubscription(index:number){
  registerStore.subscriptions.splice(index,1)
  registerStore.PreviewUrls.delete(registerStore.fileList[index]?.name)
  registerStore.fileList.splice(index,1)
}

function addSubscription(){
  registerStore.subscriptions.push({subscriptionIcon:'',aliasName:'',subscriptionLink:'',subscriptionPlatform:''})
}

function handleFileChange(file: UploadFile) {
  if (file.raw) {
    registerStore.PreviewUrls.set(file.raw.name, URL.createObjectURL(file.raw))
    console.log('Avatar file changed:', file.name, 'Preview URL:', registerStore.PreviewUrls.get(file.name))
  }
}

function next(){
  const result = validateSubscriptionList(registerStore.subscriptions)
  if(result.ok){
    ElMessage.success('社交主页关联配置验证成功')
    emit('next')
  }else{
    ElMessage.error('社交主页关联配置验证失败: ' + result.message)
  }
}
</script>
<style lang="css" scoped>
.el-form{
  display: flex;
  flex-direction: column;
  flex-wrap: wrap;
  width: 100% !important;
}

.el-form .el-form-item__content{
  display: flex;
  flex-direction: row;
  flex-wrap: nowrap;
  flex-grow: 1;
}
.el-input{width: fit-content;display: flexbox;}
</style>
