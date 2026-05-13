<template>
    <ElRow flex="column">
        <ElCol id="notfonud">0000 0001 1001 0100</ElCol>
        <ElCol>这是一个不存在的路径,请返回</ElCol>
    </ElRow>
</template>
<script lang="ts" setup>
import {ElRow,ElCol} from 'element-plus';
</script>

<style scope>
.el-row{
    height: calc(100vh - 120px);
    justify-content: center;
    align-content: center;
    text-align: center;
}
.el-row>.el-col{height: auto;}
#notfonud{ font-size:3rem;}
</style>
