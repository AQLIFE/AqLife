<script setup lang="ts">
import { ElCol,ElRow } from 'element-plus';
import { ArrowLeft } from '@element-plus/icons-vue';
</script>

<template>
   <ElRow>
    <ElCol :span="24">
      <ElPageHeader :icon="ArrowLeft"></ElPageHeader>
    </ElCol>
    <ElCol :span="12">
      <input/>
    </ElCol>
    <ElCol :span="12">

    </ElCol>
    </ElRow>
</template>
