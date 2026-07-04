<template>
  <div class="code">
    <div class="codeHeader">
      <div class="codeType">{{ props.infoType || 'text' }}</div>
      <div class="codeTitle"></div>
      <ElButton
        class="copyButton"
        title="点此全屏预览"
        :icon="FullScreen"
        @click="isFullscreen = true"
      />
    </div>

    <div class="codeContent">
      <div class="mermaid-viewer-container">
        <div v-if="svgHtml" class="mermaid-svg-wrapper" v-html="svgHtml" />

        <div v-else-if="hasError" class="mermaid-error-fallback">
          <div class="error-header">⚠️ 抱歉，Mermaid 图表渲染失败，请检查语法</div>
          <pre><code>{{ props.info }}</code></pre>
        </div>

        <div v-else class="mermaid-loading">
          <span>正在绘制图表...</span>
        </div>
        <ElDialog
          v-model="isFullscreen"
          fullscreen
          destroy-on-close
          title="图表详情预览"
          custom-class="mermaid-fullscreen-dialog"
        >
          <div class="fullscreen-content" v-html="svgHtml"></div>
        </ElDialog>
      </div>
    </div>
  </div>
</template>
<script setup lang="ts">
import { FullScreen } from '@element-plus/icons-vue'
import mermaid from 'mermaid'
import { ElButton, ElDialog } from 'element-plus'
import { nextTick, onMounted, ref } from 'vue'
const props = defineProps<{
  info: string
  infoType: string
}>()
const isFullscreen = ref(false)
const svgHtml = ref('')
// const svgCode = ref(''); // 存储生成的 SVG 代码

const hasError = ref(false)
const uniqueChartId = `mermaid-svg-${Math.floor(Math.random() * 10000000)}`
mermaid.initialize({
  startOnLoad: false, // 🔒 必须关闭：禁止全局扫描，改由我们手动精准控制
  securityLevel: 'loose', // 允许一些交互或相对宽松的标签渲染
  theme: 'default', // 主题配置：'default' | 'dark' | 'forest' | 'neutral'
})

const drawDiagram = async () => {
  if (!props.info) return

  try {
    hasError.value = false

    // 清理掉首尾的多余换行符
    const codeText = props.info.trim()

    /**
     * 🚨 核心避坑点 2：调用官方最底层、最干净的异步 render API
     * mermaid.render(id, text) 会在后台计算好布局，直接返回生成的 SVG 字符串
     * { svg } 就是我们要的 HTML 片段
     */
    const { svg } = await mermaid.render(uniqueChartId, codeText)

    // 渲染成功，喂给 Vue 的 v-html
    svgHtml.value = svg
  } catch (error) {
    console.error('Mermaid 渲染发生错误:', error)
    hasError.value = true
    svgHtml.value = ''

    /**
     * 🚨 核心避坑点 3：Mermaid 的异常拦截清理
     * 当 Mermaid 遇到语法错误渲染失败时，它会在浏览器的 <body> 尾部残留一个
     * 带有恶意错误信息的临时绑定节点（如 #dmermaid-svg-xxxx）。
     * 为了不污染全局 DOM 树，失败时我们手动去把它捞出来删掉。
     */
    nextTick(() => {
      const badElement = document.getElementById(`d${uniqueChartId}`)
      if (badElement) {
        badElement.remove()
      }
    })
  }
}

onMounted(() => {
  drawDiagram()
})
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
  background-color: var(--back_color_lv5); /*微深一点的头部颜色 */
  color: rgb(255, 255, 255);
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
  background-color: rgba(0, 0, 0, 0);
  border: 0px;
  height: initial;
}

.codeHeader .copyButton:hover {
  color: #fff;
}

.codeContent {
  background-color: var(--topColor);
  margin: 0;
}

.codeContent pre {
  margin: 0;
  padding: 1rem;
  overflow-x: auto;
}
</style>
