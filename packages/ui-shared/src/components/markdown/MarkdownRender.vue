<template>
  <div id="mdRender">
    <template v-for="(node, index) in rNode" :key="index">
      <CodeBlock v-if="node.type === 'component' && node.component === 'CodeBlock'" :info="node.content"
        :info-type="node.info" />

      <MarkdownMermaid v-else-if="
        node.type === 'component' &&
        node.component === 'MermaidPreview'
      " :info="node.content" :info-type="node.info" />

      <MarkdownTable v-else-if="node.type === 'table'" :table-tokens="node.tokens" />

      <img v-else-if="
        node.type === 'resolved-link' &&
        node.result.type === 'image'
      " :src="node.result.src" :alt="node.result.alt" />

      <RouterLink v-else-if="
        node.type === 'resolved-link' &&
        node.result.type === 'markdown'
      " :to="node.result.href" class="markdown-internal-link">
        {{ node.text }}
      </RouterLink>

      <a v-else-if="
        node.type === 'resolved-link' &&
        node.result.type === 'link'
      " :href="node.result.href">
        {{ node.text }}
      </a>

      <div v-else-if="node.type === 'html'" v-html="node.content" />
    </template>
  </div>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue'
import {
  buildMarkdownRenderNodes,
  extractFileUid,
  isImageType,
  mdRenderOption,
  type MarkdownLinkContext,
  type MarkdownLinkResult,
} from '@aqlife/domain'
import type { MarkdownFileResolver } from '../../markdown/type.ts'
import CodeBlock from './MarkdownCode.vue'
import MarkdownMermaid from './MarkdownMermaid.vue'
import MarkdownTable from './MarkdownTable.vue'

interface MarkdownRenderProps {
  markdown: string
  baseurl?: string
  fileResolver?: MarkdownFileResolver
}

const props = defineProps<MarkdownRenderProps>()

const rNode = ref<any[]>([])
const loading = ref(false)

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
  loading.value = true

  try {
    const parsed = mdRenderOption.parse(props.markdown, {})

    rNode.value = await buildMarkdownRenderNodes(
      parsed,
      props.fileResolver
        ? resolveInternalLink
        : undefined,
    )
  } finally {
    loading.value = false
  }
}

function renderTokenGroup(tokens: any[]): string {
  return mdRenderOption.renderer.render(
    tokens,
    mdRenderOption.options,
    {},
  )
}

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
  const { href, text } = context

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

  if (!uid || !props.fileResolver) {
    return null
  }

  const file = await props.fileResolver.getFile(uid)

  if (!file) {
    return null
  }

  if (isImageType(file.fileType ?? '')) {
    return {
      type: 'image',
      src: buildPreviewUrl(uid),
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

function buildPreviewUrl(uid: string): string {
  if (!props.baseurl) {
    return ''
  }

  const base = new URL(props.baseurl)

  base.pathname =
    `${base.pathname.replace(/\/$/, '')}/api/File/preview`

  base.search = ''
  base.searchParams.set('UID', uid)

  return base.toString()
}

defineExpose({
  loading,
})
</script>

<style scoped>
#mdRender {
  padding: 0 20px;
}
</style>
