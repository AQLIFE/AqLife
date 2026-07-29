<script setup lang="ts">
import { useTagStore } from '@/stores/uuseTagStore'
import {
  ElTable,
  ElSegmented, // 1. 引入 ElSegmented 替代 Radio
  ElTag,
  ElForm,ElFormItem,ElSwitch,
  ElText,
  ElEmpty,
  ElTableColumn,
  ElImage,
  ElSelect,
  ElOption,
  ElInput,
  ElButton,
  ElCol,
  ElDrawer,
  type TableInstance,
  ElMessage,
  ElRow,
} from 'element-plus'
import { Plus, Search, Upload } from '@element-plus/icons-vue'
import { onBeforeMount, computed, ref,reactive ,type Ref} from 'vue'
import { TagApi, type CreateTagCommand, type TagDto } from '@/api'
import { apiConfiguration } from '@/services/api'

const columnMap: Record<string, string> = {
  uid: '唯一标识',
  name: '标签名称',
  aliasName: '标签别名',
  isCategory: '分类标识',
}
const activeSelect = ref<string>('选择字段')
const searchQuery = ref<string>('')     // 新增：用于接收输入框的值
const selectCategory = ref<number>(-1) // 默认不分类 -1 , 0 筛选普通标签 , 1 筛选有分类标签

// 2. 定义 Segmented 的配置项
const segmentedOptions = [
  { label: 'Clear', value: -1 },
  { label: 'normal', value: 0 },
  { label: 'category', value: 1 }
]

const tagStore = useTagStore()
const proxyTags = ref<TagDto[]>([])

const addTagDto = ref<CreateTagCommand>({
    name: '',
    aliasName: '',
    isCategory: false
})
const isAdd:Ref<boolean> = ref<boolean>(false)

const tagApi = new TagApi(apiConfiguration)

onBeforeMount(async () => {
  if (tagStore.tags.length === 0) {
    tagStore.tags = await tagApi.apiTagGet()
  }
  proxyTags.value = [...tagStore.tags]
})

const tableColumns = computed(() => {
  if (!tagStore.tags || tagStore.tags.length === 0) {
    return []
  }
  return Object.keys(tagStore.tags[0])
})

const tempList = computed(() => {
  let list = Object.keys(columnMap)
  list.pop()
  return list
})

function filterData() {
  let result = [...tagStore.tags]

  // 1. 优先处理分段控制器的筛选 (isCategory)
  if (selectCategory.value !== -1) {
    const targetBool = Boolean(Number(selectCategory.value))
    result = result.filter(e => e.isCategory === targetBool)
  }

  // 2. 叠加处理输入框的文本模糊搜索
  if (searchQuery.value.trim() !== '') {
    const field = activeSelect.value as keyof TagDto
    const keyword = searchQuery.value.trim().toLowerCase()

    result = result.filter(e => {
      // 确保字段存在并且转为字符串进行模糊匹配
      const val = e[field]
      return val !== undefined && String(val).toLowerCase().includes(keyword)
    })
  }

  proxyTags.value = result
}

// 当分段切换时，直接调用统一过滤
function handleRadioChange() {
  filterData()
}

// 当点击搜索按钮或回车时触发
function handleSearch() {
  filterData()
}

async function addTag() {
  if (isAdd.value&& addTagDto.value.name!=null&&addTagDto.value.name!='' ) {
    // 默认此时isAdd : true
    const tagGid = await tagApi.apiTagPost({createTagCommand:addTagDto.value})
    const newTag = await tagApi.apiTagGet({uID:tagGid})
    tagStore.tags.push(newTag[0])
    ElMessage.success('上传 Tag 成功')
    proxyTags.value = tagStore.tags
    addTagDto.value.aliasName = ''
    addTagDto.value.name = ''
    addTagDto.value.isCategory = false
  }
  isAdd.value = !isAdd.value
}
</script>

<template>
  <ElRow>
    <ElCol class="flex">
      <ElCol :span="6">
        <ElInput :disabled="activeSelect == '选择字段'" v-model="searchQuery">
          <template #prepend>
            <ElSelect v-model="activeSelect" :placeholder="activeSelect" style="width:100px;">
              <ElOption v-for="(item, index) in tempList" :label="columnMap[item]" :value="item" :key="index" />
            </ElSelect>
          </template>
          <template #append>
            <el-button :icon="isAdd?Upload:Search" :disabled="activeSelect == '选择字段'" @click="handleSearch" :type="isAdd?'success':'info'"/>
          </template>
        </ElInput>
      </ElCol>

      <ElSegmented v-model="selectCategory" :options="segmentedOptions" @change="handleRadioChange" />
      <ElButton :icon="Plus" @click="addTag" :title="isAdd?'点击一次打开添加表单':'再点一次关闭表单并上传'"/>
    </ElCol>
    <ElForm v-if="isAdd" inline class="inline-form">
      <ElFormItem :label="Object.values(columnMap)[1]">
        <ElInput v-model="addTagDto.name" clearable/>
      </ElFormItem>
      <ElFormItem :label="Object.values(columnMap)[2]">
        <ElInput v-model="addTagDto.aliasName" clearable :disabled="!addTagDto.name"/>
      </ElFormItem>
      <ElFormItem :label="Object.values(columnMap)[3]">
        <ElSwitch v-model="addTagDto.isCategory" inline-prompt active-text="标识为 分类"/>
      </ElFormItem>


    </ElForm>

    <ElTable :data="proxyTags" class="fitHeight" highlight-current-row v-if="tagStore.tags && tagStore.tags.length > 0">
      <ElTableColumn :sortable="index != (tableColumns.length - 1)" :prop="item" v-for="(item, index) in tableColumns"
        :key="item" :label="columnMap[item] ?? item">
        <template #default="scope" v-if="index === 1">
          <ElTag v-if="scope.row.name">{{ scope.row.name }}</ElTag>
          <ElText v-else>{{ scope.row.name }}</ElText>
        </template>
      </ElTableColumn>
    </ElTable>
    <ElEmpty v-else description="正在加载标签数据..." />
  </ElRow>
</template>

<style lang="css" scoped>
.flex{
  display: flex; padding: 20px; justify-content: space-around; align-items: center;
}
.inline-form{
  padding: 20px;
  width: 100%;
  background-color: #DCDCDC;
  box-shadow:inset #DCDCDC 0px 3px 3px;
}
.inline-form > :deep(.el-form-item){
  margin-bottom: 0px;
}
</style>
