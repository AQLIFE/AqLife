<script setup lang="ts">
import { ElCol,ElAnchorLink,ElAnchor } from 'element-plus';
import { useArticleStore } from '@/stores/articleStore';

const articleStore = useArticleStore()
</script>

<template>
    <ElCol>
        <ElCol style="color:lightslategray">
            {{ articleStore.blogTitle }}
        </ElCol>
        <ElAnchor type="underline" container="#mdRender">
            <ElAnchorLink style="line-height: 30px;" v-for="item,index in articleStore.toc" :key="index" :href="'#' + item.anchor" >
            {{ item.title }}    
            </ElAnchorLink>
        </ElAnchor>
    </ElCol>
</template>

<style>
.el-anchor__link{
    color: black;
}
</style>