<template>
    <ElCol>
        <ElDescriptions title="项目信息" :border="true" :column="2">
            <ElDescriptionsItem label="前端组成">
                <ElTag @type="skillManager.getSkillColor(Skills[0])">Vue3</ElTag>
                <ElTag>ElementPlus</ElTag>
                <ElTag type="success">Vue-Router</ElTag>
                <ElTag type="warning">Pinia</ElTag>
                <ElTag>TypeScript</ElTag>
            </ElDescriptionsItem>
            <ElDescriptionsItem label="前端打包工具">
                <ElTag @type="skillManager.getSkillColor(Skills[1])">Vite</ElTag>
            </ElDescriptionsItem>
            <ElDescriptionsItem label="字体信息">
                <ElLink type="warning" target="_blank" title="导航到所属组织" href="https://unifoundry.com/unifont/index.html" underline="hover">Unifoundry</ElLink>
                <span>&nbsp;&nbsp;</span>
                <ElLink type="warning" target="_blank" title="导航到所属组织" href="https://github.com/microsoft/cascadia-code" underline="hover">Cascadia PL</ElLink>
            </ElDescriptionsItem>
            <ElDescriptionsItem label="后端组成">
                <ElTag>ASP.NET WEB API</ElTag>
                <ElTag @type="skillManager.getSkillColor('Mysql')">Mysql</ElTag>
            </ElDescriptionsItem>
        </ElDescriptions>
    </ElCol>
</template>

<script lang="ts" setup>
import { skillManager,Skills } from '@/data/SkillTagData';
import { ElCol,ElLink, ElTag, ElDescriptions, ElDescriptionsItem } from 'element-plus';

</script>

<style scoped>
.el-tag {
    margin-right: 10px;
}
</style>