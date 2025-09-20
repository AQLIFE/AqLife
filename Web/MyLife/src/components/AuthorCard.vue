<template>
    <ElCol id="author">
        <ElRow flex="row" wrap="nowrap" justify="start" align="middle">
            <ElCol id="avatar" :span="8">
                <ElImage :src="author.userInfo.avatar_url" />
            </ElCol>
            <ElCol id="info" :span="16">
                <ElRow flex="column" wrap="nowrap">
                    <ElCol id="name" :icon="GithubIcon" title="点击直达作者github主页">
                        <ElLink :href="author.userInfo.html_url" target="_blank">
                            <ElIcon>
                                <GithubIcon />
                            </ElIcon>
                            {{ author.userInfo.name }}
                        </ElLink>

                    </ElCol>
                    <ElDivider title="个人概述" />
                    <ElCol id="desc">{{ author.userInfo.bio }}</ElCol>
                </ElRow>
            </ElCol>
        </ElRow>
    </ElCol>
</template>

<script lang="ts" setup>
import GithubIcon from '@/assets/icons/GithubIcon.vue';
import { AuthorInfo } from '@/services/storer';
import { ElImage, ElRow, ElCol, ElDivider, ElIcon, ElLink } from 'element-plus';
import { onBeforeMount } from 'vue';

const author = AuthorInfo();
 
onBeforeMount(():void=>{
    author.getUserInfo('aqlife');
    console.log(author.userInfo.login)
})

</script>

<style scoped>
#author {
    background-color: white;
    overflow: hidden;
}

#author .el-row {
    height: initial;
}

#avatar {
    font-size: 0;
}


.el-button {
    border: 0px;
}

#info {
    display: flex;
    flex-direction: column;
    min-height: 2vw;
    padding: 0;
    margin: 0;
}

#info .el-row {
    padding: 0;
    margin: 0;
    height: auto;
    display: flex;
    flex-direction: column;
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