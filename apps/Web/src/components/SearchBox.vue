<template>
  <div class="search-box">
    <!-- 搜索区域 -->
    <div class="search-panel">
      <ElSelect
        ref="inputRef"
        v-model="selectedValue"
        class="search-select"
        filterable
        remote
        clearable
        :remote-method="remoteSearch"
        :loading="loading"
        :debounce="350"
        :remote-show-suffix="true"
        :default-first-option="true"
        no-match-text="不存在该博文"
        :suffix-icon="Search"
        placeholder="你想搜点什么"
        @change="handleSelect"
      >
        <template #prefix>
          <span class="shortcut">
            <ElIcon>
              <component :is="webIconRegistry[keyIcon]" />
            </ElIcon>

            <span>Ctrl+Q</span>
          </span>
        </template>

        <ElOption
          v-for="item in result"
          :key="item.uid"
          :label="item.fileName!"
          :value="item.uid!"
        />
      </ElSelect>
    </div>

    <!-- 搜索历史 -->
    <section class="history-panel">
      <div class="history-header">
        <span>搜索历史</span>

        <span
          v-if="SearchHistory.length"
          class="history-count"
        >
          {{ SearchHistory.length }}
        </span>
      </div>

      <div
        v-if="SearchHistory.length"
        class="history-list"
      >
        <div
          v-for="(item, index) in SearchHistory"
          :key="item.uid ?? index"
          class="history-item"
        >
          <ElButton
            class="history-title"
            :icon="webIconRegistry[Markdown]"
            link
            @click="router.push({ path: `/preview/${item.uid}` })"
          >
            <span>{{ item.fileName }}</span>
          </ElButton>

          <div
            v-if="item.tags?.length"
            class="history-tags"
          >
            <ElTag
              v-for="tag in item.tags"
              :key="tag.uid!"
              size="small"
              :type="tag.isCategory ? 'success' : 'info'"
            >
              {{ tag.name }}
            </ElTag>
          </div>

          <ElButton
            class="delete-button"
            :icon="Delete"
            text
            @click="deleteSH(index)"
          />
        </div>
      </div>

      <div
        v-else
        class="history-empty"
      >
        还没有搜索记录哦
      </div>
    </section>
  </div>
</template>

<script lang="ts" setup>
import {
  ElButton,
  ElIcon,
  ElOption,
  ElSelect,
  ElTag,
} from 'element-plus'

import {
  Delete,
  Search,
} from '@element-plus/icons-vue'

import {
  onBeforeMount,
  onMounted,
  onUnmounted,
  ref,
} from 'vue'

import {
  FileApi,
  type FileDto,
} from '@/api'

import { apiConfiguration } from '@/services/api'

import {
  WebIconName,
  webIconRegistry,
} from '@aqlife/icons'

import { useRouter } from 'vue-router'


const router = useRouter()

const fileApi = new FileApi(apiConfiguration)

const keyIcon = WebIconName.Key
const Markdown = WebIconName.Markdown

const HISTORY_STORAGE_KEY = 'AQLIFE_SEARCH_HISTORY'

const loading = ref(false)

const selectedValue = ref<string>('')

const result = ref<FileDto[]>([])

const inputRef = ref()

const SearchHistory = ref<FileDto[]>([])


/**
 * 处理搜索结果选择
 */
function handleSelect(uid: string) {
  if (!uid) return

  const selectedItem = result.value.find(
    item => item.uid === uid,
  )

  if (!selectedItem) return

  const index = SearchHistory.value.findIndex(
    item => item.uid === uid,
  )

  if (index !== -1) {
    SearchHistory.value.splice(index, 1)
  }

  SearchHistory.value.unshift(selectedItem)

  if (SearchHistory.value.length > 20) {
    SearchHistory.value.pop()
  }
}


/**
 * 删除搜索历史
 */
function deleteSH(index: number) {
  SearchHistory.value.splice(index, 1)
}


/**
 * 远程搜索
 */
async function remoteSearch(query: string) {
  if (!query) {
    result.value = []
    return
  }

  loading.value = true

  try {
    const response = await fileApi.apiFileGet({
      title: query,
    })

    result.value = (response.items ?? []).filter(item =>
      item.fileName
        ?.toLowerCase()
        .includes(query.toLowerCase()),
    )
  } catch (error) {
    console.error('搜索文章失败:', error)
    result.value = []
  } finally {
    loading.value = false
  }
}


/**
 * 加载搜索历史
 */
function loadSearchHistory() {
  const savedHistory =
    localStorage.getItem(HISTORY_STORAGE_KEY)

  if (!savedHistory) return

  try {
    SearchHistory.value = JSON.parse(savedHistory)
  } catch (error) {
    console.error(
      '解析搜索历史失败',
      error,
    )
  }
}


/**
 * 补充搜索历史中的标签
 */
async function loadHistoryTags() {
  if (!SearchHistory.value.length) return

  await Promise.all(
    SearchHistory.value.map(async item => {
      if (!item.uid) return

      try {
        const response =
          await fileApi.apiFileGet({
            uID: item.uid,
          })

        item.tags = response.items?.[0]?.tags ?? []
      } catch (error) {
        console.error(
          '加载搜索历史标签失败',
          error,
        )
      }
    }),
  )
}


