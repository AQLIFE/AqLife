<template>
    <ElCol class="card">
        <ElInput ref="inputRef" :prefix-icon="Search" type="text" clearable placeholder="你想搜点什么"  v-model="demo">
            <template #append>
                <ElCol>
                    <span class="key">Ctrl+K</span>
                </ElCol>
            </template>
        </ElInput>
    </ElCol>
</template>

<script lang="ts" setup>
import { ElCol, ElInput } from 'element-plus';
import { Search } from '@element-plus/icons-vue';
import { onMounted, ref } from 'vue';

const demo = ref('');
const inputRef = ref();

onMounted(() => {
    window.addEventListener('keydown', (e) => {
        if (e.key === 'k' && (e.metaKey || e.ctrlKey)) {
            e.preventDefault();//取消事件的默认动作
            inputRef.value.focus();
        }
    });
});
</script>

<style scoped>

.el-input {
    height: 50px;
    line-height: 50px;
}

.el-input >>> .el-input__wrapper{border: 0px none;box-shadow: none !important;background-color:var(--back_color_lv1) !important;}
.el-input >>> .el-input__wrapper>*{background-color: var(--back_color_lv1);}
.el-input >>> .el-input__wrapper:hover{box-shadow: none !important;border: none;}
.el-input >>> .el-input__wrapper:focus{box-shadow: none !important;border: none;}
.el-input >>> .el-input-group__append{
    background-color:var(--back_color_lv1) !important;
}

.card{
    border: 1px solid transparent;
    transition: box-shadow 0.2s, border-color 0.2s;
}
.card:hover,.el-input__wrapper >>> .el-input__inner:focus{
    border:1px solid rgb(78, 142, 47);
    transition: box-shadow 0.2s, border-color 0.2s;
}

.key{
    border: 1px dashed var(--back_color_lv1);
    border-radius: 25px !important;
    padding: 5px 10px;
    background-color: white;
}
</style>