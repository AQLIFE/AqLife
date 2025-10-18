<template>
    <ElCard shadow="hover" class="card"  title="按下Ctrl+K快速聚焦到搜索框">
        <ElInput ref="inputRef" :prefix-icon="Search" type="text" clearable placeholder="你想搜点什么" v-model="demo" tabindex="1">
            <template #append>
                <ElCol>

                    <span class="key">
                        <ElIcon>
                            <ButtonIcon />
                        </ElIcon>
                        Ctrl+Q
                    </span>
                </ElCol>
            </template>
        </ElInput>
    </ElCard>
</template>

<script lang="ts" setup>
import { ElCol,ElCard, ElInput } from 'element-plus';
import { Search } from '@element-plus/icons-vue';
import { onMounted, ref } from 'vue';
import ButtonIcon from '@/assets/icons/ButtonIcon.svg';

const demo = ref('');
const inputRef = ref();

onMounted(() => {
    window.addEventListener('keydown', (e) => {
        if (e.key === 'q' && (e.metaKey || e.ctrlKey)) {
            e.preventDefault();//取消事件的默认动作
            inputRef.value.focus();
        }
    });
});
</script>

<style scoped>
.el-input {
    height: 6vh;
    line-height:6vh;
}

.el-input :deep(.el-input__wrapper) {
    border: 0px none;
    box-shadow: none !important;
    /* background-color: var(--back_color_lv1) !important; */
}

.el-input :depp(.el-input__wrapper)>* {
    /* background-color: var(--back_color_lv1); */
    padding-left: 1vw;
}

.el-input :deep(.el-input__wrapper:hover) {
    box-shadow: none !important;
    border: none;
}

.el-input :deep(.el-input__wrapper:focus) {
    box-shadow: none !important;
    border: none;
}

.el-input :deep(.el-input-group__append) {
    background-color: white !important;
    box-shadow: none !important;;
}

.card {
    border: 1px solid transparent;
    transition: box-shadow 0.2s, border-color 0.2s;
}

.card:hover,
.el-input__wrapper :deep(.el-input__inner:focus) {
    border: 1px solid rgb(78, 142, 47);
    transition: box-shadow 0.2s, border-color 0.2s;
}

.key {
    border: 1px dashed var(--back_color_lv2);
    border-radius: 25px !important;
    padding: 5px 10px;
    background-color: white;
}

.el-icon{margin-right:2px;display: inline-block;font-size: 15px;position: relative;top:2px;}
</style>