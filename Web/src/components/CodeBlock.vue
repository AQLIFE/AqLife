<template>
  <div class="code">
    <div class="codeHeader">
      <div class="codeType">{{ props.infoType || 'text' }}</div>
      <div class="codeTitle"></div>
      <div class="copyButton" title="点此复制以下代码" @click="onCopy">Copy</div>
    </div>

    <div class="codeContent">
      <pre><code class="hljs" v-html="highlightedHTML"></code></pre>
    </div>
  </div>
</template>

<script setup lang="ts">
// import { Copy } from '@element-plus/icons-vue'
import { ref, computed } from 'vue'
import { ElMessage } from 'element-plus'
import hljs from 'highlight.js'
// 引入你喜欢的代码高亮主题（highlight.js 内置了几十种主题，可以去 node_modules 里挑）
import 'highlight.js/styles/atom-one-dark.css'

// 1. 接收从父组件传来的纯文本代码和语言类型
const props = defineProps<{
  info: string
  infoType: string
}>()

const copied = ref(false)

// 2. 核心：利用 highlight.js 将纯文本转换为带颜色标签的 HTML
const highlightedHTML = computed(() => {
  const lang = props.infoType
  const code = props.info || ''

  // 检查 highlight.js 是否支持该语言，支持则高亮，不支持则返回纯文本
  if (lang && hljs.getLanguage(lang)) {
    try {
      return hljs.highlight(code, { language: lang }).value
    } catch {}
  }
  // 降级处理：如果没有匹配语言，转义防止 XSS 后直接输出
  return hljs.highlightAuto(code).value // 或者简单的文本转义
})

// 3. 复制功能稍作调整：直接复制传进来的 RawCode 纯文本
const onCopy = async () => {
  if (!props.info) return

  try {
    await navigator.clipboard.writeText(props.info)
    ElMessage({
      showClose: true,
      message: '代码已复制到剪贴板,若有建议请发信私聊,感谢支持!',
      type: 'success',
    })
    copied.value = true
    setTimeout(() => {
      copied.value = false
    }, 1500)
  } catch {
    ElMessage.error('复制失败，当前访问暂不支持')
  }
}
</script>

<style scoped>
/* 你的原有样式保持不变 */
.code {
  background-color: var(--back_color_lv1);
  border-radius: 8px;
  overflow: hidden;
  margin: 1rem 0;
}

.codeHeader {
  display: flex;
  flex-direction: row;
  line-height: 40px;
  background-color: #2d2d2d; /* 稍微深一点的头部颜色 */
  color: #ccc;
  font-size: 0.9rem;
}

.codeHeader .codeType {
  padding: 0 1vw;
  text-transform: uppercase;
}

.codeHeader .codeTitle {
  flex-grow: 1;
}

.codeHeader .copyButton {
  padding: 0 1vw;
  cursor: pointer;
  transition: color 0.2s;
}

.codeHeader .copyButton:hover {
  color: #fff;
}

.codeContent {
  background-color: var(--back_color_lv3);
  margin: 0;
}

.codeContent pre {
  margin: 0;
  padding: 1rem;
  overflow-x: auto;
}
</style>
