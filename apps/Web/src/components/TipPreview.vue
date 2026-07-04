<script setup lang="ts">
import { computed } from 'vue'
import MarkdownIt from 'markdown-it'
import type Token from 'markdown-it/lib/token.d.mts'

const props = defineProps<{
  quoteTokens: Token[]
}>()

const md = new MarkdownIt()
const renderedHtml = computed(() => md.renderer.render(props.quoteTokens, md.options, {}))
</script>

<template>
  <div class="tip-block-container">
    <div class="tip-content" v-html="renderedHtml"></div>
  </div>
</template>

<style scoped>
.tip-block-container {
  /* 需求实现：独立的背景颜色 [cite: 215] */
  background-color: #fffbe6; /* 浅黄色背景，类似警告提示 */
  /* 需求实现：左侧黄色竖条 */
  border-left: 5px solid #fadb14;

  margin: 16px 0;
  padding: 12px 20px;
  border-radius: 4px;
}

/* 穿透渲染出的 HTML 样式 [cite: 5] */
.tip-content :deep(p) {
  margin: 0;
  color: #856404; /* 深褐色文字，提升对比度 */
  font-size: 14px;
  line-height: 1.6;
}

.tip-content :deep(strong) {
  color: #533f03;
}
</style>
