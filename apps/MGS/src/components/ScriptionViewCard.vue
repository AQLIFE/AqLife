<template>
  <ElCard
    shadow="hover"
    @click.right.prevent="isEdit = !isEdit"
    @click.middle.prevent="$emit('delete', serial)"
  >
    <template #header>
      <ImageUpload :status="!isEdit" iconSize="8vw" />
    </template>
    <ElCol>
      <ElInput
        v-model="item.aliasName"
        :class="isEdit ? '' : 'noneBorder'"
        placeholder="name"
        :prefix-icon="User"
        :disabled="!isEdit"
      />
    </ElCol>

    <ElCol>
      <ElInput
        v-model="item.subscriptionPlatform"
        :class="isEdit ? '' : 'noneBorder'"
        placeholder="platform"
        :prefix-icon="Platform"
        :disabled="!isEdit"
      />
    </ElCol>

    <ElCol>
      <ElInput
        v-model="item.subscriptionLink"
        :class="isEdit ? '' : 'noneBorder'"
        placeholder="link"
        :prefix-icon="Link"
        type="url"
        :disabled="!isEdit"
      />
    </ElCol>
  </ElCard>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { ElCard, ElCol, ElInput, ElUpload, ElImage, ElIcon } from 'element-plus'
import { User, Platform, Link, Plus } from '@element-plus/icons-vue'
import type { SubscriptionDto } from '@/api'
import ImageUpload from './ImageUpload.vue';

// 接收整个 item 对象和 serial 索引
const props = defineProps<{
  serial: number
  item: SubscriptionDto // 接收父组件传递过来的单条订阅数据
}>()

const preview = (guid: string | null | undefined): string =>
  guid ? `${import.meta.env.VITE_API}/api/File/preview?UID=${guid}` : ''

const isEdit = ref<boolean>(false)
defineEmits(['delete'])
</script>

<style lang="css" scoped>
/* 使用 :deep() 穿透到 Element Plus 内部的 wrapper 容器 */
.noneBorder :deep(.el-input__wrapper) {
  background-color: transparent !important; /* 移除背景色 */
  box-shadow: none !important; /* Element Plus 的边框其实是 box-shadow 实现的，必须设为 none */
  border: none !important; /* 保险起见清除 border */
}

/* 选填：如果你希望禁用时里面的文字、图标颜色不要太淡，可以顺便调整它们 */
.noneBorder :deep(.el-input__inner) {
  -webkit-text-fill-color: var(--el-text-color-regular) !important; /* 保持文字颜色不灰暗 */
  color: var(--el-text-color-regular) !important;
}

.noneBorder :deep(.el-input__prefix-icon) {
  color: var(--el-text-color-regular) !important; /* 保持前缀图标颜色 */
}
</style>
