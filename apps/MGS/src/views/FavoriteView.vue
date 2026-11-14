<script setup lang="ts">
import { computed, onBeforeMount, onBeforeUnmount, ref } from 'vue'
import {
  ElButton, ElDialog, ElEmpty, ElForm, ElFormItem, ElInput, ElMessage,
  ElMessageBox, ElOption, ElSelect, ElSwitch, ElTable, ElTableColumn, ElTag, ElUpload,
} from 'element-plus'
import { Delete, Edit, Plus, Refresh, Upload } from '@element-plus/icons-vue'
import {
  FavoriteApi, FileApi, FileScope, TagApi, CategoryApi, CategoryQueryType,
  type CategoryDto, type FavoriteDto, type FavoriteMediaInput, type FileDto, type TagDto,
} from '@/api'
import { apiConfiguration } from '@/services/api'
import MgsPageHeader from '@/components/ui/MgsPageHeader.vue'
import MgsToolbar from '@/components/ui/MgsToolbar.vue'
import MgsPageContainer from '@/components/ui/MgsPageContainer.vue'
import { FavoriteDetailContent } from '@aqlife/ui-shared'
import type { UploadFile } from 'element-plus'

const favoriteApi = new FavoriteApi(apiConfiguration)
const fileApi = new FileApi(apiConfiguration)
const tagApi = new TagApi(apiConfiguration)
const categoryApi = new CategoryApi(apiConfiguration)

const items = ref<FavoriteDto[]>([])
const files = ref<FileDto[]>([])
const tags = ref<TagDto[]>([])
const categories = ref<CategoryDto[]>([])
const loading = ref(false)
const saving = ref(false)
const dialogVisible = ref(false)
const detailDialogVisible = ref(false)
const detailItem = ref<FavoriteDto | null>(null)
const detailImageUrls = ref<Record<string, string>>({})
const fileDialogVisible = ref(false)
const editing = ref<FavoriteDto | null>(null)
const keyword = ref('')
const categoryFilter = ref('')
const selectedFileIds = ref<string[]>([])
const selectedTagIds = ref<string[]>([])
const previewUrls = ref<Record<string, string>>({})
const quickUploading = ref(false)

const form = ref({
  title: '',
  description: '',
  cid: '',
  sourceUrl: '',
  sortOrder: 0,
})

const filteredItems = computed(() => {
  const q = keyword.value.trim().toLocaleLowerCase()
  return items.value.filter(item =>
    (!categoryFilter.value || item.cid === categoryFilter.value) &&
    (!q || [item.title, item.description].some(value => value?.toLocaleLowerCase().includes(q))),
  )
})

async function loadData() {
  loading.value = true
  try {
    const [favoriteResult, tagResult, fileResult, categoryResult] = await Promise.all([
      favoriteApi.apiFavoriteGet({ page: 1, pageSize: 100 }),
      tagApi.apiTagGet({ page: 1, pageSize: 100 }),
      fileApi.apiFileGet({ page: 1, pageSize: 100, scope: FileScope.Image }),
      categoryApi.apiCategoryGet({ searchType: CategoryQueryType.Favorite }),
    ])
    items.value = favoriteResult.items ?? []
    tags.value = tagResult.items ?? []
    files.value = fileResult.items ?? []
    categories.value = categoryResult.items ?? []
  } catch (error) {
    ElMessage.error(error instanceof Error ? error.message : 'Favorite 数据加载失败')
  } finally {
    loading.value = false
  }
}

async function openDetail(item: FavoriteDto) {
  detailItem.value = item
  detailDialogVisible.value = true
  await Promise.all((item.media ?? []).map(async entry => {
    const fileId = entry.fileId
    if (!fileId || detailImageUrls.value[fileId]) return
    try {
      const blob = await fileApi.apiFilePreviewGet({ uID: fileId })
      detailImageUrls.value[fileId] = URL.createObjectURL(blob)
    } catch {
      // Keep detail text available if an image preview cannot be loaded.
    }
  }))
}

