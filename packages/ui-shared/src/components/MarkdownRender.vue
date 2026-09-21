<template>
    <div id="mdRender">
        <template v-for="(node, index) in rNode" :key="index">
            <CodeBlock v-if="node.type === 'component' && node.component === 'CodeBlock'" :info="node.content"
                :infoType="node.info" />
            <MermaidPreview v-else-if="node.type === 'component' && node.component === 'MermaidPreview'"
                :info="node.content" :info-type="node.info" />
            <TipPreview v-else-if="node.type === 'blockquote'" :quoteTokens="node.tokens" />
            <TablePreview v-else-if="node.type === 'table'" :tableTokens="node.tokens" />
            <InternalImage v-else-if="
                node.type === 'resolved-link' &&
                node.result.type === 'image'
            " :src="node.result.src" :alt="node.result.alt"/>

            <RouterLink v-else-if="
                node.type === 'resolved-link' &&
                node.result.type === 'markdown'
            " :to="node.result.href" class="markdown-internal-link">
                {{ node.text }}
            </RouterLink>
            <a
                v-else-if="
                    node.type === 'resolved-link' &&
                    node.result.type === 'link'
                "
                :href="node.result.href"
            >
                {{ node.text }}
            </a>

            <div v-else-if="node.type === 'html'" v-html="node.content" />
        </template>
    </div>
</template>

<script setup lang="ts">
import { computed, onBeforeMount, ref, watch } from 'vue';
import CodeBlock from './CodeBlock.vue'
import TipPreview from './TipPreview.vue'
import TablePreview from './TablePreview.vue'
import MermaidPreview from './MermaidPreview.vue';
import { mdRenderOption, buildMarkdownRenderNodes, type MarkdownLinkResult, isImageType, extractFileUid, type MarkdownLinkContext } from '@aqlife/domain'
import { FileApi } from '@/api';
import InternalImage from './InternalImage.vue'
import { apiConfiguration } from '@/services/api.ts';


interface MarkdownRenderProps {
    markdown: string
    baseurl?: string
}
const props = defineProps<MarkdownRenderProps>()

const rNode = ref<any[]>([])

watch(
    () => props.markdown,
    () => {
        renderMarkdown()
    },
    {
        immediate: true,
    },
)
async function renderMarkdown() {
    const parsed = mdRenderOption.parse(
        props.markdown,
        {},
    )

    rNode.value =
        await buildMarkdownRenderNodes(
            parsed,
            resolveInternalLink,
        )
}

const fileApi = new FileApi(apiConfiguration)

function isSameOrigin(target: URL): boolean {
  if (!props.baseurl) {
    return false
  }

  try {
    const base = new URL(props.baseurl)

    return target.origin === base.origin
  } catch {
    return false
  }
}

async function resolveInternalLink(
  context: MarkdownLinkContext,
): Promise<MarkdownLinkResult | null> {
  const {
    href,
    text,
  } = context

  let url: URL
  

  try {
    url = new URL(href)
  } catch {
    return null
  }
  if (!isSameOrigin(url)) {
    return null
  }

  const uid = extractFileUid(url)

  if (!uid) {
    console.error('没找到ID')
    return null
  }
  console.log("File=>",uid)
  const files = await fileApi.apiFileGet({
    uID: uid,
  })

  const file = files[0]

  if (!file) {
    return null
  }

  if (isImageType(file.fileType ?? '')) {
    const str =buildPreviewUrl(uid)
    console.log("str",str)
    return {
      type: 'image',
      src: str,
      alt: text,
    }
  }

  if (file.fileType?.toLowerCase() === '.md') {
    return {
      type: 'markdown',
      href: `/preview/${uid}`,
    }
  }

  return {
    type: 'link',
    href,
}
}

function buildPreviewUrl(
    uid: string,
): string {
    if (!props.baseurl) {
        return ''
    }

    const base = new URL(props.baseurl)

    base.pathname =
        `${base.pathname.replace(/\/$/, '')}/api/File/preview`

    base.search = ''

    base.searchParams.set(
        'UID',
        uid,
    )

    return base.toString()
}
</script>

<style scoped>
#mdRender {
  padding: 0 20px;
}
</style>