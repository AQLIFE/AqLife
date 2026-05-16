<template>
    <ElCard id="author" shadow="hover">
        <div class="template">
            <ElImage src="" class="avatar">
                <template #error>
                    <el-icon>
                        <Picture />
                    </el-icon>
                </template>
            </ElImage>
            <ElCol class="authorInfo">
                <ElIcon style="position: relative;top:6px;">
                    <User />
                </ElIcon>
                {{ author.name }}
            </ElCol>
            <ElCol class="authorInfo">{{ author.desc }}</ElCol>
        </div>
        <div class="template">
            <ElLink target="_blank" :href="item.subscriptionLink??'' "
                v-for="(item, index) in author.subscriptions ?? []" :key="index">
                <ElImage :src="item.subscriptionIcon!=null? previewUrl(item.subscriptionIcon!):''" class="demoIcon">
                    <template #error>
                        <ElIcon><Picture/></ElIcon>
                    </template>
                </ElImage>
            </ElLink>
        </div>
        <div class="template">
            <ElCol class="demo">正在编写NET ...</ElCol>
        </div>
    </ElCard>
</template>

<script lang="ts" setup>
import { Picture, User } from '@element-plus/icons-vue'
import { AuthorInfo } from '@/services/storer';
import { onBeforeMount,ref} from 'vue'
import { ElImage, ElCol, ElIcon, ElLink, ElCard } from 'element-plus';
import {type AccountDto } from '@/api/generated';

const author = ref<AccountDto>({});
const previewUrl = (title: string) => `http://localhost:5110/api/file/preview?title=${title}&id`

onBeforeMount(()=>{
    author.value = AuthorInfo().userInfo;
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
    min-width: 6vw; max-width: 6vw;
    min-height: 6vw;max-height: 6vw;
}


#author .template:nth-child(1) .avatar >.el-icon {
    min-width: 6vw; max-width: 6vw;
    min-height: 6vw;max-height: 6vw;
    font-size: 4rem;
}

/* 3. 右侧第一行文本：利用 :nth-of-type 精准定位 */
#author .template:nth-child(1) .authorInfo:nth-child(2) {
    grid-row: 1 / span 2;
    /* 位于第 1 行 */
    grid-column: 2 / span 1;
    /* 位于第 2 列 */
    height: fit-content;
    font-size: 2rem;
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
    grid-template-columns: 2vw 2vw 2vw 2vw 2vw 2vw;
    gap: 1vw;
}

.demoIcon {
    max-width: 2vw;
    min-width: 2vw;
    max-height: 2vw;
    min-height: 2vw;
}
</style>