<template>
    <div class="code">
        <div class="codeHeader">
            <div class="codeType">{{ props.CodeType }}</div>
            <div class="codeTitle"></div>
            <div class="copyButton" title="点此复制以下代码">Copy</div>
        </div>
        <div class="codeContent">
            <slot />
        </div>
    </div>
</template>
<script setup lang="ts">

const props = defineProps<{ CodeType: string}>()


const copyCode = async (code: string) => {
    try {
        await navigator.clipboard.writeText(code)
        ElMessage({
            showClose: true,
            message: '代码已复制到剪贴板,若有建议请发信私聊,感谢支持!',
            type: 'success',
        })
        copied.value = true
        // 1.5秒后重置状态
        setTimeout(() => {
            copied.value = false
        }, 1500)
    } catch{
        ElMessage.error('复制失败，当前访问暂不支持')
        // console.error('复制失败:', '用户未授权')
    }
}
</script>

<style scoped>
.code {
    background-color: var(--back_color_lv1);
    
}

.code .codeHeader {
    display: flex;
    flex-direction: row;
    line-height: 50px;
}
.codeHeader .codeType{
    display: flexbox;
    padding: 0 1vw;
    
}
.codeHeader .codeTitle {
    display: flexbox;
    flex-grow: 1;
    text-align: center;
}

.codeHeader .copyButton {
    background-color: var(--back_color_lv3);
    padding: 0 1vw;
}

.codeContent{
    background-color: var(--back_color_lv3);

}
</style>