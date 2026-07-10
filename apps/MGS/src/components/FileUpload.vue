<template>
  <ElDescriptions border :column="1">
    <ElDescriptionsItem label="上传">
      <ElUpload
        v-model:file-list="fileList"
        :accept="acceptType"
        drag
        multiple
        list-type="picture"
        show-file-list
        :auto-upload="false"
      >
        <ElIcon class="fillIcon"><UploadFilled /></ElIcon>
        <div class="el-upload__text">Drop file here or <em>click to upload</em></div>
        <template #tip> 仅允许{{ acceptType }}类型文件上传 </template>
        <template #file="{ file, index }">
          <!-- Elemnt-plus 在 TS 开发下,插槽参数被降级为 Any,非常狗血 -->
          <ElImage class="image" :src="isImageType(file) ? file.url : ''" :key="index">
            <template #error>
              <ElIcon><Files /></ElIcon>
            </template>
          </ElImage>
          <ElText class="maxText">{{ file.name }}</ElText>
        </template>
      </ElUpload>
    </ElDescriptionsItem>
    <ElDescriptionsItem label="标签">
      <ElSelect
        v-model="activeItem"
        multiple
        :disabled="selectDisabled"
        placeholder="选择 Tag"
        :fit-input-width="true"
      >
        <ElOption
          v-for="(item, index) in tags"
          :key="index"
          :label="item.name!"
          :value="item.uid!"
        />
        <template #footer>
          <el-button v-if="!isAdding" text bg size="small" @click="tempAddTag">
            Add an option
          </el-button>
          <template v-else>
            <el-input
              v-model="tempTag"
              class="option-input"
              placeholder="input new tag"
            />
            <el-button type="primary" size="small" @click="commit"> confirm </el-button>
            <el-button size="small" @click="cancel">cancel</el-button>
          </template>
        </template>
      </ElSelect>
    </ElDescriptionsItem>
  </ElDescriptions>
</template>

<script setup lang="ts">
import { computed, onBeforeMount, reactive, ref, type Ref } from 'vue'
import { TagApi, type FileMetadataDto, type TagDto } from '@/api'
import { UploadFilled, Files, Plus } from '@element-plus/icons-vue'
import { normalizeExtension } from '@aqlife/domain'
import ImageUpload from '@/components/ImageUpload.vue'
import {
  ElDescriptions,
  ElImage,
  ElOption,
  ElButton,
  ElInput,
  ElIcon,
  ElSelect,
  ElOptionGroup,
  ElDescriptionsItem,
  type UploadUserFile,
  type UploadFile,
  ElMessage,
} from 'element-plus'
import { defaultFilePolicy } from '@aqlife/domain'
import { apiConfiguration } from '@/services/api'
import { useTagStore } from '@/stores/uuseTagStore'

const tagStorage =  useTagStore();
const props = defineProps<{ fileDto: FileMetadataDto; columnMap: Record<string, string> }>()
const fileList = ref<UploadUserFile[]>([])
const acceptType = computed(() => {
  const fileTypes = defaultFilePolicy.allowedUpload.join(',')
  // console.log(fileTypes) // 输出: ".svg,.jpeg,.jpg,.png"
  return fileTypes
})

const selectDisabled = computed(() => fileList.value.length <= 0)
const imageExtensions = computed(() => {
  return defaultFilePolicy.allowedUpload.filter((item) => item !== '.md') // 过滤掉不需要的 .md，只留下图片
})

const isImageType = (f: UploadFile) => {
  // const f = file  // 顺手顺顺类型
  if (!f.name) return false

  // 提取后缀名并转为小写
  const ext = f.name.substring(f.name.lastIndexOf('.')).toLowerCase()
  // 检查提取出的后缀是否存在于图片白名单中
  return imageExtensions.value.includes(ext)
}
const isAdding = ref<boolean>()
  const tempTag = ref<string>()
function cancel(){
  tempTag.value = ''
  isAdding.value=!isAdding.value
}
const activeItem = ref<TagDto>()
const tags = ref<TagDto[]>()
const indexKey = Object.keys(activeItem)[0]

onBeforeMount(async () => {
  const tagApi = new TagApi(apiConfiguration)
  tags.value = await tagApi.apiTagGet()
})

function tempAddTag(){
  ElMessage.warning('您正在创建临时标签,若点击 commit 则会创建永久标签,在此处删除标签将不会被真正移除!')
  isAdding.value = !isAdding.value

}
async function commit(){
  try{
    ElMessage.warning('正在创建永久 Tags,若要删除,请移步至 tag 管理面板')
    const tagApi = new TagApi(apiConfiguration)
    const tempUid = await tagApi.apiTagPost({createTagCommand:{tagName:tempTag.value}})
    const tempTagDto = await tagApi.apiTagGet({uID:tempUid})
    tags.value?.push(tempTagDto[0])
    tempTag.value = ''
    isAdding.value = !isAdding.value

  }catch{
    ElMessage.error('创建永久Tag 失败')
    console.log('tags=>',tags)
    console.log('StoreTag=>',tagStorage.tags)

  }
}
</script>

<style lang="css" scoped>
.fillIcon {
  font-size: 5rem;
}
.image {
  max-height: 6vw;
  max-width: 6vw;
  min-width: 6vw;
  min-height: 6vw;
}

.image :deep(.el-icon) {
  font-size: 5vw;
  position: relative;
  top: 0.5vw;
}
.maxText {
  max-width: calc(100% - 10vw);
  overflow: hidden;
  padding: 20px;
}
</style>