/**
 * Ctrl + Q 聚焦搜索框
 */
function handleGlobalKeydown(
  event: KeyboardEvent,
) {
  if (
    event.key.toLowerCase() === 'q' &&
    (event.ctrlKey || event.metaKey)
  ) {
    event.preventDefault()

    inputRef.value?.focus?.()
  }
}


/**
 * 持久化搜索历史
 */
function saveSearchHistory() {
  if (SearchHistory.value.length === 0) {
    localStorage.removeItem(
      HISTORY_STORAGE_KEY,
    )

    return
  }

  localStorage.setItem(
    HISTORY_STORAGE_KEY,
    JSON.stringify(SearchHistory.value),
  )
}


onBeforeMount(() => {
  loadSearchHistory()
})


onMounted(() => {
  loadHistoryTags()

  window.addEventListener(
    'keydown',
    handleGlobalKeydown,
  )
})


onUnmounted(() => {
  saveSearchHistory()

  window.removeEventListener(
    'keydown',
    handleGlobalKeydown,
  )
})
</script>


<style scoped>
.search-box {
  width: 100%;
  height: 100%;

  min-width: 0;
  min-height: 0;

  display: grid;
  grid-template-rows: auto minmax(0, 1fr);

  overflow: hidden;

  /* background: #fafafa; */
}


/* =========================
   搜索区域
   ========================= */

.search-panel {
  min-width: 0;

  padding: 14px 12px 10px;
}

.search-select {
  width: 100%;
}


/* Element Plus Select */

.search-select :deep(.el-select__wrapper) {
  min-height: 42px;

  background: var(--el-fill-color-light);

  border: 1px solid transparent;

  border-radius: 7px;

  box-shadow: none;

  transition:
    background-color 0.2s ease,
    border-color 0.2s ease;
}

.search-select :deep(.el-select__wrapper:hover) {
  background: var(--el-fill-color);
  border-color: var(--el-border-color);
}

.search-select :deep(.el-select__wrapper.is-focused) {
  background: var(--el-bg-color);
  border-color: var(--el-color-primary);
  box-shadow: 0 0 0 2px var(--el-color-primary-light-9);
}


/* Ctrl + Q */

.shortcut {
  display: inline-flex;

  align-items: center;

  gap: 3px;

  padding: 3px 5px;

  border: 1px solid var(--el-border-color-lighter);

  border-radius: 4px !important;

  background: transparent;

  color: var(--el-text-color-placeholder);

  font-size: 11px;

  line-height: 1;
}


/* =========================
   搜索历史
   ========================= */

.history-panel {
  min-width: 0;
  min-height: 0;

  display: grid;

  grid-template-rows: auto minmax(0, 1fr);

  overflow: hidden;
}


/* 标题 */

.history-header {
  height: 40px;

  padding: 0 14px;

  display: flex;

  align-items: center;

  justify-content: space-between;

  background: transparent;

  color: var(--el-text-color-secondary);

  font-size: 12px;

  font-weight: 500;
}

.history-count {
  padding: 2px 7px;

  border-radius: 10px !important;

  /* background-color: var(--back_color_lv2); */

  font-size: 11px;
}


/* 历史列表 */

.history-list {
  min-height: 0;

  padding: 4px 8px 12px;

  overflow-x: hidden;

  overflow-y: auto;
}

.history-list::-webkit-scrollbar {
  width: 4px;
}

.history-list::-webkit-scrollbar-thumb {
  background-color: var(--back_color_lv3);
}


/* 单条历史 */

.history-item {
  min-width: 0;
  border-radius: 6px;

  display: grid;

  grid-template-columns: minmax(0, 1fr) auto;

  grid-template-rows: auto auto;

  column-gap: 4px;

  padding: 8px;

  transition:
    background-color 0.15s;
}

.history-item:hover {
  background-color: var(--el-fill-color-light);
}


/* 标题 */

.history-title {
  min-width: 0;

  grid-column: 1;

  justify-content: flex-start;

  padding: 4px 0;

  margin: 0;

  overflow: hidden;

  color: inherit;
}

.history-title :deep(.el-button__text) {
  min-width: 0;

  overflow: hidden;

  text-overflow: ellipsis;

  white-space: nowrap;
}


/* 标签 */

.history-tags {
  min-width: 0;

  grid-column: 1;

  display: flex;

  gap: 4px;

  overflow: hidden;
}

.history-tags .el-tag {
  flex-shrink: 0;
}


/* 删除按钮 */

.delete-button {
  grid-column: 2;

  grid-row: 1 / 3;

  align-self: center;

  margin: 0;
}


/* 空状态 */

.history-empty {
  display: flex;

  align-items: flex-start;

  justify-content: center;

  padding-top: 24px;
  padding-bottom:24px;

  /* background: #fafafa; */
  color:#909399;

  font-size: 13px;
}
</style>