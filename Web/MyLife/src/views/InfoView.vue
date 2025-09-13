<template>
    <ElCol>
        <ElDescriptions title="项目信息" :border="true" :column="2">
            <ElDescriptionsItem label="前端技术方案">
                <ElTag type="success">Vue3</ElTag>
                <ElTag>ElementPlus</ElTag>
                <ElTag type="success">Vue-Router</ElTag>
                <ElTag type="warning">Pinia</ElTag>
                <ElTag>TypeScript</ElTag>
            </ElDescriptionsItem>
            <ElDescriptionsItem label="前端打包工具">Vite</ElDescriptionsItem>
            <ElDescriptionsItem label="字体信息">
                <ElLink type="warning" target="_blank" href="https://unifoundry.com/unifont/index.html" underline="hover">Unifoundry</ElLink>
            </ElDescriptionsItem>
            <ElDescriptionsItem label="后端技术方案">
                <ElTag>ASP.NET WEB API</ElTag>
                <ElTag type="success">Mysql</ElTag>
            </ElDescriptionsItem>
        </ElDescriptions>
    </ElCol>
</template>

<script lang="ts" setup>
import { ElCol,ElLink, ElTag, ElDescriptions, ElDescriptionsItem } from 'element-plus';
</script>

<style scoped>
.el-tag {
    margin-right: 10px;
}
</style>