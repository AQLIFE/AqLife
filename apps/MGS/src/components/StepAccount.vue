<template>
  <ElForm label-width="120px">
    <ElFormItem label="账户名" required>
      <ElInput clearable placeholder="请输入账户名" v-model="registerStore.profile.name"/>
    </ElFormItem>
    <ElFormItem label="说说">
      <ElInput clearable placeholder="请输入简介" v-model="registerStore.profile.desc"/>
    </ElFormItem>
    <ElFormItem label="登录密码" required>
      <ElInput clearable placeholder="请输入密码" v-model="registerStore.profile.pwd" show-password/>
    </ElFormItem>
    <ElFormItem label="记忆密码" required>
      <ElInput clearable placeholder="请再次输入密码" v-model="tmpPwd" show-password/>
    </ElFormItem>
    <ElFormItem label="系统密钥" required>
      <ElInput clearable type="textarea" placeholder="请输入用于初始化博客系统的密钥" v-model="registerStore.profile.serverKey"/>
    </ElFormItem>
    <ElFormItem>
      <ElButton type="primary" @click="next" :icon="ArrowRight"/>
    </ElFormItem>
  </ElForm>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { ElForm,ElFormItem,ElInput,ElButton, ElMessage } from 'element-plus';
import {ArrowRight} from '@element-plus/icons-vue'
import { useRegisterStore } from '@/stores/useRegisterStore';
import { validateAccountProfile } from '@aqlife/domain';

const emit = defineEmits(['next'])
const tmpPwd = ref<string>('')
const registerStore = useRegisterStore()
function next(){
  if(registerStore.profile.pwd != tmpPwd.value){ElMessage.error('两次密码不一致');return}
  const result = validateAccountProfile(registerStore.profile.name,registerStore.profile.desc,registerStore.profile.serverKey)
  if(result.ok){
    registerStore.steps[0].status = 'finish'
    ElMessage.success('账户信息验证通过')
    emit('next')
  }else{
    ElMessage.error(result.message)
  }
}
</script>
<style lang="css" scoped>
.el-form :deep(.el-form-item__label) {
  text-align: justify;
  text-align-last: justify;
  padding-right: 12px;
}
</style>
