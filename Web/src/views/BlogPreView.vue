<template>
  <ElCol>
    <ElCol>header</ElCol>
    <div id="mdRender">
      <template v-for="(token, index) in tokens" :key="index">
        <CodeBlock
          v-if="token.type == 'fence' && token.info != 'mermaid'"
          :info="token.content"
          :infoType="token.info"
        />
        <MerimaidPreview
          v-else-if="token.info == 'mermaid'"
          :info="token.content"
          :info-type="token.info"
        />
        <div v-else-if="token.type != 'fence_close'" v-html="renderToken(token)"></div>
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

const route = useRoute()
const fileApi = new FileApi(new Configuration(ApiOption))
const sourceMarkdown = ref<string>('')
const tokens = computed(() => {
  const temp = mdRenderOption.parse(sourceMarkdown.value, {})
  // console.log(temp)
  return temp
})
const renderToken = (token: any) =>
  mdRenderOption.renderer.render([token], mdRenderOption.options, {})

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
  // if (sourceMarkdown.value != null && sourceMarkdown.value != '')
  //   renderedMarkdown.value = mdRenderOption.render(sourceMarkdown.value)
})

// onMounted(() => test())
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
