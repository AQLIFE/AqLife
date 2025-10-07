<template>
    <ElRow justify="center" align="middle">
        <NavigationButton v-for="item,index in NavData" :btitle="item.NavTitle" :routeObj="item.NavObj" @click="jump" :icon="item.NavIcon" v-bind:key="index" :tabindex="index+2"/>
    </ElRow>
</template>

<script lang="ts" setup>
import { ElRow } from 'element-plus';
import NavigationButton from './NavigationButton.vue';
import { NavData } from '@/data/NavData';
import type { RouteRecordRaw } from 'vue-router';
import { useRouter } from 'vue-router';

const router = useRouter();

const jump = (obj: RouteRecordRaw): void => {
    router.push(obj.path);
};

</script>

<style scoped>
.el-row {height: inherit;line-height:inherit;}
.a{color: var(--text_color_lv1);}
</style>