<script setup lang="ts">
import { useTagStore } from '@/stores/tagStore';
import { onBeforeMount, ref, watch } from 'vue';
import { useRoute } from 'vue-router';
import { ElButton } from 'element-plus';
import { FileOrder, FileScope, TagApi, CategoryApi, FileApi, CategoryQueryType, type CategoryDto, type FileDto } from '@/api';
import { apiConfiguration } from '@/services/api';
// import { View, Timer } from '@element-plus/icons-vue';
import { useBlogStore } from '../stores/fileStore';

const BlogCategoryView = ref<CategoryDto[]>([])

// const category = computed(() => useTagStore().tagList.filter(e => e.isCategory))
const hotList = ref<FileDto[]>()
const fileApi = new FileApi(apiConfiguration)
const fileStore = useBlogStore()
const route = useRoute()
async function loadAllPublishedBlogCount(): Promise<number> {
    let page = 1
    const pageSize = 100
    let count = 0

    while (true) {
        const result = await fileApi.apiFileGet({
            scope: FileScope.Blog,
            page,
            pageSize,
        })

        count += result.items?.length ?? 0
        if (!result.hasMore) break
        page = (result.page ?? page) + 1
    }

    return count
}

async function handleFilter(tagId: string) {
    fileStore.clearBlogList()
    fileStore.reset(tagId || undefined)
    const result = await fileApi.apiFileGet({
        scope: FileScope.Blog,
        categoryUID: tagId || undefined,
    })

    fileStore.setBlogList(result.items ?? [])
    fileStore.page = (result.page ?? 1) + 1
    fileStore.hasMore = result.hasMore ?? false
}

onBeforeMount(async () => {
    const tagApi = new TagApi(apiConfiguration)
    const categoryApi = new CategoryApi(apiConfiguration)
    const tagStore = useTagStore()

    if (tagStore.tagList.length === 0) {
        tagStore.tagList = (await tagApi.apiTagGet()).items ?? []
    }

    const [categories, allPublishedBlogCount, popular] = await Promise.all([
        categoryApi.apiCategoryGet({ searchType: CategoryQueryType.File }),
        loadAllPublishedBlogCount(),
        fileApi.apiFileGet({ scope: FileScope.Blog, order: FileOrder.MostViewed, pageSize: 5 }),
    ])

    BlogCategoryView.value = [
        ...(categories.items ?? []),
        { uid: '', categoryName: 'ALL', categoryCount: allPublishedBlogCount },
    ]
    hotList.value = popular.items ?? []
})

watch(
    () => route.path,
    async (path, previousPath) => {
        if (previousPath?.startsWith('/preview/') && path === '/blog') {
            const result = await fileApi.apiFileGet({
                scope: FileScope.Blog,
                order: FileOrder.MostViewed,
                pageSize: 5,
            })
            hotList.value = result.items ?? []
        }
    },
)
</script>

<template>
    <div class="overview">

        <section class="overview-section">
            <h3 class="overview-title">
                博文概览
            </h3>

            <div v-for="item in BlogCategoryView" :key="item.uid!" class="overview-item">
                <ElButton type="info" link @click="handleFilter(item.uid ?? '')">
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