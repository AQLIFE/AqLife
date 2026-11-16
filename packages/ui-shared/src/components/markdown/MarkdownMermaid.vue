<template>
  <div class="code">
    <div class="codeHeader">
      <div class="codeType">{{ props.infoType || 'text' }}</div>
      <div class="codeTitle"></div>
      <div class="viewerActions">
        <ElButton
          class="viewerButton"
          title="全屏预览"
          :icon="FullScreen"
          @click="openFullscreen"
        />
      </div>
    </div>

    <div class="codeContent">
      <div
        class="mermaid-viewport"
      >
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
      </div>
    </div>

    <ElDialog
      v-model="isFullscreen"
      fullscreen
      destroy-on-close
      title="图表详情预览"
      class="mermaid-fullscreen-dialog"
    >
      <div class="fullscreen-viewer">
        <div class="fullscreen-toolbar">
          <ElButton
            title="缩小"
            :icon="ZoomOut"
            :disabled="scale <= MIN_SCALE"
            @click="zoomOut"
          />
          <span class="fullscreen-zoom">{{ Math.round(scale * 100) }}%</span>
          <ElButton
            title="放大"
            :icon="ZoomIn"
            :disabled="scale >= MAX_SCALE"
            @click="zoomIn"
          />
          <ElButton
            title="重置视图"
            :icon="Refresh"
            @click="resetView"
          />
        </div>

        <div
          class="mermaid-viewport fullscreen-viewport"
          @wheel.prevent="handleWheel"
          @pointerdown="startPan"
          @pointermove="movePan"
          @pointerup="endPan"
          @pointercancel="endPan"
          @dblclick="resetView"
        >
          <div
            v-if="svgHtml"
            class="mermaid-svg-wrapper fullscreen-svg-wrapper"
            :style="viewportStyle"
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
        </div>
      </div>
    </ElDialog>
  </div>
</template>

<script setup lang="ts">
import {
  FullScreen,
  Refresh,
  ZoomIn,
  ZoomOut,
} from '@element-plus/icons-vue'
import { ElButton, ElDialog } from 'element-plus'
import { computed, onBeforeUnmount, ref, watch } from 'vue'
import { renderMermaid } from './mermaidRenderer'

const props = defineProps<{
  info: string
  infoType: string
}>()

const MIN_SCALE = 0.2
const MAX_SCALE = 5
const ZOOM_STEP = 0.2

const isFullscreen = ref(false)
const svgHtml = ref('')
const hasError = ref(false)
const scale = ref(1)
const offset = ref({ x: 0, y: 0 })

let renderTimer: ReturnType<typeof setTimeout> | undefined
let renderVersion = 0
let panStart: {
  pointerId: number
  x: number
  y: number
  offsetX: number
  offsetY: number
} | null = null

const viewportStyle = computed(() => ({
  transform: `translate3d(${offset.value.x}px, ${offset.value.y}px, 0) scale(${scale.value})`,
}))

const clampScale = (value: number) =>
  Math.min(MAX_SCALE, Math.max(MIN_SCALE, value))

const setScale = (value: number) => {
  scale.value = clampScale(value)
}

const zoomIn = () => setScale(scale.value + ZOOM_STEP)
const zoomOut = () => setScale(scale.value - ZOOM_STEP)

const resetView = () => {
  scale.value = 1
  offset.value = { x: 0, y: 0 }
  panStart = null
}

const openFullscreen = () => {
  isFullscreen.value = true
}

const handleWheel = (event: WheelEvent) => {
  const direction = event.deltaY < 0 ? 1 : -1
  setScale(scale.value + direction * ZOOM_STEP)
}

const startPan = (event: PointerEvent) => {
  if (event.button !== 0 || !svgHtml.value) {
    return
  }

  const target = event.currentTarget
  if (!(target instanceof HTMLElement)) {
    return
  }

  target.setPointerCapture(event.pointerId)
  panStart = {
    pointerId: event.pointerId,
    x: event.clientX,
    y: event.clientY,
    offsetX: offset.value.x,
    offsetY: offset.value.y,
  }
}

const movePan = (event: PointerEvent) => {
  if (!panStart || panStart.pointerId !== event.pointerId) {
    return
  }

  offset.value = {
    x: panStart.offsetX + event.clientX - panStart.x,
    y: panStart.offsetY + event.clientY - panStart.y,
  }
}

const endPan = (event: PointerEvent) => {
  if (panStart?.pointerId !== event.pointerId) {
    return
  }

  const target = event.currentTarget
  if (target instanceof HTMLElement && target.hasPointerCapture(event.pointerId)) {
    target.releasePointerCapture(event.pointerId)
  }

  panStart = null
}

const drawDiagram = async () => {
  const codeText = props.info.trim()

  if (!codeText) {
    svgHtml.value = ''
    hasError.value = false
    resetView()
    return
  }

  const currentVersion = ++renderVersion

  svgHtml.value = ''
  hasError.value = false
  resetView()

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
  align-items: center;
  min-height: 40px;
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

.viewerActions {
  display: flex;
  align-items: center;
  gap: 2px;
  padding-right: 6px;
}

.viewerButton {
  padding: 0 8px;
  cursor: pointer;
  transition: color 0.2s;
  background-color: transparent;
  border: 0;
  height: 32px;
}

.viewerButton:hover {
  color: #fff;
}

.viewerButton:disabled {
  opacity: 0.4;
}

.zoomPercentage {
  min-width: 42px;
  color: #fff;
  text-align: center;
  font-size: 12px;
  user-select: none;
}

.codeContent {
  background-color: var(--topColor);
  margin: 0;
}

/* 视口高度独立于 SVG 的实际尺寸，避免图表撑开 Markdown 页面。 */
.mermaid-viewport {
  position: relative;
  width: 100%;
  height: 360px;
  overflow: hidden;
  display: flex;
  align-items: center;
  justify-content: center;
  box-sizing: border-box;
  background: var(--topColor);
}

.fullscreen-viewport {
  touch-action: none;
  cursor: grab;
}

.fullscreen-viewport:active {
  cursor: grabbing;
}

.mermaid-svg-wrapper {
  width: 100%;
  height: 100%;
  flex: 0 0 100%;
  display: flex;
  align-items: center;
  justify-content: center;
}

.fullscreen-svg-wrapper {
  transform-origin: center center;
  will-change: transform;
}

.mermaid-svg-wrapper :deep(svg) {
  display: block;
  width: 100% !important;
  height: 100% !important;
  max-width: none !important;
  max-height: none !important;
}

.mermaid-error-fallback {
  min-height: 180px;
  width: 100%;
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
  width: 100%;
  display: flex;
  align-items: center;
  justify-content: center;
  color: var(--back_color_lv5);
}

.fullscreen-viewer {
  display: flex;
  flex-direction: column;
  height: calc(100vh - 100px);
  min-height: 0;
}

.fullscreen-toolbar {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 8px;
  padding: 8px;
}

.fullscreen-zoom {
  min-width: 48px;
  text-align: center;
  font-size: 13px;
}

.fullscreen-viewport {
  height: auto;
  flex: 1;
  min-height: 0;
  background: var(--el-bg-color);
  border: 1px solid var(--el-border-color-lighter);
  border-radius: 6px;
}

@media (max-width: 768px) {
  .mermaid-viewport {
    height: 300px;
  }

  .viewerActions {
    gap: 0;
  }

  .viewerButton {
    padding: 0 5px;
  }
}
</style>
