<script setup lang="ts">
import { computed, onBeforeMount, ref } from 'vue'
import {
  ElButton, ElDialog, ElEmpty, ElForm, ElFormItem, ElInput, ElMessage,
  ElMessageBox, ElOption, ElSelect, ElTable, ElTableColumn, ElTag,
} from 'element-plus'
import { Delete, Edit, Plus, Refresh } from '@element-plus/icons-vue'
import {
  CategoryApi, CategoryQueryType,
  type CategoryDto, type CreateCategoryCommand, type UpdateCategoryCommand,
} from '@/api'
import { apiConfiguration } from '@/services/api'
import MgsPageHeader from '@/components/ui/MgsPageHeader.vue'
import MgsToolbar from '@/components/ui/MgsToolbar.vue'
import MgsPageContainer from '@/components/ui/MgsPageContainer.vue'

type CategoryNode = CategoryDto & { children?: CategoryNode[] }

const categoryApi = new CategoryApi(apiConfiguration)
const categories = ref<CategoryDto[]>([])
const loading = ref(false)
const saving = ref(false)
const dialogVisible = ref(false)
const editing = ref<CategoryDto | null>(null)
const keyword = ref('')
const form = ref({ name: '', desc: '', parentId: '' })

const categoryById = computed(() => new Map(
  categories.value.filter(category => category.uid).map(category => [category.uid!, category]),
))

function buildTree(source: CategoryDto[]): CategoryNode[] {
  const nodes = new Map<string, CategoryNode>()
  for (const category of source) {
    if (category.uid) nodes.set(category.uid, { ...category, children: [] })
  }

  const roots: CategoryNode[] = []
  for (const node of nodes.values()) {
    const parentNode = node.parentId ? nodes.get(node.parentId) : undefined
    if (parentNode && parentNode.uid !== node.uid) parentNode.children!.push(node)
    else roots.push(node)
  }
  return roots
}

function filterTree(nodes: CategoryNode[], query: string): CategoryNode[] {
  if (!query) return nodes
  return nodes.flatMap(node => {
    const children = filterTree(node.children ?? [], query)
    const ownText = [node.categoryName, node.categoryDescription, node.parentId, node.uid]
      .some(value => value?.toLocaleLowerCase().includes(query))
    return ownText || children.length ? [{ ...node, children }] : []
  })
}

const tableData = computed(() => filterTree(buildTree(categories.value), keyword.value.trim().toLocaleLowerCase()))

const availableParents = computed(() => {
  if (!editing.value?.uid) return categories.value
  return categories.value.filter(candidate => {
    if (candidate.uid === editing.value?.uid) return false
    let current: CategoryDto | undefined = candidate
    const visited = new Set<string>()
    while (current?.parentId && !visited.has(current.parentId)) {
      if (current.parentId === editing.value?.uid) return false
      visited.add(current.parentId)
      current = categoryById.value.get(current.parentId)
    }
    return true
  })
})

async function loadCategories() {
  loading.value = true
  try {
    const result = await categoryApi.apiCategoryGet({ searchType: CategoryQueryType.File })
    categories.value = result.items ?? []
  } catch (error) {
    ElMessage.error(error instanceof Error ? error.message : '分类数据加载失败')
  } finally {
    loading.value = false
  }
}

function openCreate() {
  editing.value = null
  form.value = { name: '', desc: '', parentId: '' }
  dialogVisible.value = true
}

function openEdit(category: CategoryDto) {
  editing.value = category
  form.value = {
    name: category.categoryName ?? '',
    desc: category.categoryDescription ?? '',
    parentId: category.parentId ?? '',
  }
  dialogVisible.value = true
}

async function saveCategory() {
  const name = form.value.name.trim()
  if (!name) {
    ElMessage.warning('请填写分类名称')
    return
  }

  saving.value = true
  try {
    if (editing.value?.uid) {
      const command: UpdateCategoryCommand = {
        uid: editing.value.uid,
        name,
        desc: form.value.desc.trim(),
        parentId: form.value.parentId || null,
      }
      await categoryApi.apiCategoryPut({ updateCategoryCommand: command })
      ElMessage.success('分类已更新')
    } else {
      const command: CreateCategoryCommand = {
        name,
        desc: form.value.desc.trim(),
        parentId: form.value.parentId || null,
      }
      await categoryApi.apiCategoryPost({ createCategoryCommand: command })
      ElMessage.success('分类已创建')
    }

    dialogVisible.value = false
    await loadCategories()
  } catch (error) {
    ElMessage.error(error instanceof Error ? error.message : '保存分类失败')
  } finally {
    saving.value = false
  }
}

