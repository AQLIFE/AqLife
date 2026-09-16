<template>
  <ElIcon size="30px" :color="color">
    <component :is="icon" />
  </ElIcon>
</template>

<script lang="ts" setup>
import { Brush, Promotion, Timer, Warning } from '@element-plus/icons-vue';
import { computed } from 'vue';


enum PublishStatus {Draft,Scheduled,Published}
const props = defineProps<{value:PublishStatus}>()
const icon = computed(() =>{
  switch (props.value){
    case PublishStatus.Draft:   return Brush;
    case PublishStatus.Scheduled:return Timer;
    case PublishStatus.Published:return Promotion;
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
