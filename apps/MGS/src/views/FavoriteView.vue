<script setup lang="ts">
import { computed, onBeforeMount, ref } from 'vue'
import {
  ElButton, ElCheckbox, ElDialog, ElEmpty, ElForm, ElFormItem, ElInput, ElMessage,
  ElMessageBox, ElOption, ElSelect, ElSwitch, ElTable, ElTableColumn, ElTag,
} from 'element-plus'
import { Delete, Edit, Plus, Refresh } from '@element-plus/icons-vue'
import {
  FavoriteApi, FileApi, FileScope, TagApi,
  type FavoriteDto, type FavoriteMediaInput, type FileDto, type TagDto,
} from '@/api'
import { apiConfiguration } from '@/services/api'
import MgsPageHeader from '@/components/ui/MgsPageHeader.vue'
import MgsToolbar from '@/components/ui/MgsToolbar.vue'

const favoriteApi = new FavoriteApi(apiConfiguration)
const fileApi = new FileApi(apiConfiguration)
const tagApi = new TagApi(apiConfiguration)

const items = ref<FavoriteDto[]>([])
const files = ref<FileDto[]>([])
const tags = ref<TagDto[]>([])
const loading = ref(false)
const saving = ref(false)
const dialogVisible = ref(false)
const fileDialogVisible = ref(false)
const editing = ref<FavoriteDto | null>(null)
const keyword = ref('')
const typeFilter = ref('')
const selectedFileIds = ref<string[]>([])
const selectedTagIds = ref<string[]>([])

const contentTypes = [
  { label: '食物', value: 'Food' },
  { label: '书籍', value: 'Book' },
  { label: '游戏', value: 'Game' },
  { label: '技术', value: 'Tech' },
  { label: '生活', value: 'Life' },
  { label: '其他', value: 'Other' },
]
const form = ref({
  title: '',
  description: '',
  contentType: 'Other',
  sourceUrl: '',
  sortOrder: 0,
})

const filteredItems = computed(() => {
  const q = keyword.value.trim().toLocaleLowerCase()
  return items.value.filter(item =>
    (!typeFilter.value || item.contentType === typeFilter.value) &&
    (!q || [item.title, item.description, item.contentType].some(value => value?.toLocaleLowerCase().includes(q))),
  )
})

async function loadData() {
  loading.value = true
  try {
    const [favoriteResult, tagResult, fileResult] = await Promise.all([
      favoriteApi.apiFavoriteGet({ page: 1, pageSize: 100, includeUnpublished: true }),
      tagApi.apiTagGet({ page: 1, pageSize: 100 }),
      fileApi.apiFileGet({ page: 1, pageSize: 100, scope: FileScope.Image }),
    ])
    items.value = favoriteResult.items ?? []
    tags.value = tagResult.items ?? []
    files.value = fileResult.items ?? []
  } catch (error) {
    ElMessage.error(error instanceof Error ? error.message : 'Favorite 数据加载失败')
  } finally {
    loading.value = false
  }
}

function openCreate() {
  editing.value = null
  form.value = { title: '', description: '', contentType: 'Other', sourceUrl: '', sortOrder: 0 }
  selectedFileIds.value = []
  selectedTagIds.value = []
  dialogVisible.value = true
}

function openEdit(item: FavoriteDto) {
  editing.value = item
  form.value = {
    title: item.title ?? '',
    description: item.description ?? '',
    contentType: item.contentType ?? 'Other',
    sourceUrl: item.sourceUrl ?? '',
    sortOrder: item.sortOrder ?? 0,
  }
  selectedFileIds.value = (item.media ?? []).map(media => media.fileId ?? '').filter(Boolean)
  selectedTagIds.value = [...(item.tagIds ?? [])]
  dialogVisible.value = true
}

