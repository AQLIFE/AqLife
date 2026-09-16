<script setup lang="ts">
import type { FileDto, TagDto } from '@/api'
import { Plus } from '@element-plus/icons-vue';
import { ElIcon } from 'element-plus';
defineProps<{row:FileDto,
  value?: TagDto[] | null
}>()
</script>

<template>
  <template v-if="value?.length">
    <ElTag v-for="tag in value" :key="tag.uid" class="gap">
      {{ tag.name }}
    </ElTag>
  </template>

  <ElTag v-else type="warning" effect="plain" title="暂未设置标签,点击设置">
    <ElIcon>
      <Plus />
    </ElIcon>
  </ElTag>
</template>

<style scoped>
.gap {
  margin-right: 10px;
}
</style>
