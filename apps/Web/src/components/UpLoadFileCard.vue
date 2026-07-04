<script setup lang="ts">
import { reactive } from 'vue';

import { ElCard, ElRow, ElCol,ElUpload ,ElButton} from 'element-plus';

const item = reactive({})
</script>
<style scope>
.upload{
    display: flex;
    flex-direction: column;
    background-color: var(--back_color_lv1);
}

</style>

<template>
    <el-upload @name="item" :show-file-list="true" class="upload">
        <el-button type="primary">选择并上传</el-button>
        <template #tip>
            请上传=> markdown 文件
            
        </template>
    </el-upload>
</template>