async function save() {
  if (!form.value.title.trim()) {
    ElMessage.warning('请填写标题')
    return
  }
  if (!form.value.contentType) {
    ElMessage.warning('请选择内容类型')
    return
  }
  saving.value = true
  try {
    const media: FavoriteMediaInput[] = selectedFileIds.value.map((fileId, index) => ({
      fileId, role: index === 0 ? 'Cover' : 'Image', sortOrder: index,
    }))
    const payload = {
      title: form.value.title.trim(),
      description: form.value.description.trim(),
      contentType: form.value.contentType,
      sourceUrl: form.value.sourceUrl.trim() || null,
      sortOrder: Number(form.value.sortOrder) || 0,
      media,
      tagIds: selectedTagIds.value,
    }
    if (editing.value?.uid) {
      await favoriteApi.apiFavoritePut({ updateFavoriteCommand: { uid: editing.value.uid, ...payload } })
      ElMessage.success('Favorite 已更新')
    } else {
      await favoriteApi.apiFavoritePost({ createFavoriteCommand: payload })
      ElMessage.success('Favorite 已创建')
    }
    dialogVisible.value = false
    await loadData()
  } catch (error) {
    ElMessage.error(error instanceof Error ? error.message : '保存 Favorite 失败')
  } finally {
    saving.value = false
  }
}

async function togglePublished(item: FavoriteDto, value: string | number | boolean) {
  if (!item.uid) return
  try {
    await favoriteApi.apiFavoritePublishPatch({
      setFavoritePublishedCommand: { uid: item.uid, isPublished: Boolean(value) },
    })
    item.isPublished = Boolean(value)
    ElMessage.success(value ? '已发布' : '已取消发布')
  } catch (error) {
    ElMessage.error(error instanceof Error ? error.message : '发布状态更新失败')
    await loadData()
  }
}

async function remove(item: FavoriteDto) {
  if (!item.uid) return
  try {
    await ElMessageBox.confirm(`确定删除「${item.title || '未命名收藏'}」吗？此操作无法撤销。`, '删除确认', {
      type: 'warning', confirmButtonText: '确认删除', cancelButtonText: '取消',
    })
    await favoriteApi.apiFavoriteDelete({ uID: item.uid })
    ElMessage.success('Favorite 已删除')
    await loadData()
  } catch (error) {
    if (error === 'cancel' || error === 'close') return
    ElMessage.error(error instanceof Error ? error.message : '删除 Favorite 失败')
  }
}

function toggleFile(fileId: string, checked: string | number | boolean) {
  if (!fileId) return
  if (checked && !selectedFileIds.value.includes(fileId)) selectedFileIds.value.push(fileId)
  else if (!checked) selectedFileIds.value = selectedFileIds.value.filter(value => value !== fileId)
}

function fileName(fileId?: string) {
  return files.value.find(file => file.uid === fileId)?.fileName ?? fileId ?? '未知文件'
}

onBeforeMount(loadData)
</script>

