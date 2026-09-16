<template>
  <ElImage v-if="isImageType(props.row.fileType!)" class="image" :src="fileStore.previewUrl.get(props.row.uid!)">
    <template #error>加载中...</template>
  </ElImage>
  <ElImage v-else class="image" style="font-size: 5vw;">
    <template #error>
      <ElIcon>
        <component :is="mgsIconRegistry[MgsIconName.Markdown]" />
      </ElIcon>
    </template>
  </ElImage>
  <!-- <ElCol>{{ props.row.uid }}</ElCol> -->
</template>

<script setup lang="ts">
import { type FileDto } from '@/api'
import { MgsIconName, mgsIconRegistry } from '@aqlife/icons';
import { isImageType } from '@aqlife/domain'
import { useFileStore } from '@/stores/useFileStore';
const props = defineProps<{row:FileDto,}>()

const fileStore = useFileStore()
</script>
