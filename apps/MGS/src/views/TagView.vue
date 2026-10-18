<script setup lang="ts">
import { ElButton, ElCol, ElDialog, ElForm, ElFormItem, ElInput, ElMessage, ElMessageBox, ElOption, ElSegmented, ElSelect, ElSwitch, ElTable, ElTableColumn, ElTag, ElText } from 'element-plus'
import { Delete, Edit, Plus, Refresh, Search } from '@element-plus/icons-vue'
import { computed, onBeforeMount, ref } from 'vue'
import { TagApi, type CreateTagCommand, type TagDto, type UpdateTagCommand } from '@/api'
import { apiConfiguration } from '@/services/api'
import { useTagStore } from '@/stores/uuseTagStore'
import MgsPageHeader from '../components/ui/MgsPageHeader.vue'
import MgsToolbar from '@/components/ui/MgsToolbar.vue'
const columnMap: Record<string, string> = { uid: '唯一标识', name: '标签名称', aliasName: '标签别名', isCategory: '分类标识' }
const activeSelect = ref<string>('选择字段')
const searchQuery = ref('')
const selectCategory = ref<number>(-1)
const segmentedOptions = [{ label: '全部', value: -1 }, { label: '普通标签', value: 0 }, { label: '分类', value: 1 }]
const tagStore = useTagStore()
const proxyTags = ref<TagDto[]>([])
const isLoading = ref(true)
const dialogVisible = ref(false)
const isSaving = ref(false)
const editingTag = ref<TagDto | null>(null)
const addTagDto = ref<CreateTagCommand>({ name: '', aliasName: '', isCategory: false })
const tagApi = new TagApi(apiConfiguration)

onBeforeMount(async () => {
  isLoading.value = true
  try { await tagStore.fetchAllTags(tagApi); filterData() }
  catch (error) { ElMessage.error(error instanceof Error ? error.message : '标签数据加载失败') }
  finally { isLoading.value = false }
})
// const tableColumns = computed(() => Object.keys(columnMap))
const tempList = computed(() => Object.keys(columnMap).filter(key => key !== 'isCategory'))

function filterData() {
  let result = [...tagStore.tags]
  if (selectCategory.value !== -1) result = result.filter(tag => tag.isCategory === Boolean(selectCategory.value))
  const keyword = searchQuery.value.trim().toLowerCase()
  if (keyword && activeSelect.value !== '选择字段') {
    const field = activeSelect.value as keyof TagDto
    result = result.filter(tag => String(tag[field] ?? '').toLowerCase().includes(keyword))
  }
  proxyTags.value = result
}
function openCreate() {
  editingTag.value = null
  addTagDto.value = { name: '', aliasName: '', isCategory: false }
  dialogVisible.value = true
}
function openEdit(tag: TagDto) {
  editingTag.value = tag
  addTagDto.value = { name: tag.name ?? '', aliasName: tag.aliasName ?? '', isCategory: !!tag.isCategory }
  dialogVisible.value = true
}
async function saveTag() {
  const name = addTagDto.value.name?.trim()
  if (!name) { ElMessage.warning('请填写标签名称'); return }
  isSaving.value = true
  try {
    if (editingTag.value?.uid) {
      const command: UpdateTagCommand = { uid: editingTag.value.uid, tagName: name, aliasName: addTagDto.value.aliasName?.trim() || null, isCategory: !!addTagDto.value.isCategory }
      await tagApi.apiTagPatch({ updateTagCommand: command })
      const index = tagStore.tags.findIndex(tag => tag.uid === editingTag.value?.uid)
      if (index >= 0) tagStore.tags[index] = { ...tagStore.tags[index], name, aliasName: command.aliasName, isCategory: command.isCategory }
      ElMessage.success('标签已更新')
    } else {
      const uid = await tagApi.apiTagPost({ createTagCommand: { name, aliasName: addTagDto.value.aliasName?.trim() || null, isCategory: !!addTagDto.value.isCategory } })
      const result = await tagApi.apiTagGet({ uID: uid })
      const created = result.items?.[0]
      if (!created) throw new Error('创建后未找到对应标签')
      tagStore.tags.push(created)
      ElMessage.success('标签已创建')
    }
    dialogVisible.value = false
    filterData()
  } catch (error) { ElMessage.error(error instanceof Error ? error.message : '保存标签失败') }
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
    ElMessage.success('标签已删除')
  } catch (error) {
    if (error === 'cancel' || error === 'close') return
    ElMessage.error(error instanceof Error ? error.message : '删除失败：标签可能仍被博文使用')
  }
}
</script>

<template>
  <div class="todo-view-root">
    <MgsPageHeader title="标签" description="管理标签,维护标签名称、别名和分类属性">
      <template #actions>
        <ElButton :icon="Refresh" @click="filterData">Refresh</ElButton>
        <ElButton type="primary" :icon="Plus" @click="openCreate">New Tag</ElButton>
      </template>
    </MgsPageHeader>
    <MgsToolbar>
      <ElSegmented v-model="selectCategory" :options="segmentedOptions" @change="filterData" />
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

    <ElTable :data="proxyTags" v-loading="isLoading" element-loading-text="正在加载标签..." empty-text="暂无标签"
      class="tag-table" row-key="uid" highlight-current-row>
      <ElTableColumn prop="name" label="标签名称" min-width="180"><template #default="{ row }">
          <ElTag v-if="row.name">{{ row.name }}</ElTag>
          <ElText v-else>未命名</ElText>
        </template>
      </ElTableColumn>
      <ElTableColumn prop="aliasName" label="标签别名" min-width="160"><template #default="{ row }">{{ row.aliasName || '—'
          }}</template></ElTableColumn>
      <ElTableColumn prop="isCategory" label="类型" width="120"><template #default="{ row }">
          <ElTag :type="row.isCategory ? 'success' : 'info'">{{ row.isCategory ? '分类' : '普通标签' }}</ElTag>
        </template>
      </ElTableColumn>
      <ElTableColumn prop="uid" label="UID" min-width="220" show-overflow-tooltip />
      <ElTableColumn label="操作" width="150" fixed="right"><template #default="{ row }">
          <ElButton link type="primary" :icon="Edit" @click="openEdit(row)">编辑</ElButton>
          <ElButton link type="danger" :icon="Delete" @click="deleteTag(row)">删除</ElButton>
        </template>
      </ElTableColumn>
    </ElTable>
    <ElDialog v-model="dialogVisible" :title="editingTag ? '编辑标签' : '添加标签'" width="min(520px, 92vw)" destroy-on-close>
      <ElForm label-position="top">
        <ElFormItem label="标签名称" required>
          <ElInput v-model="addTagDto.name" maxlength="100" show-word-limit />
        </ElFormItem>
        <ElFormItem label="标签别名">
          <ElInput v-model="addTagDto.aliasName" maxlength="100" />
        </ElFormItem>
        <ElFormItem label="分类标签">
          <ElSwitch v-model="addTagDto.isCategory" inline-prompt active-text="分类" inactive-text="普通" />
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
