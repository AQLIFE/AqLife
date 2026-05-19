<template>
    <ElRow id="Home">{{ corpus }}</ElRow>
</template>

<script setup lang="ts">
import { onBeforeMount,ref,type Ref} from 'vue'
import { CorpusApi } from '@/api/generated';
import { ElRow } from 'element-plus';
import { ApiOption } from '@/services/storage/BaseOptions';

// const Quotes = [
// 	'距离产生美,保持距离',
// 	'我爱双休',
//     '诚惶诚恐,逐浪无依',
//     '蜉蝣幻梦,昙花一现',
//     '有限生命怎敢度量无限时空',
//     '天体自由',
//     '在证明自己这件事上,没有错,但也没有用',
//     '下班了,就别看工作上的事了,看不完滴'
// ]

const corpusAPI = new CorpusApi(ApiOption)
const corpus:Ref<string> = ref('')

onBeforeMount(async ()=>{
    corpus.value = await corpusAPI.apiCorpusRandomGet()
})

// 随机显示一句话
// const idx = Math.floor(Math.random() * Quotes.length);

</script>

<style scoped>
/* #Home {
    flex-grow: 1;
} */
</style>
