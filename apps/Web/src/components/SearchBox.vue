<template>
    <ElCard shadow="hover" class="card" title="按下Ctrl+Q快速聚焦到搜索框">

        <ElSelect v-model="selectedValue" filterable remote clearable :remote-method="remoteSearch" placeholder="你想搜点什么"
            :loading="loading" :debounce="350" :remote-show-suffix="true" no-match-text="不存在该博文"
            :default-first-option="true" :suffix-icon="Search" ref="inputRef" @change="handleSelect">
            <template #prefix>
                <ElCol style="height: 50px;line-height: 50px;">
                    <span class="key">
                        <ElIcon>
                            <component :is="webIconRegistry[keyIcon]" />
                        </ElIcon>
                        Ctrl+Q
                    </span>
                </ElCol>
            </template>
            <ElOption v-for="item in result" :key="item.uid" :label="item.fileName" :value="item.uid" />
        </ElSelect>
        <ElCol class="search-history">
            <ElCol class="header">搜索历史</ElCol>
            <ElCol class="content" v-if="SearchHistory.length >= 1">
                <ElRow v-for="item, index in SearchHistory" :key="index" style="padding: 10px;">
                    <ElCol :span="12">
                        <ElButton class="none" :icon="webIconRegistry[Markdown]" @click="router.push({path:`/preview/${item.uid}`})" link>{{ item.fileName }}</ElButton>
                    </ElCol>
                    <ElCol :span="10">
                        <ElTag v-for="tag, index in item.tags" :key="index" :type="tag.isCategory?'success':'info'" style="height:100%;">{{tag.name}}</ElTag>
                    </ElCol>
                    <ElCol :span="2">
                        <ElButton @click="deleteSH(index)" :icon="Delete"/>
                    </ElCol>
                </ElRow>
            </ElCol>
            <ElCol v-else class="content">
                还没有搜索记录哦
            </ElCol>
        </ElCol>
    </ElCard>
</template>

<script lang="ts" setup>
import { ElCol, ElCard, ElIcon, ElSelect, ElButton, ElTag, ElRow } from 'element-plus';
import { Delete, Search } from '@element-plus/icons-vue';
import { onBeforeMount, onMounted, onUnmounted, ref } from 'vue';
import { FileApi, type FileDto } from '@/api';
import { apiConfiguration } from '@/services/api';
import { WebIconName, webIconRegistry } from '@aqlife/icons'
import { useRouter } from 'vue-router';

const router = useRouter()
const keyIcon = WebIconName.Key;
const Markdown = WebIconName.Markdown;
const loading = ref(false);
const selectedValue = ref<string>(''); // 存储当前选中的 UID
const result = ref<FileDto[]>([]);
const inputRef = ref();
const SearchHistory = ref<FileDto[]>([]); // 搜索历史
const fileApi = new FileApi(apiConfiguration);

// const Preview = (uid:string|undefined)=>`/preview/${uid}`
const HISTORY_STORAGE_KEY = 'AQLIFE_SEARCH_HISTORY';

// 1. 处理选择逻辑：将结果追加到历史记录
function handleSelect(uid: string) {
    if (!uid) return;

    // 从当前搜索结果中找到对应的完整对象
    const selectedItem = result.value.find(item => item.uid === uid);

    if (selectedItem) {
        // 去重逻辑：如果已存在则先移除，再插入到最前面（保证时间顺序）
        const index = SearchHistory.value.findIndex(h => h.uid === uid);
        if (index !== -1) {
            SearchHistory.value.splice(index, 1);
        }
        SearchHistory.value.unshift(selectedItem);

        // 限制历史记录上限，例如只保留最新的 20 条
        if (SearchHistory.value.length > 20) {
            SearchHistory.value.pop();
        }
    }
}
function deleteSH(index:number){
    SearchHistory.value.splice(index,1)
}
async function remoteSearch(query: string) {
    if (query) {
        loading.value = true;
        try {
            // 修正：搜索应使用 query 参数而非选中的 v-model 值
            const response = await fileApi.apiFileGet({ title: query });
            result.value = response.filter(item =>
                item.fileName?.toLowerCase().includes(query.toLowerCase())
            );
        } finally {
            loading.value = false;
        }
    } else {
        result.value = [];
    }
}
onMounted(() => {
    // 2. 初始化时：从本地存储读取历史记录
    const savedHistory = localStorage.getItem(HISTORY_STORAGE_KEY);
    if (savedHistory) {
        try {
            SearchHistory.value = JSON.parse(savedHistory);
        } catch (e) {
            console.error('解析搜索历史失败', e);
        }
    }

    window.addEventListener('keydown', (e) => {
        if (e.key === 'q' && (e.metaKey || e.ctrlKey)) {
            e.preventDefault();
            inputRef.value.focus();
        }
    });
});
onBeforeMount(()=>{
    if(SearchHistory.value.length>=1)
{
    SearchHistory.value.forEach(async item => {
        item.tags =( await fileApi.apiFileGet({uID:item.uid}))[0].tags
        console.log(item.tags)
    });
}
})
// 3. 组件卸载时：持久化写入 LocalStorage
onUnmounted(() => {
    if (SearchHistory.value.length > 0) {
        // 由于在 handleSelect 中已经去重，这里直接序列化存储
        localStorage.setItem(HISTORY_STORAGE_KEY, JSON.stringify(SearchHistory.value));
    }
});
</script>

<style scoped>

.search-history{
    background-color: var(--back_color_lv1);
}
.search-history>.header {
    display: flex;
    justify-content: center;
}
.search-history >.content{
    padding: 5px 20px;
}

.el-input {
    height: 6vh;
    line-height: 6vh;
}

.el-input :deep(.el-input__wrapper) {
    border: 0px none;
    box-shadow: none !important;
    /* background-color: var(--back_color_lv1) !important; */
}

.el-input :depp(.el-input__wrapper)>* {
    /* background-color: var(--back_color_lv1); */
    padding-left: 1vw;
}

.el-input :deep(.el-input__wrapper:hover) {
    box-shadow: none !important;
    border: none;
}

.el-input :deep(.el-input__wrapper:focus) {
    box-shadow: none !important;
    border: none;
}

.el-input :deep(.el-input-group__append) {
    background-color: white !important;
    box-shadow: none !important;
    ;
}

.card {
    border: 1px solid transparent;
    transition: box-shadow 0.2s, border-color 0.2s;
}

.card:hover,
.el-input__wrapper :deep(.el-input__inner:focus) {
    border: 1px solid rgb(78, 142, 47);
    transition: box-shadow 0.2s, border-color 0.2s;
}

.key {
    border: 1px dashed var(--back_color_lv2);
    border-radius: 25px !important;
    padding: 5px 10px;
    background-color: white;
}
</style>