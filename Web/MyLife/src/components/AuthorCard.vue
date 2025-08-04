<template>
    <ElCol id="author">
        <ElRow flex="row" wrap="nowrap" justify="start" align="middle">
            <ElCol id="avatar" :span="8">
                <ElImage src="/src/assets/b_6584376235e266a00019954e8c2e7d26.jpg" />
            </ElCol>
            <ElCol id="info" :span="16">
                <ElRow flex="column" wrap="nowrap">
                    <ElCol id="name">AQ</ElCol>
                    <ElDivider title="个人概述" />
                    <ElCol id="desc">喜欢Web,虽然只是略懂</ElCol>
                </ElRow>
            </ElCol>
        </ElRow>
    </ElCol>
</template>

<script lang="ts" setup>
import { ElCard, ElImage, ElRow, ElCol, ElDivider } from 'element-plus';
</script>

<style scoped>
#author {
    background-color: white;
    overflow: hidden;
}

#author .el-row{
    height: initial;
}

#avatar{font-size: 0;}



#info{display: flex;flex-direction:column;min-height: 2vw;padding: 0;margin: 0;}
#info .el-row{padding: 0;margin: 0;height: auto;display: flex;flex-direction: column;gap:0;}
#name {
    padding-left: 20px;
    text-align: left;
    font-size: 1rem;
    padding-bottom: 20px;
}

#desc{font-size: 0.7rem; padding: 5px;}
.el-divider{padding: 0;margin: 0;}
</style>