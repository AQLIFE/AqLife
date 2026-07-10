<template>
  <ElRow style="height: 120px;">
    <ElButton :disabled="!actionStore.onAdd" @click="actionStore.onAdd?.()">Add</ElButton>
    <ElButton :disabled="!actionStore.onUpdate" @click="actionStore.onUpdate?.()">Update</ElButton>
    <ElButton :disabled="!actionStore.onDelete" @click="actionStore.onDelete?.()">Delete</ElButton>
    <ElButton>View</ElButton>
  </ElRow>
</template>

<script setup lang="ts">
import { ElButton,ElRow } from 'element-plus';
import { useActionStore } from '@/stores/useActionStore'
const actionStore = useActionStore()
</script>
