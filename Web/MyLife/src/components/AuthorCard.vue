<template>
    <ElCard id="author" shadow="hover">
        <ElRow align="middle">
            <ElCol id="avatar" :span="8">
                <ElImage :src="author.userInfo.avatar_url" />
            </ElCol>
            <ElCol id="info" :span="16">
                <ElRow>
                    <ElCol id="name" title="点击直达作者github主页">
                        <ElLink :href="author.userInfo.html_url" target="_blank">
                            <ElIcon :icon="GithubIcon"/>
                                <!-- <GithubIcon />
                            </ElIcon> -->
                            {{ author.userInfo.name }}
                        </ElLink>

                    </ElCol>
                    <ElDivider title="个人概述" />
                    <ElCol id="desc">{{ author.userInfo.bio }}</ElCol>
                </ElRow>
            </ElCol>
        </ElRow>
    </ElCard>
</template>

<script lang="ts" setup>
import GithubIcon from '@/assets/icons/GithubIcon.svg';
import { AuthorInfo } from '@/services/storer';
import { ElImage, ElRow, ElCol, ElDivider, ElIcon, ElLink,ElCard } from 'element-plus';

const author = AuthorInfo();
 
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

.el-icon{margin-right: 10px;}
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