<script setup lang="ts">
import { notify, useLoadingTask } from '@aqlife/ui-shared'
import { ElButton, ElCol, ElDialog, ElForm, ElFormItem, ElInput, ElMessageBox, ElOption, ElSegmented, ElSelect, ElTable, ElTableColumn, ElTag, ElText } from 'element-plus'
import { Delete, Edit, Plus, Refresh, Search } from '@element-plus/icons-vue'
import { computed, onBeforeMount, ref } from 'vue'
import { TagApi, type CreateTagCommand, type TagDto, type UpdateTagCommand } from '@/api'
import { apiConfiguration } from '@/services/api'
import { useTagStore } from '@/stores/uuseTagStore'
import MgsPageHeader from '../components/ui/MgsPageHeader.vue'
import MgsToolbar from '@/components/ui/MgsToolbar.vue'
import MgsPageContainer from '@/components/ui/MgsPageContainer.vue'
const columnMap: Record<string, string> = { uid: '唯一标识', name: '标签名称', aliasName: '标签别名' }
const activeSelect = ref<string>('选择字段')
const searchQuery = ref('')
const tagStore = useTagStore()
const proxyTags = ref<TagDto[]>([])
const { isLoading: isTableLoading, run: withTableLoading } = useLoadingTask('mgs:table:tags')
const dialogVisible = ref(false)
const isSaving = ref(false)
const editingTag = ref<TagDto | null>(null)
const addTagDto = ref<CreateTagCommand>({ name: '', aliasName: '' })
const tagApi = new TagApi(apiConfiguration)

onBeforeMount(async () => {
  try {
    await withTableLoading(async () => { await tagStore.fetchAllTags(tagApi); filterData() })
  } catch (error) {
    notify.fromError(error, '标签数据加载失败')
  }
})
// const tableColumns = computed(() => Object.keys(columnMap))
const tempList = computed(() => Object.keys(columnMap))

function filterData() {
  let result = [...tagStore.tags]
  const keyword = searchQuery.value.trim().toLowerCase()
  if (keyword && activeSelect.value !== '选择字段') {
    const field = activeSelect.value as keyof TagDto
    result = result.filter(tag => String(tag[field] ?? '').toLowerCase().includes(keyword))
  }
  proxyTags.value = result
}
function openCreate() {
  editingTag.value = null
  addTagDto.value = { name: '', aliasName: '' }
  dialogVisible.value = true
}
function openEdit(tag: TagDto) {
  editingTag.value = tag
  addTagDto.value = { name: tag.name ?? '', aliasName: tag.aliasName ?? '' }
  dialogVisible.value = true
}
async function saveTag() {
  const name = addTagDto.value.name?.trim()
  if (!name) { notify.warning('请填写标签名称'); return }
  isSaving.value = true
  try {
    if (editingTag.value?.uid) {
      const command: UpdateTagCommand = { uid: editingTag.value.uid, tagName: name, aliasName: addTagDto.value.aliasName?.trim() || null }
      await tagApi.apiTagPatch({ updateTagCommand: command })
      const index = tagStore.tags.findIndex(tag => tag.uid === editingTag.value?.uid)
      if (index >= 0) tagStore.tags[index] = { ...tagStore.tags[index], name, aliasName: command.aliasName }
      notify.success('标签已更新')
    } else {
      const uid = await tagApi.apiTagPost({ createTagCommand: { name, aliasName: addTagDto.value.aliasName?.trim() || null } })
      const result = await tagApi.apiTagGet({ uID: uid })
      const created = result.items?.[0]
      if (!created) throw new Error('创建后未找到对应标签')
      tagStore.tags.push(created)
      notify.success('标签已创建')
    }
    dialogVisible.value = false
    filterData()
  } catch (error) { notify.error(error instanceof Error ? error.message : '保存标签失败') }
  finally { isSaving.value = false }
}
async function deleteTag(tag: TagDto) {
  try {
    await ElMessageBox.confirm(`确定删除标签「${tag.name || '未命名标签'}」吗？如果该标签仍被博文使用，服务器会拒绝删除。`, '删除标签确认', {
      type: 'warning', confirmButtonText: '确认删除', cancelButtonText: '取消', distinguishCancelAndClose: true,
    })
    await tagApi.apiTagDelete({ deleteTagCommand: { uid: tag.uid ?? undefined } })
    const index = tagStore.tags.findIndex(item => item.uid === tag.uid)
    if (index >= 0) tagStore.tags.splice(index, 1)
    filterData()
    notify.success('标签已删除')
  } catch (error) {
    if (error === 'cancel' || error === 'close') return
    notify.error(error instanceof Error ? error.message : '删除失败：标签可能仍被博文使用')
  }
}
</script>

<template>
  <div class="todo-view-root">
    <MgsPageHeader title="标签" description="管理普通标签的名称和别名">
      <template #actions>
        <ElButton :icon="Refresh" @click="filterData">Refresh</ElButton>
        <ElButton type="primary" :icon="Plus" @click="openCreate">New Tag</ElButton>
      </template>
    </MgsPageHeader>
    <MgsToolbar>
      <ElInput v-model="searchQuery" placeholder="输入关键词" clearable @keyup.enter="filterData" @clear="filterData">
        <template #prepend>
          <ElSelect v-model="activeSelect" placeholder="搜索字段" style="width:120px">
            <ElOption v-for="item in tempList" :key="item" :label="columnMap[item]" :value="item" />
          </ElSelect>
        </template>
        <template #append>
          <ElButton :icon="Search" @click="filterData" />
        </template>
      </ElInput>
    </MgsToolbar>

    <MgsPageContainer>
    <ElTable :data="proxyTags" v-loading="isTableLoading" element-loading-text="正在加载标签..." empty-text="暂无标签"
      class="tag-table" row-key="uid" highlight-current-row>
      <ElTableColumn prop="name" label="标签名称" min-width="180"><template #default="{ row }">
          <ElTag v-if="row.name">{{ row.name }}</ElTag>
          <ElText v-else>未命名</ElText>
        </template>
      </ElTableColumn>
      <ElTableColumn prop="aliasName" label="标签别名" min-width="160"><template #default="{ row }">{{ row.aliasName || '—'
          }}</template></ElTableColumn>
      <ElTableColumn prop="uid" label="UID" min-width="220" show-overflow-tooltip />
      <ElTableColumn label="操作" width="150" fixed="right"><template #default="{ row }">
          <ElButton link type="primary" :icon="Edit" @click="openEdit(row)">编辑</ElButton>
          <ElButton link type="danger" :icon="Delete" @click="deleteTag(row)">删除</ElButton>
        </template>
      </ElTableColumn>
    </ElTable>
    </MgsPageContainer>
    <ElDialog v-model="dialogVisible" :title="editingTag ? '编辑标签' : '添加标签'" width="min(520px, 92vw)" destroy-on-close>
      <ElForm label-position="top">
        <ElFormItem label="标签名称" required>
          <ElInput v-model="addTagDto.name" maxlength="100" show-word-limit />
        </ElFormItem>
        <ElFormItem label="标签别名">
          <ElInput v-model="addTagDto.aliasName" maxlength="100" />
        </ElFormItem>
      </ElForm>
      <template #footer>
        <ElButton @click="dialogVisible = false">取消</ElButton>
        <ElButton type="primary" :loading="isSaving" @click="saveTag">保存</ElButton>
      </template>
    </ElDialog>
  </div>
</template>

<style scoped>
.tag-table {
  width: 100%;
}
</style>
