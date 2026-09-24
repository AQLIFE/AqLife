<template>
  <div class="code-block">
    <header class="code-block__header">
      <span class="code-block__language">
        {{ language }}
      </span>

      <span class="code-block__title">
        {{ title }}
      </span>

      <div class="code-block__actions">
        <button
          class="code-block__copy"
          type="button"
          title="复制代码"
          @click="onCopy"
        >
          {{ copied ? 'Copied' : 'Copy' }}
        </button>
      </div>
    </header>

    <div class="code-block__body">
      <pre><code
        class="hljs"
        v-html="highlightedHTML"
      /></pre>
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
  infoType?: string
  title?: string
}>()

const language = computed(() =>
  props.infoType?.trim() || 'text'
)

const copied = ref(false)

// 2. 核心：利用 highlight.js 将纯文本转换为带颜色标签的 HTML
const highlightedHTML = computed(() => {
  const lang = props.infoType
  const code = (props.info || '').replace(/\r?\n$/, '')

  if (lang && hljs.getLanguage(lang)) {
    try {
      return hljs.highlight(code, { language: lang }).value
    } catch {}
  }

  return hljs.highlightAuto(code).value
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
.code-block {
  margin: 1rem 0;
  overflow: hidden;
  border-radius: 8px;
}

.code-block__header {
  display: flex;
  align-items: center;

  min-height: 40px;

  background-color: #2d2d2d;
  color: #ccc;

  font-size: 0.9rem;
}

.code-block__language {
  padding: 0 1rem;

  font-weight: 500;
  text-transform: uppercase;
}

.code-block__title {
  flex: 1;

  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.code-block__actions {
  display: flex;
  align-items: center;
}

.code-block__copy {
  padding: 0 1rem;

  border: 0;
  background: transparent;
  color: #ccc;

  cursor: pointer;
}

.code-block__copy:hover {
  color: #fff;
}

.code-block__body {
  background-color: #282c34;
}

.code-block__body pre {
  margin: 0;
  padding: 0;

  overflow-x: auto;
}

.code-block__body :deep(code.hljs) {
  display: block;

  box-sizing: border-box;
  padding: 1rem;

  background: transparent;
}
</style>
