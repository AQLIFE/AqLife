<template>
  <ElText>{{ ouput }}</ElText>
</template>
<script setup lang="ts">
import type { FileDto } from '@/api';
import { computed } from 'vue';
const scope = defineProps<{row:FileDto}>()
const ouput = computed(()=> scope.row.fileSize! > 1024?
  (scope.row.fileSize as number / 1024).toFixed(3) +' KB'
  : scope.row.fileSize+' B')
</script>
