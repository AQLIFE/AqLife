<template>
    <ElRow justify="center" align="middle">
        <NavigationButton 
            v-for="(item, index) in navList" 
            :key="item.path"
            :btitle="item.title" 
            :routeObj="item" 
            @click="jump" 
            :icon="item.icon" 
            :tabindex="index + 2"
        />
    </ElRow>
</template>

<script lang="ts" setup>
import { ElRow } from 'element-plus';
import {type NavItemDto} from '@/types/navigation'
import NavigationButton from '@/components/NavigationButton.vue';
import { useNavigation } from '@/composables/useNavigation';
import { useRouter } from 'vue-router';

const router = useRouter();

const { navList } = useNavigation();
const jump = (item: NavItemDto): void => {
    router.push(item.path);
};
</script>

<style scoped>
.el-row {height: inherit;line-height:inherit;}
.a{color: var(--text_color_lv1);}
</style>