function editFromDetail() {
  const item = detailItem.value
  detailDialogVisible.value = false
  if (item) openEdit(item)
}
function openCreate() {
  editing.value = null
  form.value = { title: '', description: '', cid: '', sourceUrl: '', sortOrder: 0 }
  selectedFileIds.value = []
  selectedTagIds.value = []
  dialogVisible.value = true
}

async function loadFilePreviews(fileIds: string[]) {
  await Promise.all(fileIds.map(async fileId => {
    if (!fileId || previewUrls.value[fileId]) return
    try {
      const blob = await fileApi.apiFilePreviewGet({ uID: fileId })
      previewUrls.value[fileId] = URL.createObjectURL(blob)
    } catch {
      // Keep the file selected even if its preview cannot be loaded.
    }
  }))
}

async function openEdit(item: FavoriteDto) {
  editing.value = item
  form.value = {
    title: item.title ?? '',
    description: item.description ?? '',
    cid: item.cid ?? '',
    sourceUrl: item.sourceUrl ?? '',
    sortOrder: item.sortOrder ?? 0,
  }
  selectedFileIds.value = (item.media ?? []).map(media => media.fileId ?? '').filter(Boolean)
  selectedTagIds.value = [...(item.tagIds ?? [])]
  dialogVisible.value = true
  await loadFilePreviews(selectedFileIds.value)
}

