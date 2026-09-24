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

```css
<style scoped>
.toc {
  height: 100%;
  display: flex;
  flex-direction: column;
  overflow: hidden;
}

.toc-title {
  flex: 0 0 auto;
  height: 36px;
  line-height: 36px;
  padding: 0 16px;
  font-size: 14px;
  font-weight: 600;
  color: #303133;
  border-bottom: 1px solid #ebeef5;
}

.toc-list {
  flex: 1;
  min-height: 0;
  overflow-y: auto;
  padding: 8px 0;
}

.toc-list > ul {
  margin: 0;
  padding: 0;
  list-style: none;
}

/* 滚动条 */
.toc-list::-webkit-scrollbar {
  width: 6px;
}

.toc-list::-webkit-scrollbar-thumb {
  border-radius: 3px;
  background: #dcdfe6;
}

.toc-list::-webkit-scrollbar-track {
  background: transparent;
}
</style>
```
