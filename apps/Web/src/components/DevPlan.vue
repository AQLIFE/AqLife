<script setup lang="ts">
import { TodoApi } from '@/api';
// import { devPlanTasks } from '@/data/devPlanTasks'
import { apiConfiguration } from '@/services/api';
import { useTodoStore } from '@/stores/useTodoStore';
import { Plus } from '@element-plus/icons-vue';
import { ElButton, ElRow,ElCol } from 'element-plus'
import { onBeforeMount,ref } from 'vue';
const todoStore = useTodoStore()
const devPlanTitle = '开发计划'
const todoApi = new TodoApi(apiConfiguration)
onBeforeMount(async ()=>{
   if(todoStore.todoList.length==0)todoStore.todoList = await todoApi.apiTodoGet()
})

const isExtend = ref<boolean>(false)

</script>

<template>
    <ElCol id="title">{{ devPlanTitle }}</ElCol>
    <ElCol id="contents">
        <ElRow flex="column">
            <ElCol v-for="item,index in todoStore.todoList" :key="index" class="content-item">
                <ElCol :span="4" class="serial">{{ index+1 }}</ElCol>
                <ElCol :span="18">{{ item.desc }}</ElCol>
                <ElCol :span="2" v-if="item.hasChildren" class="serial"><ElButton :icon="Plus" link @click="isExtend=!isExtend"/></ElCol>
            </ElCol>
            
        </ElRow>
    </ElCol>
</template>

<style scoped>
#title{
    text-align: center;
    background-color: var(--topColor);
    height: var(--inlineCenter);
    line-height: var(--inlineCenter);
}
#contents .el-row{height: auto;}
.content-item{
    display: flex;padding: 10px;border-bottom: 1px dotted var(--back_color_lv3);
    background-color: var(--back_color_lv1);}


.content-item > .serial{display: flex;justify-content: center;align-items: center;}
.content-item > :deep(.el-col > .el-button){margin: 5%;}
</style>