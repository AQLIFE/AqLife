<template>
  <ElCol class="preview">
    <ElPageHeader :icon="ArrowLeft" @back="$router.push('/blog')" class="header">
      <template #content>
        <span>{{articleStore.blogTitle}}</span>
        <template v-if="fileMeta[0]">
          <ElTag style="margin: 0px 20px;" v-for="item,index in fileMeta[0].tags" :key="index">{{ item.name }}</ElTag>
        </template>
      </template>
      <template #extra>
        <ElButton :icon="Share" link/>
      </template>
    </ElPageHeader>

    <div id="mdRender">
      <template v-for="(node, index) in rNode" :key="index">
        <CodeBlock v-if="node.type === 'component' && node.component === 'CodeBlock'" :info="node.content"
          :infoType="node.info" />

        <MermaidPreview v-else-if="node.type === 'component' && node.component === 'MermaidPreview'"
          :info="node.content" :info-type="node.info" />
        <TipPreview v-else-if="node.type == 'blockquote'" :quoteTokens="node.tokens" />
        <TablePreview v-else-if="node.type === 'table'" :tableTokens="node.tokens" />

        <div v-else-if="node.type === 'html'" v-html="node.content"></div>
      </template>
    </div>
  </ElCol>
</template>

<script lang="ts" setup>
import { ElCol, ElPageHeader,ElButton, ElTag } from 'element-plus'
import { useRoute, useRouter } from 'vue-router'
import { FileApi, type ApiFileDownloadGetRequest, type FileDto } from '@/api'
import { apiConfiguration } from '@/services/api'
import { ref, computed, onBeforeMount,watch } from 'vue'
import { mdRenderOption, renderNodes } from '@aqlife/domain'
import CodeBlock from '@/components/CodeBlock.vue'
import MermaidPreview from '@/components/MermaidPreview.vue'
import TablePreview from '@/components/TablePreview.vue'
import TipPreview from '@/components/TipPreview.vue'
import { useArticleStore } from '@/stores/articleStore'
import { extractToc } from '@/services/markdownParser'
import { ArrowLeft, Share } from '@element-plus/icons-vue'
const articleStore = useArticleStore()


const route = useRoute()
useRouter();
const fileApi = new FileApi(apiConfiguration)

const sourceMarkdown = ref<string>('')

watch(sourceMarkdown, (value: string) => {
  articleStore.markdown = value
  articleStore.toc = extractToc(tokens.value)
})
const tokens = computed(() => mdRenderOption.parse(sourceMarkdown.value, {}))

const rNode = computed(()=>renderNodes(tokens.value))

async function getPreview(params: ApiFileDownloadGetRequest) {
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
const fileMeta = ref<FileDto[]>([])
onBeforeMount(async () => {
  sourceMarkdown.value = await getPreview({ uID: route.params.id as string })
  fileMeta.value = await fileApi.apiFileGet({ uID: route.params.id as string })
  if (fileMeta.value[0] != undefined) articleStore.blogTitle = fileMeta.value[0].fileName!
})
watch(()=>route.params.id,
async()=>{
  sourceMarkdown.value = await getPreview({ uID: route.params.id as string })
  fileMeta.value = await fileApi.apiFileGet({ uID: route.params.id as string })
  if (fileMeta.value[0] != undefined) articleStore.blogTitle = fileMeta.value[0].fileName!
})
</script>

<style lang="css" scoped>
.preview{
  display: grid;
  height: 100vh;
  grid-template-rows: auto 1fr;
}
.header{
  height:50px;
  line-height: 50px;
}
#mdRender {
  grid-column: 1/2;
  overflow-x:scroll;
}

#mdRender::-webkit-scrollbar {
  display: none;
}
</style>
