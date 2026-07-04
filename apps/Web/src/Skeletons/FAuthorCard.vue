<template>
    <ElSkeleton id="author" :animated="true" :throttle="300" :loading="AuthorInfo().isShow">
        <template #template>
            <div class="template">
                <ElSkeletonItem variant="image" class="avatar" />
                <ElSkeletonItem variant="text" class="authorInfo" />
                <ElSkeletonItem variant="text" class="authorInfo" />
            </div>
            <div class="template">
                <ElSkeletonItem variant="image" class="demoIcon" />
                <ElSkeletonItem variant="image" class="demoIcon" />
                <ElSkeletonItem variant="image" class="demoIcon" />
                <ElSkeletonItem variant="image" class="demoIcon" />
                <ElSkeletonItem variant="image" class="demoIcon" />
            </div>
            <div class="template">
                <ElSkeletonItem variant="text" class="demo" />
            </div>
        </template>

        <template #default>
            <AuthorCard />
        </template>
    </ElSkeleton>
</template>

<script lang="ts" setup>

import { AuthorInfo } from '@/services/storage/AuthorInfo';
import { onBeforeMount } from 'vue';
import AuthorCard from '@/components/AuthorCard.vue';

onBeforeMount(async () => {
    await AuthorInfo().getUser()
})

</script>

<style scoped>
#author {
    display: grid;
    overflow: hidden;
    background-color: var(--topColor);
}

#author .template:nth-child(even) {
    margin: -20px 0px;
}


/*-------------------设置第1个template的布局---------------------*/
/* 1. 父级网格：3行2列 */
#author .template:nth-child(1) {
    display: grid;
    grid-template-columns: 6vw 1fr;
    /* 第1列放头像，第2列放文本 */
    grid-template-rows: repeat(3, 1fr);
    /* 均匀分成3行 */
    gap: 10px;
    /* 加上间距 */
    align-items: center;
    /* 让文本在行内垂直居中 */
}

/* 2. 头像：高个子，独占左侧第1列，纵跨前2行（或者3行，取决于你想让他多高） */
#author .template:nth-child(1) .avatar {
    grid-row: 1 / span 3;
    /* 占第 1、2、3 行 */
    grid-column: 1 / span 1;
    /* 只占第 1 列 */
    width: 6vw;
    height: 6vw;
}

/* 3. 右侧第一行文本：利用 :nth-of-type 精准定位 */
#author .template:nth-child(1) .authorInfo:nth-child(2) {
    grid-row: 1 / span 2;
    /* 位于第 1 行 */
    grid-column: 2 / span 1;
    /* 位于第 2 列 */
    height: 2vh;
    width: 5vw;
}

#author .template:nth-child(1) .authorInfo:nth-child(3) {
    grid-row: 3 / span 1;
    /* 位于第 2 行 */
    grid-column: 2 / span 1;
    /* 位于第 2 列 */
    width: 75%;
    /* 第二行文本通常长一点，显得逼真 */
    height: 2vh;
}

/*-------------------设置第3个template的布局---------------------*/
#author .template:nth-child(3) .demo {
    /* #author .template:nth-child(3) .demo */
    width: 65%;
    height: 1vw;
}

/*-------------------设置第2个template的布局---------------------*/
#author .template:nth-child(2) {
    display: grid;
    grid-template-columns: 3vw 3vw 3vw 3vw 3vw 3vw;
}

.demoIcon {
    width: 2vw;
    height: 2vw;
}
</style>