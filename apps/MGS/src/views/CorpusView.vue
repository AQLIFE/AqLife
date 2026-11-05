<script setup lang="ts">
import { computed, onMounted, reactive, ref } from 'vue'
import {
  ElButton, ElDialog, ElForm, ElFormItem, ElInput, ElMessageBox,
  ElOption, ElSelect, ElTable, ElTableColumn, ElTag,
} from 'element-plus'
import { Delete, Plus, Refresh, Search } from '@element-plus/icons-vue'
import { notify } from '@aqlife/ui-shared'
import { CorpusApi, type CorpusDto } from '@/api'
import { apiConfiguration } from '@/services/api'
import MgsPageHeader from '@/components/ui/MgsPageHeader.vue'
import MgsToolbar from '@/components/ui/MgsToolbar.vue'
import MgsPageContainer from '@/components/ui/MgsPageContainer.vue'
import MgsPageState from '@/components/ui/MgsPageState.vue'

const api = new CorpusApi(apiConfiguration)
const items = ref<CorpusDto[]>([])
const keyword = ref('')
const appliedKeyword = ref('')
const page = ref(1)
const pageSize = ref(10)
const hasMore = ref(false)
const loading = ref(false)
const saving = ref(false)
const dialogVisible = ref(false)
const form = reactive({ content: '' })

const pageLabel = computed(() => `第 ${page.value} 页`)

async function loadCorpora() {
  loading.value = true
  try {
    const result = await api.apiCorpusGet({
      content: appliedKeyword.value.trim() || undefined,
      page: page.value,
      pageSize: pageSize.value,
    })
    items.value = result.items ?? []
    hasMore.value = result.hasMore ?? false
  } catch (error) {
    notify.fromError(error, '语录加载失败')
  } finally {
    loading.value = false
  }
}

function search() {
  appliedKeyword.value = keyword.value
  page.value = 1
  void loadCorpora()
}

function clearSearch() {
  keyword.value = ''
  appliedKeyword.value = ''
  page.value = 1
  void loadCorpora()
}

function changePageSize(value: number) {
  pageSize.value = value
  page.value = 1
  void loadCorpora()
}

function openCreate() {
  form.content = ''
  dialogVisible.value = true
}

async function createCorpus() {
  const content = form.content.trim()
  if (!content) {
    notify.warning('请填写语录内容')
    return
  }

  saving.value = true
  try {
    await api.apiCorpusPost({ createCorpusCommand: { content } })
    dialogVisible.value = false
    if (page.value !== 1) page.value = 1
    await loadCorpora()
    notify.success('语录已添加')
  } catch (error) {
    notify.fromError(error, '添加语录失败，内容可能已存在')
  } finally {
    saving.value = false
  }
}

async function deleteCorpus(corpus: CorpusDto) {
  if (!corpus.uid) return
  try {
    await ElMessageBox.confirm('删除后无法恢复，确定删除这条语录吗？', '删除语录', {
      type: 'warning',
      confirmButtonText: '确认删除',
      cancelButtonText: '取消',
      distinguishCancelAndClose: true,
    })
    await api.apiCorpusDelete({ deleteCorepusCommand: { uid: corpus.uid } })
    if (items.value.length === 1 && page.value > 1) page.value -= 1
    await loadCorpora()
    notify.success('语录已删除')
  } catch (error) {
    if (error === 'cancel' || error === 'close') return
    notify.fromError(error, '删除语录失败')
  }
}

function formatDate(value?: string | null) {
  if (!value) return '—'
  const date = new Date(value)
  return Number.isNaN(date.getTime()) ? value : date.toLocaleString()
}

onMounted(loadCorpora)
</script>

<template>
  <div class="corpus-view">
    <MgsPageHeader title="语录管理" description="维护语录内容。当前接口支持分页查询、新增和删除。">
      <template #actions>
        <ElButton :icon="Refresh" :loading="loading" @click="loadCorpora">刷新</ElButton>
        <ElButton type="primary" :icon="Plus" @click="openCreate">新增语录</ElButton>
      </template>
    </MgsPageHeader>

    <MgsToolbar>
      <ElInput
        v-model="keyword"
        clearable
        placeholder="搜索语录内容"
        class="search-input"
        @keyup.enter="search"
        @clear="clearSearch"
      >
        <template #append>
          <ElButton :icon="Search" @click="search">搜索</ElButton>
        </template>
      </ElInput>
      <ElSelect :model-value="pageSize" class="page-size" @change="changePageSize">
        <ElOption :value="10" label="每页 10 条" />
        <ElOption :value="20" label="每页 20 条" />
        <ElOption :value="50" label="每页 50 条" />
      </ElSelect>
      <ElTag type="info">{{ pageLabel }}</ElTag>
    </MgsToolbar>

    <MgsPageContainer>
      <MgsPageState
        v-if="!loading && !items.length && !appliedKeyword"
        state="empty"
        description="还没有语录，点击“新增语录”添加第一条。"
      />
      <MgsPageState
        v-else-if="!loading && !items.length"
        state="empty"
        description="没有找到匹配的语录，请尝试其他关键词。"
      />
      <ElTable
        v-else
        :data="items"
        v-loading="loading"
        row-key="uid"
        class="corpus-table"
        empty-text="暂无语录"
      >
        <ElTableColumn label="语录内容" min-width="360">
          <template #default="{ row }">
            <div class="corpus-content">{{ row.corpusContent || '（空内容）' }}</div>
          </template>
        </ElTableColumn>
        <ElTableColumn prop="createDate" label="创建时间" width="190">
          <template #default="{ row }">{{ formatDate(row.createDate) }}</template>
        </ElTableColumn>
        <ElTableColumn prop="uid" label="UID" min-width="220" show-overflow-tooltip />
        <ElTableColumn label="操作" width="100" fixed="right">
          <template #default="{ row }">
            <ElButton link type="danger" :icon="Delete" @click="deleteCorpus(row)">删除</ElButton>
          </template>
        </ElTableColumn>
      </ElTable>

      <div class="pagination">
        <ElButton :disabled="page <= 1 || loading" @click="page--; loadCorpora()">上一页</ElButton>
        <span>{{ pageLabel }}</span>
        <ElButton :disabled="!hasMore || loading" @click="page++; loadCorpora()">下一页</ElButton>
      </div>
    </MgsPageContainer>

    <ElDialog v-model="dialogVisible" title="新增语录" width="min(620px, 94vw)" destroy-on-close>
      <ElForm label-position="top">
        <ElFormItem label="语录内容" required>
          <ElInput
            v-model="form.content"
            type="textarea"
            :rows="6"
            resize="vertical"
            placeholder="输入语录内容"
            show-word-limit
          />
        </ElFormItem>
      </ElForm>
      <template #footer>
        <ElButton @click="dialogVisible = false">取消</ElButton>
        <ElButton type="primary" :loading="saving" @click="createCorpus">保存</ElButton>
      </template>
    </ElDialog>
  </div>
</template>

<style scoped>
.corpus-view { min-height: 100%; }
.search-input { width: min(520px, 100%); }
.page-size { width: 140px; }
.corpus-table { width: 100%; }
.corpus-content { white-space: pre-wrap; overflow-wrap: anywhere; line-height: 1.65; }
.pagination { display: flex; align-items: center; justify-content: flex-end; gap: 12px; padding-top: 16px; color: var(--el-text-color-secondary); font-size: 12px; }
@media (max-width: 640px) {
  .search-input { width: 100%; }
  .page-size { width: 100%; }
  .pagination { justify-content: center; }
}
</style>
