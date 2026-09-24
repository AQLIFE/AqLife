<script setup lang="ts">
import {
  ElTable,
  ElSegmented,
  ElTag,
  ElForm,
  ElFormItem,
  ElSwitch,
  ElText,
  ElEmpty,
  ElTableColumn,
  ElSelect,
  ElOption,
  ElInput,
  ElButton,
  ElCol,
  ElMessage,
  ElRow,
} from 'element-plus'
import { Plus, Search, Upload } from '@element-plus/icons-vue'
import { onBeforeMount, computed, ref, type Ref } from 'vue'
import { TagApi, type CreateTagCommand, type TagDto } from '@/api'
import { apiConfiguration } from '@/services/api'
import { useTagStore } from '@/stores/uuseTagStore'

const columnMap: Record<string, string> = {
  uid: '唯一标识',
  name: '标签名称',
  aliasName: '标签别名',
  isCategory: '分类标识',
}

const activeSelect = ref<string>('选择字段')
const searchQuery = ref<string>('')
const selectCategory = ref<number>(-1)

const segmentedOptions = [
  { label: 'Clear', value: -1 },
  { label: 'normal', value: 0 },
  { label: 'category', value: 1 },
]

const tagStore = useTagStore()
const proxyTags = ref<TagDto[]>([])

const addTagDto = ref<CreateTagCommand>({
  name: '',
  aliasName: '',
  isCategory: false,
})
const isAdd: Ref<boolean> = ref(false)

const tagApi = new TagApi(apiConfiguration)

onBeforeMount(async () => {
  await tagStore.fetchAllTags(tagApi)
  proxyTags.value = [...tagStore.tags]
})

const tableColumns = computed(() =>
  tagStore.tags.length > 0 ? Object.keys(tagStore.tags[0]) : [],
)

const tempList = computed(() => {
  const list = Object.keys(columnMap)
  list.pop()
  return list
})

function filterData() {
  let result = [...tagStore.tags]

  if (selectCategory.value !== -1) {
    const targetBool = Boolean(Number(selectCategory.value))
    result = result.filter(e => e.isCategory === targetBool)
  }

  if (searchQuery.value.trim() !== '') {
    const field = activeSelect.value as keyof TagDto
    const keyword = searchQuery.value.trim().toLowerCase()

    result = result.filter(e => {
      const val = e[field]
      return val !== undefined && String(val).toLowerCase().includes(keyword)
    })
  }

  proxyTags.value = result
}

function handleRadioChange() {
  filterData()
}

function handleSearch() {
  filterData()
}

async function addTag() {
  if (isAdd.value && addTagDto.value.name?.trim()) {
    try {
      const tagUid = await tagApi.apiTagPost({
        createTagCommand: addTagDto.value,
      })
      const result = await tagApi.apiTagGet({ uID: tagUid })
      const newTag = result.items?.[0]

      if (!newTag) {
        throw new Error('创建 Tag 后未找到对应数据')
      }

      tagStore.tags.push(newTag)
      proxyTags.value = [...tagStore.tags]
      ElMessage.success('上传 Tag 成功')

      addTagDto.value = {
        name: '',
        aliasName: '',
        isCategory: false,
      }
    } catch (error) {
      ElMessage.error('上传 Tag 失败')
      console.error('Create Tag Error:', error)
    }
  }

  isAdd.value = !isAdd.value
}
</script>

<template>
  <ElRow>
    <ElCol class="flex">
      <ElCol :span="6">
        <ElInput
          v-model="searchQuery"
          :disabled="activeSelect === '选择字段'"
        >
          <template #prepend>
            <ElSelect
              v-model="activeSelect"
              :placeholder="activeSelect"
              style="width: 100px"
            >
              <ElOption
                v-for="(item, index) in tempList"
                :key="index"
                :label="columnMap[item]"
                :value="item"
              />
            </ElSelect>
          </template>
          <template #append>
            <ElButton
              :icon="isAdd ? Upload : Search"
              :disabled="activeSelect === '选择字段'"
              :type="isAdd ? 'success' : 'info'"
              @click="handleSearch"
            />
          </template>
        </ElInput>
      </ElCol>

      <ElSegmented
        v-model="selectCategory"
        :options="segmentedOptions"
        @change="handleRadioChange"
      />
      <ElButton
        :icon="Plus"
        @click="addTag"
        :title="isAdd ? '点击一次关闭表单并上传' : '点击一次打开添加表单'"
      />
    </ElCol>

    <ElForm v-if="isAdd" inline class="inline-form">
      <ElFormItem :label="Object.values(columnMap)[1]">
        <ElInput v-model="addTagDto.name" clearable />
      </ElFormItem>
      <ElFormItem :label="Object.values(columnMap)[2]">
        <ElInput
          v-model="addTagDto.aliasName"
          clearable
          :disabled="!addTagDto.name"
        />
      </ElFormItem>
      <ElFormItem :label="Object.values(columnMap)[3]">
        <ElSwitch
          v-model="addTagDto.isCategory"
          inline-prompt
          active-text="标识为 分类"
        />
      </ElFormItem>
    </ElForm>

    <ElTable
      :data="proxyTags"
      class="fitHeight"
      highlight-current-row
      v-if="tagStore.tags.length > 0"
    >
      <ElTableColumn
        v-for="(item, index) in tableColumns"
        :key="item"
        :prop="item"
        :label="columnMap[item] ?? item"
        :sortable="index !== tableColumns.length - 1"
      >
        <template #default="scope">
          <ElTag v-if="item === 'name' && scope.row.name">
            {{ scope.row.name }}
          </ElTag>
          <ElText v-else>
            {{ scope.row[item] }}
          </ElText>
        </template>
      </ElTableColumn>
    </ElTable>

    <ElEmpty v-else description="正在加载标签数据..." />
  </ElRow>
</template>

<style lang="css" scoped>
.flex {
  display: flex;
  padding: 20px;
  justify-content: space-around;
  align-items: center;
}

.inline-form {
  padding: 20px;
  width: 100%;
  background-color: #DCDCDC;
  box-shadow: inset #DCDCDC 0px 3px 3px;
}

.inline-form > :deep(.el-form-item) {
  margin-bottom: 0px;
}
</style>
