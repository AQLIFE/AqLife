<template>
    <ElButton :icon="props.icon" type="text" @click="handleClick">{{ props.btitle }}
    </ElButton>
</template>

<script setup lang="ts">
import { ElButton } from 'element-plus';
import { type Component,type PropType } from 'vue';
import {type NavItemDto} from '@/types/navigation'

const props = defineProps({
    btitle: {type:String,required:true}, 
    routeObj: {type:Object as PropType<NavItemDto>,required:true},
    icon: {
        type: [Object, String]  as import('vue').PropType<Component | string>,
        required: false
    }
});
const emits = defineEmits<{(e: 'click', item: NavItemDto): void}>();

const handleClick = (): void => {emits('click', props.routeObj);};

</script>

<style lang="css" scoped>
.el-button {
    border: 0px;
    /* 建议添加：确保图标和文字对齐 */
    display: inline-flex;
    align-items: center;
}
</style>