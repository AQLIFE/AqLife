<template>
  <div id="mdRender">
    <template v-for="(node, index) in rNode" :key="index">
      <CodeBlock v-if="node.type === 'component' && node.component === 'CodeBlock'" :info="node.content"
        :info-type="node.info" />

      <MarkdownMermaid v-else-if="
        node.type === 'component' &&
        node.component === 'MermaidPreview'
      " :info="node.content" :info-type="node.info" />

      <div
        v-else-if="node.type === 'blockquote'"
        class="markdown-blockquote"
        v-html="renderTokenGroup(node.tokens)"
      />

      <MarkdownTable v-else-if="node.type === 'table'" :table-tokens="node.tokens" />

      <MarkdownImage
        v-else-if="node.type === 'resolved-link' && node.result.type === 'image'"
        :src="node.result.src"
        :alt="node.result.alt"
      />

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
  isImageType,
  mdRenderOption,
  type MarkdownLinkContext,
  type MarkdownLinkResult,
} from '@aqlife/domain'
import type { MarkdownFileResolver } from '../../markdown/type.ts'
import CodeBlock from './MarkdownCode.vue'
import MarkdownMermaid from './MarkdownMermaid.vue'
import MarkdownTable from './MarkdownTable.vue'
import MarkdownImage from './MarkdownImage.vue'

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


async function resolveInternalLink(
  context: MarkdownLinkContext,
): Promise<MarkdownLinkResult | null> {
  const { href, text } = context

  let uid: string | undefined

  // Markdown image links may reference a file directly by UID.
  // Keep absolute backend preview URLs compatible with existing content.
  // Only resolve direct UID references. Legacy preview URLs remain ordinary links.
  if (!/^[0-9a-f]{8}-[0-9a-f]{4}-[1-8][0-9a-f]{3}-[89ab][0-9a-f]{3}-[0-9a-f]{12}$/i.test(href.trim())) {
    return null
  }
  uid = href.trim()

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

.markdown-blockquote {
  margin: 1rem 0;
  padding: 0.25rem 1rem;
  border-left: 4px solid var(--el-border-color);
  color: var(--el-text-color-secondary);
  background: var(--el-fill-color-lighter);
}

.markdown-blockquote :deep(p) {
  margin: 0.5rem 0;
}
</style>
