<template>
  <ElCol>
    <ElCol>header</ElCol>
    <div id="mdRender">
      <template v-for="(node, index) in renderNodes" :key="index">
        <CodeBlock
          v-if="node.type === 'component' && node.component === 'CodeBlock'"
          :info="node.content"
          :infoType="node.info"
        />

        <MermaidPreview
          v-else-if="node.type === 'component' && node.component === 'MermaidPreview'"
          :info="node.content"
          :info-type="node.info"
        />
        <TipPreview v-else-if="node.type == 'blockquote'" :quoteTokens="node.tokens" />
        <TablePreview v-else-if="node.type === 'table'" :tableTokens="node.tokens" />

        <div v-else-if="node.type === 'html'" v-html="node.content"></div>
      </template>
    </div>
  </ElCol>
</template>

<script lang="ts" setup>
import { ElCol } from 'element-plus'
import { useRoute } from 'vue-router'
import { FileApi, type ApiFileDownloadGetRequest } from '@/api'
import { apiConfiguration } from '@/services/api'
import { ref, onBeforeMount, computed } from 'vue'
import { mdRenderOption } from '@/data/mdRenderOption'
import CodeBlock from '@/components/CodeBlock.vue'
import MermaidPreview from '@/components/MermaidPreview.vue'
import TablePreview from '@/components/TablePreview.vue'
import TipPreview from '@/components/TipPreview.vue'
import type Token from 'markdown-it/lib/token.mjs'

const route = useRoute()
const fileApi = new FileApi(apiConfiguration)

const sourceMarkdown = ref<string>('')

const tokens = computed(() => mdRenderOption.parse(sourceMarkdown.value, {}))

const renderTokens = (token: Token) =>
  mdRenderOption.renderer.render([token], mdRenderOption.options, {})

const renderNodes = computed(() => {
  const nodes: any[] = []
  const allTokens = tokens.value

  for (let i = 0; i < allTokens.length; i++) {
    const token = allTokens[i]

    if (token.type === 'fence') {
      nodes.push({
        type: 'component',
        component: token.info === 'mermaid' ? 'MermaidPreview' : 'CodeBlock',
        content: token.content,
        info: token.info,
      })
    } else if (token.type === 'blockquote_open') {
      const quoteGroup = []
      let j = i
      while (j < allTokens.length && allTokens[j].type !== 'blockquote_close') {
        quoteGroup.push(allTokens[j])
        j++
      }
      quoteGroup.push(allTokens[j])
      nodes.push({ type: 'blockquote', tokens: quoteGroup })
      i = j
    } else if (token.type === 'table_open') {
      const tableGroup = []
      while (i < allTokens.length - 1 && allTokens[i].type !== 'table_close') {
        tableGroup.push(allTokens[i++])
      }
      tableGroup.push(allTokens[i])
      nodes.push({ type: 'table', tokens: tableGroup })
    } else {
      const html = renderTokens(token)
      nodes.push({ type: 'html', content: html })
    }
  }
  return nodes
})

async function downloadAndRenderMarkdown(params: ApiFileDownloadGetRequest) {
  try {
    const responseWrapper = await fileApi.apiFilePreviewGetRaw(params)
    const response = responseWrapper.raw

    if (response.status === 204) return ''
    if (!response.ok) {
      console.error(await response.text())
      return ''
    }

    return await response.text()
  } catch (error) {
    console.error('网络请求发生严重异常:', error)
    return ''
  }
}

onBeforeMount(async () => {
  sourceMarkdown.value = await downloadAndRenderMarkdown({ uID: route.params.id as string })
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
