<template>
    <ElCard id="author" shadow="hover">
        <ElRow align="middle">
            <ElCol id="avatar" :span="8">
                <ElImage src="">
                    <template #error>
                        <el-icon>
                            <Picture />
                        </el-icon>
                    </template>
                </ElImage>
            </ElCol>
            <ElCol id="info" :span="16">
                <ElRow>
                    <ElCol id="name" title="点击直达作者github主页">
                        <!-- <ElLink :href="author.userInfo.html_url" target="_blank"> -->
                        <el-icon>
                            <User />
                        </el-icon>
                        {{ author.userInfo.name }}
                        <!-- </ElLink> -->

                    </ElCol>
                    <ElCol id="desc">{{ author.userInfo.desc }}</ElCol>
                    <ElDivider title="个人概述" />
                    <ElCol style="max-height:30px;line-height:30px;overflow: hidden;">
                        <ElRow
                            style="align-content:flex-start;height: inherit;overflow: hidden;align-items: start;align-self: flex-start;max-height: initial;"
                            justify="space-evenly">
                            <ElLink :href="item.subscriptionLink ?? ''"
                                v-for="(item, index) in author.userInfo.subscriptions ?? []" target="_blank"
                                :key="index">
                                <ElImage :src="previewUrl(item.subscriptionIcon ?? '')"
                                    style="width: 20px; height: 20px;" />
                            </ElLink>
                        </ElRow>
                    </ElCol>
                </ElRow>
            </ElCol>
        </ElRow>
    </ElCard>
</template>

<script lang="ts" setup>
import { Picture, User } from '@element-plus/icons-vue'
import { AuthorInfo } from '@/services/storer';
import { ElImage, ElRow, ElCol, ElDivider, ElIcon, ElLink, ElCard } from 'element-plus';

const author = AuthorInfo();
const previewUrl = (title: string) => `http://localhost:5110/api/file/preview?title=${title}&id`
</script>

<style scoped>
#author {
    background-color: white;
    overflow: hidden;
}

#author .el-row {
    min-height: 162.13px;
    height: inherit;
}

#avatar {
    font-size: 0;
}

.el-icon {
    position: relative;
    top: 5px;
    margin-right: 3px;
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
    width: auto;
    justify-content: left;
}

#name>* {
    font-size: 1.2rem;
    line-height: 1.2rem;
}


#desc {
    font-size: 0.7rem;
    padding: 5px;
}

.el-divider {
    padding: 0;
    margin: 20px 0;
}
</style>