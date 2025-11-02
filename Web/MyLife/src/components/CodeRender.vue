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
// import type { CodeInfo } from '@/data/SkillTagData';
// import { defineComponent } from 'vue';
// import { onMounted,type Ref,ref } from 'vue';
const props = defineProps<{ CodeType: string}>()
// const content :Ref<CodeInfo> = ref({CodeType:'',CodeLines:[]})

// onMounted(()=>{
//     const codeDom = document.getElementsByClassName('codeContent')
//     // console.log(codeDom[0],codeDom[0].children.length==0,codeDom[0].innerHTML)
//     content.value = getCode(codeDom[0].innerHTML)
//     console.log(content.value)

// })

// function getCode(sourceStr:string):CodeInfo {
//     const decodedText = sourceStr.replace(/&gt;/g, '>');
//     // 匹配语言类型（修复后的正则）
//     const RegexCodeType: RegExp = /```([\w]{1,})/;
    
//     // 匹配代码块（修复后的正则）
//     const RegexCode: RegExp = /```[\w\S]{1,}([\s\S]*?)```/;
    
    
//     const codeType:string = decodedText.match(RegexCodeType)?.[1]||'txt'
//     // console.log(decodedText,decodedText.match(RegexCode),decodedText.match(RegexCodeType))

//     const codeLines:string[] = decodedText.match(RegexCode)?.[1].split('\n')||[]
//     return {CodeType:codeType,CodeLines:codeLines}
    
// }
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