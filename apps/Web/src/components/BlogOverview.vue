<script setup lang="ts">
import { useTagStore } from '@/stores/tagStore';
import { computed, onBeforeMount } from 'vue';
import { ElText, ElDivider, ElButton } from 'element-plus';
import { TagApi } from '@/api';
import { apiConfiguration } from '@/services/api';
import { View,Timer } from '@element-plus/icons-vue';

const category = computed(() => useTagStore().tagList.filter(e => e.isCategory))
onBeforeMount(async () => {
    const tagApi = new TagApi(apiConfiguration)
    if (useTagStore().tagList.length == 0) useTagStore().tagList = await tagApi.apiTagGet()
})
</script>

<template>
    <div>
        <div class="node">
            <ElText>博文概览</ElText>
            <ElDivider />
            <div v-for="item, index in category" :key="index" class="node-item">
                <ElButton type="info" link>{{ item.name }}</ElButton>
                <div><ElText>100</ElText></div>
            </div>
        </div>
        <div class="node">
            <ElText>最新发布</ElText>
            <ElDivider />
            <div v-for="item, index in category" :key="index" class="node-item">
                <ElButton  link>{{ item.name }}</ElButton>
                <div>
                    <ElIcon><Timer/></ElIcon>
                    <ElText>2026-09-11</ElText>
                </div>
            </div>
        </div>
        <div class="node">
            <ElText>最多阅览</ElText>
            <ElDivider />
            <div v-for="item, index in category" :key="index" class="node-item">
                <ElButton  link>{{ item.name }}</ElButton>
                <div>
                    <ElIcon><View/></ElIcon>
                    <ElText>8000+</ElText>
                </div>
            </div>
        </div>
    </div>
</template>

<style lang="css" scoped>
.node {
    padding: 20px;
}

.node .node-item{
    display: flex;
    justify-content: space-between;
}
</style>