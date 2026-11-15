<script setup lang="ts">
import { computed, onBeforeMount, ref } from 'vue'
import { ElOption, ElSelect } from 'element-plus'
import { notify } from '@aqlife/ui-shared'
import { CategoryApi, CategoryQueryType, type CategoryDto } from '@/api'
import { apiConfiguration } from '@/services/api'

const props = defineProps({
  selectDisabled: { type: Boolean, default: false },
})

const categoryId = defineModel<string>('categoryId', { required: true })
const categoryApi = new CategoryApi(apiConfiguration)
const categories = ref<CategoryDto[]>([])
const loading = ref(false)

const categoryOptions = computed(() => {
  const byId = new Map(
    categories.value
      .filter(category => category.uid)
      .map(category => [category.uid!, category]),
  )

  return categories.value
    .filter(category => category.uid)
    .map(category => {
      const names = [category.categoryName?.trim() || '未命名分类']
      const visited = new Set<string>([category.uid!])
      let parentId = category.parentId ?? undefined

      while (parentId && !visited.has(parentId)) {
        visited.add(parentId)
        const parent = byId.get(parentId)
        if (!parent) break
        names.unshift(parent.categoryName?.trim() || '未命名分类')
        parentId = parent.parentId ?? undefined
      }

      return { value: category.uid!, label: names.join(' / ') }
    })
    .sort((left, right) => left.label.localeCompare(right.label, 'zh-CN'))
})

async function loadCategories() {
  loading.value = true
  try {
    const result = await categoryApi.apiCategoryGet({ searchType: CategoryQueryType.Default })
    categories.value = result.items ?? []
  } catch (error) {
    notify.error(error instanceof Error ? error.message : '分类列表加载失败')
  } finally {
    loading.value = false
  }
}

onBeforeMount(loadCategories)
</script>

<template>
  <ElSelect
    v-model="categoryId"
    class="category-select"
    filterable
    clearable
    :loading="loading"
    :disabled="props.selectDisabled"
    :placeholder="categoryOptions.length ? '请选择文件分类' : '暂无可用分类，请先创建分类'"
  >
    <ElOption
      v-for="option in categoryOptions"
      :key="option.value"
      :label="option.label"
      :value="option.value"
    />
  </ElSelect>
</template>

<style scoped>
.category-select {
  width: 100%;
}
</style>
