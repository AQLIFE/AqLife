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
import { TagApi, TodoApi, type CreateTagCommand, type TodoDto } from '@/api'
import { apiConfiguration } from '@/services/api'
import { useTodoStorage } from '@/stores/useTodoStore'

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

const todoStore = useTodoStorage();
const proxyTags = ref<TodoDto[]>([])

const isAdd:Ref<boolean> = ref<boolean>(false)

const todoApi = new TodoApi(apiConfiguration)

onBeforeMount(async () => {
  if (todoStore.todoList.length === 0) {
    todoStore.todoList = await todoApi.apiTodoGet();
  }
  proxyTags.value = [...todoStore.todoList]
})

const tableColumns = computed(() => {
  if (!todoStore.todoList || todoStore.todoList.length === 0) {
    return []
  }
  return Object.keys(todoStore.todoList[0])
})

const tempList = computed(() => {
  let list = Object.keys(columnMap)
  list.pop()
  return list
})

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
            <el-button :icon="isAdd?Upload:Search" :disabled="activeSelect == '选择字段'" :type="isAdd?'success':'info'"/>
          </template>
        </ElInput>
      </ElCol>

      <ElSegmented v-model="selectCategory" :options="segmentedOptions"  />
      <ElButton :icon="Plus" :title="isAdd?'点击一次打开添加表单':'再点一次关闭表单并上传'"/>
    </ElCol>


    <ElTable :data="proxyTags" row-key="uid" class="fitHeight" highlight-current-row v-if="todoStore.todoList && todoStore.todoList.length > 0" :tree-props="{ children: 'todoList',hasChildren:'hasChildren' }">
      <ElTableColumn :sortable="index != (tableColumns.length - 1)" :prop="item" v-for="(item, index) in tableColumns"
        :key="item" :label="columnMap[item] ?? item">
        <template #default="scope" v-if="index === 1">
          <!-- <ElTag v-if="scope.row.name">{{ scope.row.name }}</ElTag> -->
          <ElText>{{ scope.row.ftid??'无' }}</ElText>
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
