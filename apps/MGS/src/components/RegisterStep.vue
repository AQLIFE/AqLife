<template>
  <ElSteps simple>
    <ElStep  v-for="item,index in useRegisterStore().steps" :key="index" :title="item.title" :icon="item.icon" :status="item.status as 'wait' | 'process' | 'finish' | 'error' | 'success'"/>
    <!-- <ElStep title="设置头像" :icon="Picture"/> -->
    <!-- <ElStep title="添加关联个人主页" :icon="Share"/> -->
  </ElSteps>
</template>

<script setup lang="ts">
import { ElStep, ElSteps } from 'element-plus'
import { useRegisterStore } from '@/stores/useRegisterStore'
// const steps= useRegisterStore().steps
</script>
