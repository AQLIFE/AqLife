<script setup lang="ts">
import { DevPlan } from '@/data/DevPlanData';
import { ElRow } from 'element-plus';
const DevPlanTitle :string = "开发计划";


</script>

<template>
    <ElCol id="title">{{ DevPlanTitle }}</ElCol>
    <ElCol id="contents">
        <ElRow flex="column">
            <ElCol v-for="item,index in DevPlan" v-bind:key="index" class="content-item">
                <span>{{ index+1 }}</span>
                <span>{{ item.content }}</span>
            </ElCol>
        </ElRow>
    </ElCol>
</template>

<style scoped>



#title{
    background-color: var(--topColor);
    height: var(--inlineCenter);
    line-height: var(--inlineCenter);
}
#contents .el-row{height: auto;}
.content-item{
    height: 30px;line-height: 30px;
    text-align:left;
    background-color: var(--back_color_lv1);}

.content-item > span:nth-child(1){text-align: center;width: 30px;display: inline-block;}
</style>