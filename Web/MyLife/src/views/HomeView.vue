<template>
    <ElRow id="Home">{{Quotes[idx] }}</ElRow>
</template>

<script setup lang="ts">
import { ElRow } from 'element-plus';

const Quotes = [
	'距离产生美,保持距离',
	'我爱双休',
    '诚惶诚恐,逐浪无依',
    '蜉蝣幻梦,昙花一现',
    '有限生命怎敢度量无限时空',
    '天体自由'
]

// 随机显示一句话
const idx = Math.floor(Math.random() * Quotes.length);

</script>

<style scoped>
#Home {
    background-color: var(--back_color_lv1);
    flex-grow: 1;
}
</style>
