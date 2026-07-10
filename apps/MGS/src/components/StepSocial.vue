<template>
  <ElForm>
    <ElFormItem :label="'社交主页关联配置' + index" v-for="(item, index) in source" :key="index">
      <ElTooltip content="请上传对应平台的logo">
        <ElUpload :auto-upload="false" :limit="1" :show-file-list="false" accept=".svg" :on-change="(file: any) => handleFileChange(file, index)" action="#">
          <ElImage :src="PreviewUrls[index]" fit="cover" style="   width: 30px;   height: 30px;   display: block;   border-radius: 4px;   border: 1px dashed #d9d9d9;">
            <template #error>
              <ElIcon style="position: relative; top: 2px">
                <Plus />
              </ElIcon>
            </template>
          </ElImage>
        </ElUpload>
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
        <ElButton :icon="ArrowRight" @click="$emit('next')" type="success" />
      </ElTooltip>
    </ElFormItem>
  </ElForm>
</template>
<script setup lang="ts">
import { onBeforeUnmount, ref,type Ref } from 'vue';
import { ElForm, ElFormItem, ElButton, ElInput, ElImage, ElTooltip, ElIcon, ElUpload,} from 'element-plus'
import {Delete,Plus,ArrowLeft,ArrowRight} from '@element-plus/icons-vue'
import type { UploadFile } from 'element-plus'
import type { SubscriptionDto } from '@/api';
const PreviewUrls = ref<string[]>([])
const source:Ref<SubscriptionDto[]> = ref([
  {aliasName:'',subscriptionLink:'',subscriptionPlatform:'',subscriptionIcon:''}
])
defineEmits(['prev', 'next'])

function removeSubscription(index:number){
  source.value.splice(index,index+1)
}

function addSubscription(){
  source.value.push({subscriptionIcon:'',aliasName:'',subscriptionLink:'',subscriptionPlatform:''})
}

function handleFileChange(file: UploadFile, index: number) {
  if (file.raw) {
    PreviewUrls.value[index] = URL.createObjectURL(file.raw)
  }
}

onBeforeUnmount(()=>{

})
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
