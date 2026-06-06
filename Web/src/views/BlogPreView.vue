<template>
  <ElCol>
    <ElCol>header</ElCol>
    <div id="mdRender">
      <!-- 遍历解析后的结构块，精准投喂组件 -->
      <template v-for="(block, index) in parsedBlocks" :key="index">
        <!-- 如果是代码块类型，直接渲染你的 CodeBlock 组件 -->
        <CodeBlock v-if="block.type === 'code'" :CodeType="block.lang">
          <!-- 这里包裹一层 pre/code，这样你组件里的 textContent 才能抓到带格式的纯代码 -->
          <pre><code>{{ block.content }}</code></pre>
        </CodeBlock>

        <!-- 如果是普通富文本，直接渲染原本的 HTML -->
        <div v-else v-html="block.content" />
      </template>
    </div>
  </ElCol>
</template>

<script lang="ts" setup>
import { ElCol } from 'element-plus'
import { useRoute } from 'vue-router'
import { Configuration, FileApi, type ApiFileDownloadGetRequest } from '@/api/generated'
import { ApiOption } from '@/services/storage/BaseOptions'
import { ref, type Ref, onMounted, h } from 'vue'

// 1. 引入 vue-markdown-render
import VueMarkdown from 'vue-markdown-render'
// 2. 引入你的自定义代码块组件
import CodeBlock from '@/components/CodeBlock.vue'

const route = useRoute()
const fileApi = new FileApi(new Configuration(ApiOption))
const markdownContent: Ref<string> = ref('')

// markdown-it 的插件数组（如果需要高亮，可以把 markdown-it-highlightjs 放这里）
const plugins = ref([])

// ⭐ 核心：定义标签到组件的映射
const renderComponents = {
  // 当解析到 markdown 的 pre 标签（即代码块外层）时，交由我们的自定义组件渲染
  pre: (props: any, { slots }: any) => {
    // 提取 markdown-it 传下来的代码语言类型 (例如 language-javascript)
    const codeNode = slots.default?.()[0]
    const className = codeNode?.props?.className || ''
    const lang = className.replace('language-', '') || 'text'

    // 使用 Vue 的 h 函数，渲染 CodeBlock 组件，并将原本的 <code> 节点作为插槽传给它
    return h(
      CodeBlock,
      { CodeType: lang },
      {
        default: () => slots.default(),
      },
    )
  },
}

onMounted(() => {
  downloadAndRenderMarkdown({ id: route.params.id as string })
})

async function downloadAndRenderMarkdown(params: ApiFileDownloadGetRequest) {
  try {
    const responseWrapper = await fileApi.apiFileDownloadGetRaw(params)
    const response = responseWrapper.raw

    if (response.status === 204) return
    if (!response.ok) {
      console.error(await response.text())
      return
    }

    // 只需要拿到纯文本赋值即可，VueMarkdown 会自动监听并响应渲染
    markdownContent.value = await response.text()
  } catch (error) {
    console.error('网络请求发生严重异常:', error)
  }
}
</script>

<style lang="css" scoped>
#mdRender {
  width: 100%;
  height: 100vh;
  overflow-y: auto;
}
</style>
