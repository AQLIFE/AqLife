<template>
  <ElIcon size="30px" :color="color">
    <component :is="icon" />
  </ElIcon>
</template>

<script lang="ts" setup>
import { Brush, Promotion, Timer, Warning } from '@element-plus/icons-vue'
import { computed } from 'vue'
import { FileStatus } from '@/api'

const props = defineProps<{ value: FileStatus }>()
const icon = computed(() =>{
  switch (props.value){
    case FileStatus.Draft:   return Brush;
    case FileStatus.Scheduled:return Timer;
    case FileStatus.Published:return Promotion;
    default:return Warning;}
})

const color = computed(():string =>{
   switch (props.value){
    case PublishStatus.Draft:   return 'rgb(115, 118, 122)';
    case PublishStatus.Scheduled:return 'rgb(51, 126, 204)';
    case PublishStatus.Published:return 'rgb(82, 155, 46)';
    default:return 'rgb(184, 130, 48)';}
})
</script>
