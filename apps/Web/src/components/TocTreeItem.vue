<script setup lang="ts">
import type { TocNode } from '@aqlife/domain'

defineProps<{
  node: TocNode
}>()
</script>

<template>
  <li class="toc-node">
    <a
      class="toc-link"
      :href="`#${node.anchor}`"
      :style="{
        paddingLeft: `${16 + (node.level - 2) * 16}px`,
      }"
    >
      {{ node.title }}
    </a>

    <ul v-if="node.children.length">
      <TocNode
        v-for="child in node.children"
        :key="child.anchor"
        :node="child"
      />
    </ul>
  </li>
</template>

<style scoped>
.toc-node {
  margin: 0;
  padding: 0;

  list-style: none;
}

.toc-link {
  display: block;

  padding-top: 6px;
  padding-right: 12px;
  padding-bottom: 6px;

  color: inherit;
  text-decoration: none;

  line-height: 1.5;

  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.toc-link:hover {
  text-decoration: underline;
}
</style>