<template>
  <div class="favorite-admin">
    <MgsPageHeader title="收藏管理" description="整理访客端展示的收藏内容">
      <template #actions>
        <ElButton :icon="Refresh" :loading="loading" @click="loadData">刷新</ElButton>
        <ElButton type="primary" :icon="Plus" @click="openCreate">新增收藏</ElButton>
      </template>
    </MgsPageHeader>

    <MgsToolbar>
      <ElInput v-model="keyword" clearable placeholder="搜索标题、描述或类型" />
      <ElSelect v-model="typeFilter" clearable placeholder="全部类型" class="type-filter">
        <ElOption v-for="type in contentTypes" :key="type.value" :label="type.label" :value="type.value" />
      </ElSelect>
    </MgsToolbar>

    <ElTable :data="filteredItems" v-loading="loading" row-key="uid" height="100%" empty-text="暂无收藏内容">
      <ElTableColumn prop="title" label="标题" min-width="190" show-overflow-tooltip />
      <ElTableColumn prop="contentType" label="类型" width="110">
        <template #default="{ row }">
          <ElTag>{{ contentTypes.find(type => type.value === row.contentType)?.label ?? row.contentType }}</ElTag>
        </template>
      </ElTableColumn>
      <ElTableColumn label="媒体" min-width="180">
        <template #default="{ row }">{{ (row.media ?? []).map((media: NonNullable<FavoriteDto['media']>[number]) => fileName(media.fileId)).join('、') || '无媒体' }}</template>
      </ElTableColumn>
      <ElTableColumn label="标签" min-width="140">
        <template #default="{ row }">
          <ElTag v-for="id in row.tagIds ?? []" :key="id" class="tag-chip">
            {{ tags.find(tag => tag.uid === id)?.name ?? id }}
          </ElTag>
        </template>
      </ElTableColumn>
      <ElTableColumn label="公开" width="100" align="center">
        <template #default="{ row }">
          <ElSwitch :model-value="!!row.isPublished" @change="value => togglePublished(row, value)" />
        </template>
      </ElTableColumn>
      <ElTableColumn label="排序" prop="sortOrder" width="80" />
      <ElTableColumn label="操作" width="140" fixed="right">
        <template #default="{ row }">
          <ElButton link type="primary" :icon="Edit" @click="openEdit(row)">编辑</ElButton>
          <ElButton link type="danger" :icon="Delete" @click="remove(row)">删除</ElButton>
        </template>
      </ElTableColumn>
    </ElTable>

    <ElDialog v-model="dialogVisible" :title="editing ? '编辑收藏' : '新增收藏'" width="min(720px, 94vw)" destroy-on-close>
      <ElForm label-position="top">
        <ElFormItem label="标题" required>
          <ElInput v-model="form.title" maxlength="160" show-word-limit />
        </ElFormItem>
        <ElFormItem label="内容类型" required>
          <ElSelect v-model="form.contentType" class="full-width">
            <ElOption v-for="type in contentTypes" :key="type.value" :label="type.label" :value="type.value" />
          </ElSelect>
        </ElFormItem>
        <ElFormItem label="描述">
          <ElInput v-model="form.description" type="textarea" :rows="4" />
        </ElFormItem>
        <ElFormItem label="来源链接">
          <ElInput v-model="form.sourceUrl" placeholder="https://..." />
        </ElFormItem>
        <ElFormItem label="排序值">
          <ElInput v-model.number="form.sortOrder" type="number" />
        </ElFormItem>
        <ElFormItem label="关联图片">
          <div class="media-picker">
            <div class="media-summary">{{ selectedFileIds.length ? selectedFileIds.map(fileName).join('、') : '尚未选择图片' }}</div>
            <ElButton @click="fileDialogVisible = true">选择文件</ElButton>
            <ElButton v-if="selectedFileIds.length" text type="danger" @click="selectedFileIds = []">清空</ElButton>
          </div>
        </ElFormItem>
        <ElFormItem label="标签">
          <ElSelect v-model="selectedTagIds" multiple filterable clearable class="full-width" placeholder="选择标签">
            <ElOption v-for="tag in tags" :key="tag.uid ?? tag.name ?? 'tag'" :label="tag.name ?? '未命名标签'" :value="tag.uid!" />
          </ElSelect>
        </ElFormItem>
      </ElForm>
      <template #footer>
        <ElButton @click="dialogVisible = false">取消</ElButton>
        <ElButton type="primary" :loading="saving" @click="save">保存</ElButton>
      </template>
    </ElDialog>

    <ElDialog v-model="fileDialogVisible" title="选择图片文件" width="min(680px, 94vw)">
      <div class="file-options">
        <ElCheckbox v-for="file in files" :key="file.uid" :model-value="selectedFileIds.includes(file.uid ?? '')"
          :label="file.uid" @change="checked => toggleFile(file.uid ?? '', checked)">
          {{ file.fileName || file.uid }}
        </ElCheckbox>
        <ElEmpty v-if="!files.length" description="没有可关联的图片文件" />
      </div>
      <template #footer>
        <ElButton @click="fileDialogVisible = false">完成（{{ selectedFileIds.length }}）</ElButton>
      </template>
    </ElDialog>
  </div>
</template>

<style scoped>
.favorite-admin { height: 100%; min-height: 0; display: flex; flex-direction: column; gap: 12px; }
.favorite-admin :deep(.el-table) { flex: 1; min-height: 0; }
.type-filter { width: 160px; }
.full-width { width: 100%; }
.tag-chip { margin: 2px 4px 2px 0; }
.media-picker { display: flex; align-items: center; flex-wrap: wrap; gap: 10px; width: 100%; }
.media-summary { flex: 1; min-width: 180px; color: var(--el-text-color-secondary); overflow-wrap: anywhere; }
.file-options { display: grid; grid-template-columns: repeat(2, minmax(0, 1fr)); gap: 12px; max-height: 52vh; overflow: auto; }
@media (max-width: 600px) { .file-options { grid-template-columns: 1fr; } }
</style>
