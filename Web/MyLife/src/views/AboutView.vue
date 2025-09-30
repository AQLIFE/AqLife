<template>
    <ElCol>
        <ElDescriptions title="项目信息" :border="true" :column="2">
            <ElDescriptionsItem label="前端技术方案">
                <ElTag :type="skillManager.getSkillColor(Skills[0])">Vue3</ElTag>
                <ElTag>ElementPlus</ElTag>
                <ElTag type="success">Vue-Router</ElTag>
                <ElTag type="warning">Pinia</ElTag>
                <ElTag>TypeScript</ElTag>
            </ElDescriptionsItem>
            <ElDescriptionsItem label="前端打包工具">
                <ElTag :type="skillManager.getSkillColor(Skills[1])">Vite</ElTag>
            </ElDescriptionsItem>
            <ElDescriptionsItem label="字体信息">
                <ElLink type="warning" target="_blank" :title="lisce" href="https://unifoundry.com/unifont/index.html" underline="hover">Unifoundry</ElLink>
            </ElDescriptionsItem>
            <ElDescriptionsItem label="后端技术方案">
                <ElTag>ASP.NET WEB API</ElTag>
                <ElTag :type="skillManager.getSkillColor('Mysql')">Mysql</ElTag>
            </ElDescriptionsItem>
        </ElDescriptions>
    </ElCol>
</template>

<script lang="ts" setup>
import { skillManager,Skills } from '@/data/SkillTagData';
import { ElCol,ElLink, ElTag, ElDescriptions, ElDescriptionsItem } from 'element-plus';
const lisce = 'GNU Unifont 是 GNU 项目的一部分。此页面包含最新版本 GNU Unifont，每个可打印代码点都有字形 在 Unicode 基本多语言平面 （BMP） 中。BMP 占据 Unicode 空间的前 65,536 个代码点， 表示为 U+0000..U+FFFF。对 补充多语言平面 （SMP），在 U+010000..U+01FFFF 范围内， 以及 Michael Everson 的 ConScript Unicode Registry （CSUR） 丽贝卡·贝当古 （Rebecca Bettencourt） 的 Under-CSUR 补充。'
</script>

<style scoped>
.el-tag {
    margin-right: 10px;
}
</style>