<script setup lang="ts">
import { useTagStore } from '@/stores/tagStore';
import { computed, onBeforeMount } from 'vue';
import { ElCol,ElCard,ElTag,ElText,ElDivider } from 'element-plus';
import { TagApi } from '@/api';
import { apiConfiguration } from '@/services/api';

const category = computed(()=>useTagStore().tagList.filter(e=>e.isCategory))
onBeforeMount(async()=>{
    const tagApi = new TagApi(apiConfiguration)
    if(useTagStore().tagList.length==0)useTagStore().tagList = await tagApi.apiTagGet()
})
</script>

<template>
    <ElCard shadow="hover" style="padding: 20px;">
        <ElText>博文概览</ElText>
        <ElDivider/>
        <ElCol v-for="item,index in category" :key="index">
            <ElTag type="success">{{ item.name }}</ElTag>
            <ElText>100</ElText>
        </ElCol>       
    </ElCard>
    <ElCard style="padding: 20px;" shadow="hover">
         <ElText>最新发布</ElText>
        <ElDivider/>
        <ElCol v-for="item,index in category" :key="index" >
            <ElTag type="success">{{ item.name }}</ElTag>
            <ElText>100</ElText>
        </ElCol>
    </ElCard>
</template>