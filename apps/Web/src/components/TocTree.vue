<script setup lang="ts">
import type { TocNode } from '@aqlife/domain'
import TocNodeItem from './TocTreeItem.vue'

defineProps<{
  items: TocNode[]
  title?: string
}>()
</script>

<template>
  <aside class="toc">
    <div class="toc-title">
      {{ title || '文章目录' }}
    </div>

    <nav class="toc-list">
      <ul>
        <TocNodeItem
          v-for="item in items"
          :key="item.anchor"
          :node="item"
        />
      </ul>
    </nav>
  </aside>
</template>

<style scoped>
.toc {
  display: flex;
  flex-direction: column;

  width: 100%;
  height: 100%;
  min-height: 0;

  overflow: hidden;
}

.toc-title {
  flex: 0 0 30px;

  height: 30px;
  line-height: 30px;

  padding-left: 20px;

  font-weight: 500;
}

.toc-list {
  min-height: 0;

  overflow-y: auto;
}

.toc-list ul {
  margin: 0;
  padding: 0;

  list-style: none;
}
</style>