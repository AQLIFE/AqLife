<script setup lang="ts">
defineProps<{
    blogTitle: string
    description?: string
    updateTime?: string
    htmlContent: string
}>()
</script>

<template>
    <KeepAlive>
        <ElCol class="blog">
            <ElCol class="viewTitle">{{ blogTitle }}</ElCol>
            <ElCol class="description">
                <ElRow>
                    <ElCol>{{ description }}</ElCol>
                    <ElCol>Update:{{ updateTime }}</ElCol>
                </ElRow>
            </ElCol>
            <div id="blogContent" v-html="htmlContent" />
        </ElCol>
    </KeepAlive>
</template>



<style scoped>
.blogTitle {
    font-size: 2rem;
    font-weight: bold;
    margin: 2vh 0;
    text-align: center;
}

.description {
    font-size: 1rem;
    text-align: center;
}

.description>.el-row {
    justify-content: center;
    height: auto;
    font-size: small;
    color: var(--back_color_lv4);
    background-color: var(--back_color_lv2);
}

#blogContent {
    margin: 4vh 8vw;
    line-height: 2rem;
    font-size: 1.2rem;
    height: 100vh !important;
}
</style>

<style>
.row {
    display: flex;
    flex-direction: row;
    flex-wrap: nowrap;
    margin: 0;
    padding: 0;
}

.CodeLine {
    display: inline-block;
    width: 20px;
    padding: 0 2vh;
    border-right: 1px dashed var(--back_color_lv5);
    color: var(--back_color_lv5);
}

span.txt {
    padding: 0;
    margin: 0;
}

.code {
    background-color: var(--back_color_lv1);
    display: flex;
    flex-direction: column;
    flex-wrap: nowrap;
}

.codeHeader {
    display: flex !important;
    flex-direction: row;
    line-height: 50px;
    max-height: 50px;
}

.codeHeader>.codeType {
    display: flexbox;
    padding: 0 1vw;

}

.codeHeader>.codeTitle {
    display: flexbox;
    flex-grow: 1;
    text-align: center;
}

.codeHeader>.copyButton {
    background-color: var(--back_color_lv3);
    padding: 0 1vw;
}

.codeContent {
    background-color: var(--back_color_lv3);
    overflow-y: hidden;
    overflow-x: auto;
    flex-grow: 1;
    display: flex;
    flex-direction: column;
    justify-items: start;
}
</style>