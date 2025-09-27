<script setup lang="ts">
import { ElCol,ElAnchorLink,ElAnchor } from 'element-plus';
import { Blog } from '@/services/storer';
const blog = Blog();
</script>

<template>
    <ElCol>
        <ElCol>
            {{ blog.blogTitle }}
        </ElCol>
        <ElAnchor>
            <ElAnchorLink v-for="item in blog.anchorList.filter(i=>i.level<=3)" :key="item.id" :href="'#' + item.id" :title="item.title" />
        </ElAnchor>
    </ElCol>
</template>