async function save() {
  if (!form.value.title.trim()) {
    ElMessage.warning('请填写标题')
    return
  }
  if (!form.value.cid.trim()) {
    ElMessage.warning('请选择分类')
    return
  }
  saving.value = true
  try {
    const media: FavoriteMediaInput[] = selectedFileIds.value.map((fileId, index) => ({
      fileId, sortOrder: index,
    }))
    const payload = {
      title: form.value.title.trim(),
      description: form.value.description.trim(),
      cid: form.value.cid,
      sourceUrl: form.value.sourceUrl.trim() || null,
      sortOrder: Number(form.value.sortOrder) || 0,
      media,
      tagIds: selectedTagIds.value,
    }
    if (editing.value?.uid) {
      await favoriteApi.apiFavoritePut({ updateFavoriteCommand: { uid: editing.value.uid, ...payload } })
      ElMessage.success('Favorite 已更新')
    } else {
      await favoriteApi.apiFavoritePost({ createFavoriteCommand: { ...payload, media: selectedFileIds.value } })
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

async function openFilePicker() {
  fileDialogVisible.value = true
  await loadFilePreviews(files.value.map(file => file.uid ?? '').filter(Boolean))
}

async function handleQuickUpload(uploadFile: UploadFile) {
  const raw = uploadFile.raw
  if (!raw || quickUploading.value) return
  if (!raw.type.startsWith('image/')) {
    ElMessage.warning('这里只支持上传图片文件')
    return
  }

  quickUploading.value = true
  try {
    const uid = await fileApi.apiFileUploadPost({ file: raw })
    if (!uid) throw new Error('上传成功，但服务端未返回文件标识')
    const result = await fileApi.apiFileGet({ uID: uid })
    const uploaded = result.items?.[0]
    if (!uploaded?.uid) throw new Error('图片已上传，但无法读取文件信息')
    if (!files.value.some(file => file.uid === uploaded.uid)) files.value.unshift(uploaded)
    selectedFileIds.value = [...selectedFileIds.value, uploaded.uid]
    try {
      const blob = await fileApi.apiFilePreviewGet({ uID: uploaded.uid })
      previewUrls.value[uploaded.uid] = URL.createObjectURL(blob)
    } catch {
      // Keep the uploaded file selectable even if a preview cannot be loaded.
    }
    ElMessage.success('图片上传成功并已选中')
  } catch (error) {
    ElMessage.error(error instanceof Error ? error.message : '图片上传失败')
  } finally {
    quickUploading.value = false
  }
}

function fileName(fileId?: string) {
  return files.value.find(file => file.uid === fileId)?.fileName ?? fileId ?? '未知文件'
}

onBeforeUnmount(() => {
  Object.values(previewUrls.value).forEach(url => URL.revokeObjectURL(url))
  Object.values(detailImageUrls.value).forEach(url => URL.revokeObjectURL(url))
})

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
      <ElInput v-model="keyword" clearable placeholder="搜索标题或描述" />
      <ElSelect v-model="categoryFilter" clearable placeholder="全部分类" class="type-filter">
        <ElOption v-for="category in categories" :key="category.uid" :label="category.categoryName ?? '未命名分类'" :value="category.uid ?? ''" />
      </ElSelect>
    </MgsToolbar>

    <MgsPageContainer class="table-container">
    <ElTable :data="filteredItems" v-loading="loading" row-key="uid" height="100%" empty-text="暂无收藏内容">
      <ElTableColumn prop="title" label="标题" min-width="190" show-overflow-tooltip>
        <template #default="{ row }">
          <ElButton link type="primary" @click="openDetail(row)">{{ row.title || '未命名收藏' }}</ElButton>
        </template>
      </ElTableColumn>
      <ElTableColumn label="分类" width="130">
        <template #default="{ row }">
          {{ categories.find(category => category.uid === row.cid)?.categoryName ?? '未分类' }}
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
    </MgsPageContainer>

    <ElDialog v-model="detailDialogVisible" title="收藏详情" width="min(920px, 94vw)" top="5vh" destroy-on-close>
      <div v-if="detailItem" class="favorite-detail-dialog">
        <FavoriteDetailContent :favorite="detailItem" :tags="tags" :categories="categories" :image-urls="detailImageUrls" editable @edit="editFromDetail" />
      </div>
    </ElDialog>
    <ElDialog v-model="dialogVisible" :title="editing ? '编辑收藏' : '新增收藏'" width="min(720px, 94vw)" destroy-on-close>
      <ElForm label-position="top">
        <ElFormItem label="标题" required>
          <ElInput v-model="form.title" maxlength="160" show-word-limit />
        </ElFormItem>
        <ElFormItem label="分类" required>
          <ElSelect v-model="form.cid" class="full-width" placeholder="请选择分类">
            <ElOption v-for="category in categories" :key="category.uid" :label="category.categoryName ?? '未命名分类'" :value="category.uid ?? ''" />
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
            <div v-if="selectedFileIds.length" class="selected-media-grid">
              <div v-for="(fileId, index) in selectedFileIds" :key="fileId" class="selected-media-item">
                <img v-if="previewUrls[fileId]" :src="previewUrls[fileId]" :alt="fileName(fileId)">
                <div v-else class="media-placeholder">无预览</div>
                <span>{{ index === 0 ? '封面' : '图片 ' + (index + 1) }}</span>
                <ElButton circle text type="danger" :icon="Delete" @click="selectedFileIds = selectedFileIds.filter(id => id !== fileId)" />
              </div>
            </div>
            <div v-else class="media-summary">尚未选择图片</div>
            <div class="media-actions">
              <ElButton @click="openFilePicker">选择素材</ElButton>
              <ElButton v-if="selectedFileIds.length" text type="danger" @click="selectedFileIds = []">清空</ElButton>
            </div>
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

    <ElDialog v-model="fileDialogVisible" title="选择图片素材" width="min(860px, 94vw)">
      <div class="picker-toolbar">
        <div><strong>图片素材库</strong><span>点击卡片即可多选，第一张图片将作为封面</span></div>
        <ElUpload accept="image/*" :auto-upload="false" :show-file-list="false" :on-change="handleQuickUpload">
          <ElButton type="primary" :icon="Upload" :loading="quickUploading">上传图片</ElButton>
        </ElUpload>
      </div>
      <div class="file-options">
        <button v-for="file in files" :key="file.uid" type="button" class="file-option"
          :class="{ selected: selectedFileIds.includes(file.uid ?? '') }"
          @click="toggleFile(file.uid ?? '', !selectedFileIds.includes(file.uid ?? ''))">
          <div class="file-option-preview">
            <img v-if="file.uid && previewUrls[file.uid]" :src="previewUrls[file.uid]" :alt="file.fileName ?? '图片预览'">
            <span v-else>暂无预览</span>
            <span v-if="selectedFileIds.includes(file.uid ?? '')" class="selected-mark">✓</span>
          </div>
          <span class="file-option-name">{{ file.fileName || file.uid }}</span>
        </button>
        <ElEmpty v-if="!files.length" description="暂无图片素材，可直接上传" />
      </div>
      <template #footer>
        <span class="picker-selection">已选择 {{ selectedFileIds.length }} 张图片</span>
        <ElButton @click="fileDialogVisible = false">完成</ElButton>
      </template>
    </ElDialog>
  </div>
</template>

<style scoped>
.favorite-detail-dialog { max-height: 72vh; overflow-y: auto; padding: 0 8px; }
.favorite-admin { height: 100%; min-height: 0; display: flex; flex-direction: column; gap: 12px; }
.table-container { flex:1; min-height:0; display:flex; flex-direction:column; }
.favorite-admin :deep(.el-table) { flex: 1; min-height: 0; }
.type-filter { width: 160px; }
.full-width { width: 100%; }
.tag-chip { margin: 2px 4px 2px 0; }
.media-picker { display: flex; flex-direction: column; gap: 12px; width: 100%; }
.media-actions { display: flex; gap: 8px; }
.selected-media-grid { display: flex; flex-wrap: wrap; gap: 10px; }
.selected-media-item { position: relative; width: 112px; display: flex; flex-direction: column; gap: 4px; font-size: 12px; color: var(--el-text-color-secondary); }
.selected-media-item img, .media-placeholder { width: 112px; height: 82px; object-fit: cover; border-radius: 6px; background: var(--el-fill-color-light); }
.selected-media-item :deep(.el-button) { position: absolute; top: 3px; right: 3px; background: var(--el-bg-color); }
.media-placeholder { display: grid; place-items: center; }
.picker-toolbar { display: flex; justify-content: space-between; align-items: center; gap: 16px; margin-bottom: 16px; }
.picker-toolbar > div { display: flex; flex-direction: column; gap: 4px; }
.picker-toolbar span { font-size: 12px; color: var(--el-text-color-secondary); }
.picker-selection { float: left; color: var(--el-text-color-secondary); font-size: 13px; }
.media-summary { flex: 1; min-width: 180px; color: var(--el-text-color-secondary); overflow-wrap: anywhere; }
.file-options { display: grid; grid-template-columns: repeat(auto-fill, minmax(132px, 1fr)); gap: 12px; max-height: 56vh; overflow: auto; }
.file-option { min-width: 0; padding: 7px; border: 1px solid var(--el-border-color); border-radius: 8px; background: var(--el-bg-color); cursor: pointer; text-align: left; transition: border-color .15s, background .15s; }
.file-option:hover { border-color: var(--el-color-primary); }
.file-option.selected { border: 2px solid var(--el-color-primary); padding: 6px; background: var(--el-color-primary-light-9); }
.file-option-preview { position: relative; height: 104px; display: grid; place-items: center; overflow: hidden; border-radius: 5px; background: var(--el-fill-color-light); color: var(--el-text-color-placeholder); font-size: 12px; }
.file-option-preview img { width: 100%; height: 100%; object-fit: cover; }
.selected-mark { position: absolute; top: 5px; right: 5px; width: 22px; height: 22px; display: grid; place-items: center; border-radius: 50%; color: white; background: var(--el-color-primary); font-weight: 700; }
.file-option-name { display: block; margin-top: 7px; overflow: hidden; text-overflow: ellipsis; white-space: nowrap; font-size: 12px; }
@media (max-width: 600px) { .file-options { grid-template-columns: repeat(2, minmax(0, 1fr)); } .picker-toolbar { align-items: flex-start; flex-direction: column; } }
</style>
