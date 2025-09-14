<template>
    <ElRow flex="column" id="skill" wrap="wrap">
        <ElDivider>
            <ElIcon>
                <SkillIcon />
            </ElIcon>
        </ElDivider>
        <ElTag>Vue</ElTag>
        <ElTag>ASP.NET Core</ElTag>
        <ElTag>Mysql</ElTag>
        <ElTag>Docker</ElTag>
        <ElTag>Linux</ElTag>
    </ElRow>
</template>

<script lang="ts" setup>
import { ElRow, ElTag, ElDivider, ElIcon } from 'element-plus';
import SkillIcon from '@/assets/icons/SkillIcon.vue';
</script>


<style scope>

#skill {
	background-color: white;
	height: auto;
	padding-bottom: 10px;
}

#skill .el-tag{margin:5px;}
</style>