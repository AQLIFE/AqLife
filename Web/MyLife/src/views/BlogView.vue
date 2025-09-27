<script setup lang="ts">
import { ElCol, ElRow } from 'element-plus';
import { ref, onMounted, reactive } from 'vue';
import { marked } from 'marked';
import mermaid from 'mermaid';


const updateTime = '2024-06-20';
const description = '文章内容仅供参考，如有错误，欢迎指正。';

import mdContentRaw from '@/assets/NET Core 开发要点.md?raw'; // Vite 支持 ?raw 导入文本


onMounted( async() => {
    mermaid.initialize({ startOnLoad: false });
    await mermaid.run({
        querySelector: '.language-mermaid',
    });
});
</script>

<template>
    <!-- <ElSkeleton>
        <template #template>

        </template>
</ElSkeleton> -->

    <ElCol>
        <ElCol class="blogTitle">Title=></ElCol>
        <ElCol class="description">
            <ElRow>
                <ElCol>{{ description }}</ElCol>
                <ElCol>Update:{{ updateTime }}</ElCol>
            </ElRow>
        </ElCol>
        <ElCol id="blogContent" v-html="marked(mdContentRaw)" />
    </ElCol>
</template>


<style scoped>
.blogTitle {
    font-size: 2rem;
    font-weight: bold;
    margin: 2vh 0;
    text-align: center;
}

.description {
    font-size: 1rem;
    text-align: center;
}

.description>.el-row {
    justify-content: center;
    height: auto;
    font-size: small;
    color: var(--back_color_lv4);
    background-color: var(--back_color_lv2);
}

#blogContent {
    margin: 4vh 8vw;
    line-height: 2rem;
    font-size: 1.2rem;
    height: 100vh !important;
}
</style>