<template>
    <ElCard shadow="hover" @click="() => $router.push(`/preview/${blog.uid}`)">
        <template #header>
            <ElCol>{{ blog.fileName }}</ElCol>
          <!-- <ElRow class="header">
          </ElRow> -->
        </template>
        <ElCol style="padding: 10px 25px;">{{ blog.fileIntroduction??demo }}</ElCol>
        <template #footer>
          <ElRow>
            <ElCol :span="14 ">
                <ElTag v-for="tag,index in blog.tags" :key="index" :type="tag.isCategory?'success':'info'">{{ tag.name }}</ElTag>
            </ElCol>
            <ElCol :span="6">{{ blog.uploadTime }}</ElCol>
            <ElCol :span="4">
                <ElIcon><View/></ElIcon>
                800
            </ElCol>            
          </ElRow>
        </template>
      </ElCard>
</template>
<script setup lang="ts">
import { View } from '@element-plus/icons-vue';
import { ElCard, ElCol, ElIcon, ElRow,ElTag } from 'element-plus';
import type { FileDto } from '@/api';
defineProps<{blog:FileDto}>()
const demo = '博文开头博文开头博文开头博文开头博文开头博文开头博文开头博文开头博文开头博文开头博文开头博文开头博文开头博文开头博文开头博文开头博文开头博文开头博文开头博文开头'
</script>