<template>
    <ElRow justify="center" align="middle">
        <ElCol :span="4">首页</ElCol>
        <ElCol :span="4">分享</ElCol>
        <ElCol :span="4">建文</ElCol>
        <ElCol :span="4">测试</ElCol>
        <ElCol :span="4">开发</ElCol>
        <ElCol :span="4">开发</ElCol>
    </ElRow>
</template>

<script lang="ts" setup>
import { ElCol, ElRow } from 'element-plus';
</script>

<style scoped>
.el-row>.el-col{height: inherit;line-height:inherit;}
</style>