async function deleteCategory(category: CategoryDto) {
  if (!category.uid) return
  try {
    await ElMessageBox.confirm(
      `确定删除分类「${category.categoryName || '未命名分类'}」吗？仍有子分类、文件或收藏使用时，服务器会拒绝删除。`,
      '删除分类确认',
      {
        type: 'warning',
        confirmButtonText: '确认删除',
        cancelButtonText: '取消',
        distinguishCancelAndClose: true,
      },
    )
    await categoryApi.apiCategoryDelete({ uID: category.uid })
    ElMessage.success('分类已删除')
    await loadCategories()
  } catch (error) {
    if (error === 'cancel' || error === 'close') return
    ElMessage.error(error instanceof Error ? error.message : '删除失败：分类可能仍被业务数据使用')
  }
}

onBeforeMount(loadCategories)
</script>

<template>
  <div class="category-view">
    <MgsPageHeader title="分类管理" description="维护独立的树形分类体系，供 File 与 Favorite 业务选择">
      <template #actions>
        <ElButton :icon="Refresh" :loading="loading" @click="loadCategories">刷新</ElButton>
        <ElButton type="primary" :icon="Plus" @click="openCreate">新增分类</ElButton>
      </template>
    </MgsPageHeader>

    <MgsToolbar>
      <ElInput v-model="keyword" clearable placeholder="搜索分类名称、描述或 UID" />
    </MgsToolbar>

    <MgsPageContainer>
      <ElTable
        v-if="tableData.length"
        :data="tableData"
        v-loading="loading"
        row-key="uid"
        :tree-props="{ children: 'children' }"
        default-expand-all
        empty-text="暂无分类"
        class="category-table"
      >
        <ElTableColumn prop="categoryName" label="分类名称" min-width="180">
          <template #default="{ row }">
            <span class="category-name">{{ row.categoryName || '未命名分类' }}</span>
          </template>
        </ElTableColumn>
        <ElTableColumn prop="categoryDescription" label="描述" min-width="220" show-overflow-tooltip>
          <template #default="{ row }">{{ row.categoryDescription || '—' }}</template>
        </ElTableColumn>
        <ElTableColumn label="业务数量" width="110" align="center">
          <template #default="{ row }"><ElTag type="info">{{ row.count ?? 0 }}</ElTag></template>
        </ElTableColumn>
        <ElTableColumn prop="uid" label="UID" min-width="220" show-overflow-tooltip />
        <ElTableColumn label="操作" width="150" fixed="right">
          <template #default="{ row }">
            <ElButton link type="primary" :icon="Edit" @click="openEdit(row)">编辑</ElButton>
            <ElButton link type="danger" :icon="Delete" @click="deleteCategory(row)">删除</ElButton>
          </template>
        </ElTableColumn>
      </ElTable>
      <ElEmpty v-else-if="!loading" description="暂无分类，点击“新增分类”创建第一个分类" />
    </MgsPageContainer>

    <ElDialog
      v-model="dialogVisible"
      :title="editing ? '编辑分类' : '新增分类'"
      width="min(560px, 92vw)"
      destroy-on-close
    >
      <ElForm label-position="top">
        <ElFormItem label="分类名称" required>
          <ElInput v-model="form.name" maxlength="32" show-word-limit placeholder="请输入分类名称" />
        </ElFormItem>
        <ElFormItem label="分类描述">
          <ElInput
            v-model="form.desc"
            type="textarea"
            :rows="3"
            maxlength="256"
            show-word-limit
            placeholder="可选，介绍该分类的用途"
          />
        </ElFormItem>
        <ElFormItem label="父分类">
          <ElSelect v-model="form.parentId" clearable class="full-width" placeholder="无（顶级分类）">
            <ElOption
              v-for="category in availableParents"
              :key="category.uid"
              :label="category.categoryName ?? '未命名分类'"
              :value="category.uid ?? ''"
            />
          </ElSelect>
          <div class="field-hint">不选择父分类时，该分类将作为顶级分类。</div>
        </ElFormItem>
      </ElForm>
      <template #footer>
        <ElButton @click="dialogVisible = false">取消</ElButton>
        <ElButton type="primary" :loading="saving" @click="saveCategory">保存</ElButton>
      </template>
    </ElDialog>
  </div>
</template>

<style scoped>
.category-table { width: 100%; }
.category-name { font-weight: 600; color: var(--mgs-text); }
.full-width { width: 100%; }
.field-hint { margin-top: 6px; color: var(--mgs-muted); font-size: 12px; line-height: 1.5; }
</style>
