<script setup lang="ts">
import { ElCol,ElAnchorLink,ElAnchor } from 'element-plus';
import { useArticleStore } from '@/stores/articleStore';

const articleStore = useArticleStore()
</script>

<template>
    <ElCol class="toc">
        <ElCol class="catalog">
            {{ articleStore.blogTitle??'文章目录' }}
        </ElCol>
        <ElAnchor type="underline" container="#markdown-scroll-container" class="toc-list">
            <ElAnchorLink class="toc-list-item" v-for="item,index in articleStore.toc" :key="index" :href="'#' + item.anchor" >
            {{ item.title }}    
            </ElAnchorLink>
        </ElAnchor>
    </ElCol>
</template>

<style>
.el-anchor__link{
    color: black;
}

.toc{
    max-height: 100%;
    overflow-y: hidden;
}

.tov > .toc-list{
    height: inherit;
    overflow-y:scroll;
}
.toc .toc-list-item{
    padding-left: 25px;
}

.toc > .catalog{
    height: 30px;line-height: 30px;padding-left: 20px;
}
</style>