<script setup lang="ts">
import type { TocNode } from '@aqlife/domain'
import { computed, ref, watch } from 'vue'
import { useArticleStore } from '@/stores/articleStore'
import TocTreeItem from './TocTreeItem.vue'

const props = defineProps<{
  items: TocNode[]
  title?: string
}>()

const articleStore = useArticleStore()

const expandedAnchor = ref<string | null>(null)

function containsAnchor(
  node: TocNode,
  anchor: string,
): boolean {
  if (node.anchor === anchor) {
    return true
  }

  return node.children.some(child =>
    containsAnchor(child, anchor),
  )
}

const activeRoot = computed(() => {
  const activeAnchor = articleStore.activeAnchor

  if (!activeAnchor) {
    return null
  }

  return (
    props.items.find(item =>
      containsAnchor(item, activeAnchor),
    ) ?? null
  )
})

watch(
  activeRoot,
  root => {
    if (root) {
      expandedAnchor.value = root.anchor
    }
  },
  {
    immediate: true,
  },
)

function toggleRoot(node: TocNode) {
  if (expandedAnchor.value === node.anchor) {
    expandedAnchor.value = null
  } else {
    expandedAnchor.value = node.anchor
  }
}
</script>

<template>
  <aside class="toc">
    <div class="toc-title">
      {{ title || '文章目录' }}
    </div>

    <nav class="toc-list">
      <ul>
        <TocTreeItem v-for="item in items" :key="item.anchor" :node="item" :root-anchor="item.anchor"
          :expanded-anchor="expandedAnchor" @toggle-root="toggleRoot" />
      </ul>
    </nav>
  </aside>
</template>