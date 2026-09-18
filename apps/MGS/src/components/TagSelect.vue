<template>
  <ElSelect v-model="tagListModel" multiple :disabled="props.selectDisabled" placeholder="选择 Tag" :fit-input-width="true"
    value-key="uid">
    <ElOption v-for="item in tagSelectList" :key="item.uid as PropertyKey" :label="item.name!" :value="item" />
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
  ElMessage,
  ElButton,
  ElInput,
  ElOption,
  ElSelect,
} from 'element-plus'
import { onBeforeMount, type Ref, ref } from 'vue'
import { TagApi } from '@/api'
import { apiConfiguration } from '@/services/api'
// 组件内部属性
const tagApi = new TagApi(apiConfiguration)


/**
 * @argument tagList 提供已选择的标签列表
 */
const props = defineProps({
  selectDisabled: { type: Boolean, default: false, required: false }
})
const tagListModel = defineModel<TagDto[]>('tagList')

const tagSelectList: Ref<TagDto[]> = ref<TagDto[]>([])
// 组件方法

// ================================添加新的Tag
const isAdding = ref<boolean>()
const tempTag = ref<string>()// 临时tag
async function commit() {
  try {
    ElMessage.warning('正在创建永久 Tags,若要删除,请移步至 tag 管理面板')

    const tempUid = await tagApi.apiTagPost({ createTagCommand: { name: tempTag.value } })
    const tempTagDto = await tagApi.apiTagGet({ uID: tempUid })
    tagSelectList.value.push(tempTagDto[0])
    cancel()
  } catch {
    ElMessage.error('创建永久Tag 失败')
  }
}

function AddTag() {
  ElMessage.warning(
    '您正在创建临时标签,若点击 commit 则会创建永久标签',
  )
  isAdding.value = !isAdding.value
}

function cancel() {
  tempTag.value = ''
  isAdding.value = !isAdding.value
}
// 提供选择列表
onBeforeMount(async () => {
  tagSelectList.value = await tagApi.apiTagGet()
})

</script>
