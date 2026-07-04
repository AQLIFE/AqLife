<template>
  <ElRow>
    <ElCol :span="8">
      <ElImage />
    </ElCol>
    <ElCol :span="16">
      <ElCol>name</ElCol>
      <ElCol> bio</ElCol>
    </ElCol>
    <ElCol :span="24">sub1</ElCol>
    <ElCol :span="24">sub2</ElCol>
    <ElCol :span="24">sub3</ElCol>
    <ElCol>
      <ElButton @click="$emit('prev')">left</ElButton>
      <ElButton>commit</ElButton>
    </ElCol>
  </ElRow>
</template>

<script setup lang="ts">
import { ElButton,ElCol,ElImage } from 'element-plus';
defineEmits(['prev'])
</script>
