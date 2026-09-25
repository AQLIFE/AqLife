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
        <div
          v-if="svgHtml"
          class="mermaid-svg-wrapper"
          v-html="svgHtml"
        />

        <div v-else-if="hasError" class="mermaid-error-fallback">
          <div class="error-header">
            图表采用落后样式编写,当前不支持渲染
          </div>
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
          <div
            v-if="svgHtml"
            class="fullscreen-content"
            v-html="svgHtml"
          />
          <div v-else-if="hasError" class="mermaid-error-fallback">
            <div class="error-header">
              图表采用落后样式编写,当前不支持渲染
            </div>
          </div>
        </ElDialog>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { FullScreen } from '@element-plus/icons-vue'
import { renderMermaid } from './mermaidRenderer'
import { ElButton, ElDialog } from 'element-plus'
import {
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

const drawDiagram = async () => {
  const codeText = props.info.trim()

  if (!codeText) {
    svgHtml.value = ''
    hasError.value = false
    return
  }

  const currentVersion = ++renderVersion

  svgHtml.value = ''
  hasError.value = false

  try {
    // 直接以 Mermaid 的 render API 作为唯一判定入口。
    // 不额外调用 parse，避免把 Mermaid 能实际渲染的定义提前判为失败。
    const svg = await renderMermaid(codeText)

    // 旧请求完成得比新请求晚，丢弃。
    if (currentVersion !== renderVersion) {
      return
    }

    if (!svg?.trim()) {
      throw new Error('Mermaid render returned an empty SVG')
    }

    svgHtml.value = svg
  } catch (error) {
    // 旧请求的错误同样忽略。
    if (currentVersion !== renderVersion) {
      return
    }

    console.error('Mermaid 渲染发生错误:', error)

    svgHtml.value = ''
    hasError.value = true
  }
}

watch(
  () => props.info,
  () => {
    if (renderTimer) {
      clearTimeout(renderTimer)
    }

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

  // 让正在进行的旧 render 失效。
  renderVersion++
})
</script>

<style scoped>
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
  background-color: var(--back_color_lv5);
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
  border: 0;
  height: initial;
}

.codeHeader .copyButton:hover {
  color: #fff;
}

.codeContent {
  background-color: var(--topColor);
  margin: 0;
}

.mermaid-viewer-container {
  width: 100%;
  min-width: 0;
  overflow-x: auto;
  overflow-y: hidden;
}

.mermaid-svg-wrapper {
  width: max-content;
  min-width: 100%;
}

.mermaid-svg-wrapper :deep(svg) {
  display: block;
  width: auto !important;
  height: auto !important;
  max-width: none !important;
  margin: 0 auto;
}

.mermaid-error-fallback {
  min-height: 180px;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 24px;
  box-sizing: border-box;
}

.error-header {
  padding: 16px 24px;
  border: 1px solid var(--back_color_lv5);
  background-color: var(--back_color_lv2);
  color: var(--back_color_lv5);
  text-align: center;
  font-size: 0.95rem;
}

.mermaid-loading {
  min-height: 180px;
  display: flex;
  align-items: center;
  justify-content: center;
  color: var(--back_color_lv5);
}

.fullscreen-content {
  width: 100%;
  height: 100%;
  overflow: auto;
}

.fullscreen-content :deep(svg) {
  display: block;
  width: auto !important;
  height: auto !important;
  max-width: none !important;
}
</style>
