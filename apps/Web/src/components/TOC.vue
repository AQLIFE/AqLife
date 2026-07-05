<script setup lang="ts">
import { ElCol,ElAnchorLink,ElAnchor } from 'element-plus';
import { useBlogStore } from '@/stores/useBlogStore'

const blogStore = useBlogStore()
</script>

<template>
    <ElCol>
        <ElCol style="color:lightslategray">
            {{ blogStore.blogTitle }}
        </ElCol>
        <ElAnchor>
            <!-- <ElAnchorLink v-for="item in blog..filter(i=>i.level<=3)" :key="item.id" :href="'#' + item.id" :title="item.title" /> -->
        </ElAnchor>
    </ElCol>
</template>

<style>
.el-anchor__link{
    color: black;
}
</style>