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
          class="mermaid-fullscreen-dialog"
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
import {
  nextTick,
  onBeforeUnmount,
  ref,
  watch,
} from 'vue'

const props = defineProps<{
  info: string
  infoType: string
}>()

const isFullscreen = ref(false)
const svgHtml = ref('')
const hasError = ref(false)

let renderTimer: ReturnType<typeof setTimeout> | undefined
let renderVersion = 0

mermaid.initialize({
  startOnLoad: false,
  securityLevel: 'loose',
  theme: 'default',
})

const drawDiagram = async () => {
  const codeText = props.info.trim()

  if (!codeText) {
    svgHtml.value = ''
    hasError.value = false
    return
  }

  const currentVersion = ++renderVersion
  const chartId = `mermaid-${Date.now()}-${currentVersion}`

  try {
    hasError.value = false

    const { svg } = await mermaid.render(
      chartId,
      codeText,
    )

    // 旧请求完成得比新请求晚，丢弃
    if (currentVersion !== renderVersion) {
      return
    }

    svgHtml.value = svg
  } catch (error) {
    // 旧请求的错误同样忽略
    if (currentVersion !== renderVersion) {
      return
    }

    console.error('Mermaid 渲染发生错误:', error)

    hasError.value = true
    svgHtml.value = ''

    nextTick(() => {
      const badElement = document.getElementById(`d${chartId}`)

      if (badElement) {
        badElement.remove()
      }
    })
  }
}

watch(
  () => props.info,
  () => {
    // 清除上一次 debounce
    if (renderTimer) {
      clearTimeout(renderTimer)
    }

    // 用户停止输入 300ms 后再渲染
    renderTimer = setTimeout(() => {
      drawDiagram()
    }, 300)
  },
  {
    immediate: true,
  },
)

onBeforeUnmount(() => {
  if (renderTimer) {
    clearTimeout(renderTimer)
  }

  // 让正在进行的旧 render 失效
  renderVersion++
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
