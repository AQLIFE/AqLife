<script setup lang="ts">
import ShareCard from '@/components/ShareCard.vue';
import { ElRow,ElCol } from 'element-plus';

const links=[
    'https://www.phind.com/assets/images/phind.svg',
    'https://registry.npmmirror.com/@lobehub/icons-static-png/latest/files/light/copilot-text.png'
]
</script>

<template>
    <ElCol>Share: 与你同享</ElCol>
    <ElRow>
        <ElCol :span="8"  v-for="item,index in links" :key="index">
            <ShareCard :src="item"/>
        </ElCol>
    </ElRow>
</template>