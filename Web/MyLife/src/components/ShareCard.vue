<script setup lang="ts">
import { ElButton, ElImage } from 'element-plus';
const props = defineProps<{src:string}>();
</script>

<template>
    <ElButton>
        <ElImage alt="phind icon" :src="props.src"/>
    </ElButton>
</template>

<style scoped>
.el-button{
    padding:3vh 5vw !important;
    height:auto;width: auto;
    background-color: rgba(0,0,0,0);
    border:1px dashed rgba(0,0,0,0);
    overflow: hidden;
}
.el-button:hover{
    background-color: rgba(0,0,0,0);
    border:1px dashed black;
}
</style>