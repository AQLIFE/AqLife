<template>
    <ElButton :icon="props.icon" type="text" @click="click(props.routeObj as RouteRecordRaw)">{{ props.btitle }}
    </ElButton>
</template>

<script setup lang="ts">
import { ElButton } from 'element-plus';
import { type Component, type PropType } from 'vue';
import { type RouteRecordRaw } from 'vue-router';

const props = defineProps({
    btitle: String, routeObj: Object as () => RouteRecordRaw, icon: {
        type: [String, Object] as PropType<string | Component>,
        required: false
    }
});
const emits = defineEmits<{ (e: 'click', routeObj: RouteRecordRaw): void }>()

const click = (obj: RouteRecordRaw): void => emits('click', obj);
</script>

<style lang="css" scoped>
.el-button {
    border: 0px;
}
</style>