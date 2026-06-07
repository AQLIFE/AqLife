<template>
  <ElCol>
    <ElCol>header</ElCol>
    <div id="mdRender">
      <template v-for="(node, index) in renderNodes" :key="index">
        <!-- 渲染代码块 -->
        <CodeBlock
          v-if="node.type === 'component' && node.component === 'CodeBlock'"
          :info="node.content"
          :infoType="node.info"
        />

        <!-- 渲染 Mermaid -->
        <MerimaidPreview
          v-else-if="node.type === 'component' && node.component === 'MerimaidPreview'"
          :info="node.content"
          :info-type="node.info"
        />
        <TipPreview v-else-if="node.type == 'blockquote'" :quoteTokens="node.tokens" />
        <!-- 渲染表格：由于 TablePreview 拿到了属于自己的完整 tokens，不会被外部 div 拆散 -->
        <TablePreview v-else-if="node.type === 'table'" :tableTokens="node.tokens" />

        <!-- 渲染普通 HTML：连续的普通标签现在被正确包裹，且不会截断 table 内部结构 -->
        <div v-else-if="node.type === 'html'" v-html="node.content"></div>
      </template>
    </div>
  </ElCol>
</template>

<script lang="ts" setup>
import { ElCol } from 'element-plus'
import { useRoute } from 'vue-router'
import { Configuration, FileApi, type ApiFileDownloadGetRequest } from '@/api/generated'
import { ApiOption } from '@/services/storage/BaseOptions'
import { ref, onBeforeMount, computed } from 'vue'
import { mdRenderOption } from '@/data/MdRenderOption'
import CodeBlock from '@/components/CodeBlock.vue'
import MerimaidPreview from '@/components/MerimaidPreview.vue'
import TablePreview from '@/components/TablePreview.vue'
import TipPreview from '@/components/TipPreview.vue'
import type Token from 'markdown-it\\lib\\token.d.mts'

const route = useRoute()
const fileApi = new FileApi(new Configuration(ApiOption))

const sourceMarkdown = ref<string>('')

// const tableToken: Ref<Token[]> = ref([])
// const codeToken: Ref<Token[]> = ref([])

const tokens = computed(() => mdRenderOption.parse(sourceMarkdown.value, {}))

const renderTokens = (token: Token) =>
  mdRenderOption.renderer.render([token], mdRenderOption.options, {})

const renderNodes = computed(() => {
  const nodes: any[] = [] // 抽象节点集合
  const allTokens = tokens.value // 经过解析后的AST树

  for (let i = 0; i < allTokens.length; i++) {
    const token = allTokens[i]
    console.log(token)

    // 1. 处理代码块或 Mermaid [cite: 5]
    if (token.type === 'fence') {
      nodes.push({
        type: 'component',
        component: token.info === 'mermaid' ? 'MerimaidPreview' : 'CodeBlock',
        content: token.content,
        info: token.info,
      })
    } else if (token.type === 'blockquote_open') {
      const quoteGroup = []
      let j = i
      // 寻找对应的闭合标签 blockquote_close
      while (j < allTokens.length && allTokens[j].type !== 'blockquote_close') {
        quoteGroup.push(allTokens[j])
        j++
      }
      quoteGroup.push(allTokens[j]) // 压入 blockquote_close
      nodes.push({ type: 'blockquote', tokens: quoteGroup })
      i = j // 跳过已聚合的 Token
    }
    // 2. 识别表格：从 table_open 抓到 table_close
    else if (token.type === 'table_open') {
      const tableGroup = []

      // 这里的逻辑就是你之前写的，但在 computed 里做可以支持多个表格 [cite: 5]
      while (i < allTokens.length - 1 && allTokens[i].type !== 'table_close') {
        tableGroup.push(allTokens[i++])
      }
      tableGroup.push(allTokens[i]) // 放入 table_close
      nodes.push({ type: 'table', tokens: tableGroup })
    }
    // 3. 处理普通 HTML 内容：将其渲染为字符串块
    else {
      // 这里的 renderToken 逻辑保持不变 [cite: 5]
      const html = renderTokens(token)
      nodes.push({ type: 'html', content: html })
    }
  }
  return nodes
})

/**
 * 下载文件，若不存在则不解析
 * @param params
 */
async function downloadAndRenderMarkdown(params: ApiFileDownloadGetRequest) {
  try {
    const responseWrapper = await fileApi.apiFileDownloadGetRaw(params)
    const response = responseWrapper.raw

    if (response.status === 204) return ''
    if (!response.ok) {
      console.error(await response.text())
      return ''
    }

    // 只需要拿到纯文本赋值即可，VueMarkdown 会自动监听并响应渲染
    return await response.text()
  } catch (error) {
    console.error('网络请求发生严重异常:', error)
    return ''
  }
}

onBeforeMount(async () => {
  sourceMarkdown.value = await downloadAndRenderMarkdown({ id: route.params.id as string })
})
</script>

<style lang="css" scoped>
#mdRender {
  width: 100%;
  height: calc(100vh - 100px);

  overflow-y: auto;
}

#mdRender::-webkit-scrollbar {
  display: none;
}
</style>
