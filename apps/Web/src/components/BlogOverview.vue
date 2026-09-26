<script setup lang="ts">
import { useTagStore } from '@/stores/tagStore';
import { onBeforeMount, ref } from 'vue';
import { ElButton } from 'element-plus';
import { TagApi,FileApi, type BlogCategoryStatistics, type FileDto } from '@/api';
import { apiConfiguration } from '@/services/api';
// import { View, Timer } from '@element-plus/icons-vue';
import { useBlogStore } from '../stores/fileStore';

const BlogCategoryView = ref<BlogCategoryStatistics[]>([])

// const category = computed(() => useTagStore().tagList.filter(e => e.isCategory))
const hotList = ref<FileDto[]>()
const fileApi = new FileApi(apiConfiguration)
const fileStore = useBlogStore()
async function handleFilter(tagId:string){
    // if(tagId!=undefine)return 
    fileStore.clearBlogList()
    fileStore.reset(tagId || undefined)
    const result = await fileApi.apiFileGet({categoryUID:tagId})

    if(result.items)fileStore.setBlogList(result.items)
}

onBeforeMount(async () => {
    const tagApi = new TagApi(apiConfiguration)
    if (useTagStore().tagList.length == 0) useTagStore().tagList = await (await tagApi.apiTagGet()).items??[]
    if(BlogCategoryView.value.length==0) BlogCategoryView.value = await tagApi.apiTagOverviewGet()
    const list = await fileApi.apiFileGet({order:2,pageSize:5})
    if( list.items) hotList.value = list.items
    BlogCategoryView.value.push({uid:'',categoryName:'ALL',categoryCount:fileStore.cacheBlogList.length})// 用于清除 filter的效果
})
</script>

<template>
    <div class="overview">

        <section class="overview-section">
            <h3 class="overview-title">
                博文概览
            </h3>

            <div v-for="item in BlogCategoryView" :key="item.uid!" class="overview-item">
                <ElButton type="info" link @click="handleFilter(item.uid??'')">
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

            <div v-for="item in hotList" :key="item.uid!" class="overview-item">
                <ElButton type="info" link @click="$router.push({ path: `/preview/${item.uid}` })">
                    {{ item.fileName }}
                </ElButton>

                <span>
                    {{ item.viewCount }}
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