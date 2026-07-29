<template>
  <ElSelect
    v-model="tagListModel"
    multiple
    :disabled="selectDisabled"
    placeholder="选择 Tag"
    :fit-input-width="true"
    value-key="uid"
  >
    <ElOption
      v-for="(item, index) in tagSelectList"
      :key="item.uid as PropertyKey"
      :label="item.name!"
      :value="item"
    />
    <template #footer>
      <el-button v-if="!isAdding" text bg size="small" @click="AddTag"> Add an option </el-button>
      <template v-else>
        <el-input v-model="tempTag" class="option-input" placeholder="input new tag" />
        <el-button type="primary" size="small" @click="commit"> confirm </el-button>
        <el-button size="small" @click="cancel">cancel</el-button>
      </template>
    </template>
  </ElSelect>
</template>

<script setup lang="ts">
import type { TagDto } from '@/api'
import {
  ElImage,
  ElMessage,
  ElUpload,
  ElIcon,
  ElButton,
  ElInput,
  ElOption,
  ElSelect,
} from 'element-plus'
import { onBeforeMount, type Ref, ref } from 'vue'
import { TagApi } from '@/api'
import { apiConfiguration } from '@/services/api'
// 组件内部属性

const isAdding = ref<boolean>()
const tempTag = ref<string>()

// 依赖属性
/**
 * @argument tagList 提供已选择的标签列表
 */
const props = defineProps({
  selectDisabled: { type: Boolean, default: false,required:false }
})
const tagListModel = defineModel<TagDto[] | null | undefined>('tagList')

const tagApi = new TagApi(apiConfiguration)
const tagSelectList: Ref<TagDto[]> = ref<TagDto[]>([])
// 组件方法
async function commit() {
  try {
    ElMessage.warning('正在创建永久 Tags,若要删除,请移步至 tag 管理面板')

    const tempUid = await tagApi.apiTagPost({ createTagCommand: { name: tempTag.value } })
    const tempTagDto = await tagApi.apiTagGet({ uID: tempUid })
    tagSelectList.value?.push(tempTagDto[0])
    tempTag.value = ''
    isAdding.value = !isAdding.value
  } catch {
    ElMessage.error('创建永久Tag 失败')
    // console.log('tags=>',tags)
    // console.log('StoreTag=>',tagStorage.tags)
  }
}

function AddTag() {
  ElMessage.warning(
    '您正在创建临时标签,若点击 commit 则会创建永久标签,在此处删除标签将不会被真正移除!',
  )
  isAdding.value = !isAdding.value
}

function cancel() {
  tempTag.value = ''
  isAdding.value = !isAdding.value
}

// 组件生命周期
onBeforeMount(async () => {
  tagSelectList.value = await tagApi.apiTagGet()
})
</script>
