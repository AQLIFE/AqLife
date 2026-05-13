<template>
    <ElSkeleton id="author" :animated="true" :throttle="300" :loading="author.isShow">
        <template #template>
            <ElRow align="middle">
                <ElCol id="avatar" :span="8">
                    <ElSkeletonItem variant="image" style="width: 100%; height:162.13px;" />
                </ElCol>
                <ElCol id="info" :span="16">
                    <ElRow flex="column" wrap="nowrap">
                        <ElCol id="name">
                            <ElSkeletonItem variant="image" style="width: 10%;margin-right: 10px;" />
                            <ElSkeletonItem variant="text" style="width:20%;" />
                        </ElCol>
                        <ElDivider title="个人概述" />
                        <ElCol id="desc">
                            <ElSkeletonItem variant="text" style="width: 60%; height:15px" />
                        </ElCol>
                    </ElRow>
                </ElCol>
            </ElRow>
        </template>

        <template #default>
            <AuthorCard />
        </template>
    </ElSkeleton>
</template>

<script lang="ts" setup>
import { AuthorInfo, ApiOption } from '@/services/storer';
import { ElRow, ElCol, ElDivider, ElSkeletonItem } from 'element-plus';
import { onBeforeMount } from 'vue';
import AuthorCard from './AuthorCard.vue';
import { AccountApi } from '@/api/generated';
import { handle } from '@/utils/request';

const author = AuthorInfo();

const accountapi = new AccountApi(ApiOption);

onBeforeMount(async () => {
    const [response, status] = await handle(accountapi.apiAccountGet());
    if (status) author.userInfo = response!;
})

</script>

<style scoped>
#author {
    background-color: white;
    overflow: hidden;
}

#author .el-row {
    height: 162px;
}

#avatar {
    font-size: 0;
}


.el-button {
    border: 0px;
}

#info {
    min-height: 2vw;
    padding: 0;
    margin: 0;
}

#info .el-row {
    padding: 0;
    margin: 0;
    height: auto;
    gap: 0;
}

#name {
    display: flex;
    width: auto;
    justify-content: center;
}

#name>* {
    display: inline-block;
    height: 30px;
    font-size: 0px;
    line-height: 30px;
}


.el-divider {
    padding: 0;
    margin: 20px 0;
}
</style>