<script setup lang="ts">
import { useTagStore } from '@/stores/tagStore';
import { computed, onBeforeMount, ref } from 'vue';
import { ElButton } from 'element-plus';
import { TagApi, type BlogCategoryStatistics } from '@/api';
import { apiConfiguration } from '@/services/api';
import { View, Timer } from '@element-plus/icons-vue';

const BlogCategoryView = ref<BlogCategoryStatistics[]>([])

const category = computed(() => useTagStore().tagList.filter(e => e.isCategory))
onBeforeMount(async () => {
    const tagApi = new TagApi(apiConfiguration)
    if (useTagStore().tagList.length == 0) useTagStore().tagList = await (await tagApi.apiTagGet()).items??[]
    if(BlogCategoryView.value.length==0) BlogCategoryView.value = await tagApi.apiTagOverviewGet()
})
</script>

<template>
    <div class="overview">

        <section class="overview-section">
            <h3 class="overview-title">
                博文概览
            </h3>

            <div v-for="item in BlogCategoryView" :key="item.uid!" class="overview-item">
                <ElButton type="info" link>
                    {{ item.categoryName }}
                </ElButton>

                <span>
                    {{ item.categoryCount }}
                </span>
            </div>
        </section>

        <section class="overview-section">
            <h3 class="overview-title">
                最多阅览
            </h3>

            <div v-for="item in category" :key="item.uid!" class="overview-item">
                <ElButton type="info" link>
                    {{ item.name }}
                </ElButton>

                <span>
                    100
                </span>
            </div>
        </section>

    </div>
</template>

<style lang="css" scoped>
.overview {
    padding: 24px 20px;
}

.overview-section {
    padding: 0 0 28px;
}

.overview-section+.overview-section {
    padding-top: 4px;
}

.overview-title {
    margin: 0 0 14px;

    color: var(--el-text-color-secondary);

    font-size: 12px;

    font-weight: 600;
}

.overview-item {
    min-height: 26px;

    display: flex;

    align-items: center;

    justify-content: space-between;

    color: var(--el-text-color-secondary);

    font-size: 12px;
}

.overview-item+.overview-item {
    margin-top: 2px;
}
</style>