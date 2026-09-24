<script setup lang="ts">
import type { TocNode } from '@aqlife/domain'
import { computed } from 'vue'
import { useArticleStore } from '@/stores/articleStore'

const props = defineProps<{
  node: TocNode
  rootAnchor: string
  expandedAnchor: string | null
}>()

const emit = defineEmits<{
  toggleRoot: [node: TocNode]
}>()

const articleStore = useArticleStore()

const isActive = computed(() =>
  articleStore.activeAnchor === props.node.anchor,
)

const isRoot = computed(() =>
  props.node.anchor === props.rootAnchor,
)

const hasChildren = computed(() =>
  props.node.children.length > 0,
)

const isExpanded = computed(() =>
  props.expandedAnchor === props.rootAnchor,
)

function handleClick() {
  articleStore.activeAnchor = props.node.anchor
}

function toggleExpand(event: MouseEvent) {
  event.preventDefault()
  event.stopPropagation()

  emit('toggleRoot', props.node)
}
</script>

<template>
  <li class="toc-node">
    <div class="toc-node-row">
      <a class="toc-link" :class="{ active: isActive }" :href="`#${node.anchor}`" @click="handleClick">
        {{ node.title }}
      </a>

      <button v-if="isRoot && hasChildren" class="toc-toggle" type="button" :aria-label="isExpanded ? '折叠目录' : '展开目录'"
        :aria-expanded="isExpanded" @click="toggleExpand">
        <span class="toc-toggle-icon" :class="{ expanded: isExpanded }">
          ›
        </span>
      </button>
    </div>

    <ul v-if="hasChildren" v-show="isRoot ? isExpanded : true" class="toc-children">
      <TocTreeItem v-for="child in node.children" :key="child.anchor" :node="child" :root-anchor="rootAnchor"
        :expanded-anchor="expandedAnchor" @toggle-root="emit('toggleRoot', $event)" />
    </ul>
  </li>
</template>

<style lang="css" scoped>
.toc-node-row {
  display: flex;
  align-items: stretch;
}

.toc-link {
  position: relative;

  display: block;
  flex: 1;
  min-width: 0;

  padding-top: 6px;
  padding-left: 5px;
  padding-right: 8px;
  padding-bottom: 6px;

  color: inherit;
  text-decoration: none;

  line-height: 1.5;

  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;

  border-radius: 4px;

  transition:
    color 0.15s ease,
    background-color 0.15s ease;
}

.toc-link:hover {
  background-color: var(--el-fill-color-light);
  text-decoration: none;
}

.toc-link.active {
  font-weight: 600;
  color: var(--el-color-primary);
  background-color: var(--el-color-primary-light-9);
}

.toc-link.active::before {
  content: '';

  position: absolute;
  left: 0;
  top: 4px;
  bottom: 4px;

  width: 3px;

  background-color: var(--el-color-primary);
  border-radius: 0 2px 2px 0;
}

.toc-toggle {
  flex: 0 0 28px;

  display: flex;
  align-items: center;
  justify-content: center;

  padding: 0;

  border: 0;
  background: transparent;

  color: var(--el-text-color-secondary);

  cursor: pointer;

  border-radius: 4px;

  transition:
    color 0.15s ease,
    background-color 0.15s ease;
}

.toc-toggle:hover {
  color: var(--el-color-primary);
  background-color: var(--el-fill-color-light);
}

.toc-toggle-icon {
  display: inline-block;

  font-size: 18px;
  line-height: 1;

  transform: rotate(0deg);

  transition: transform 0.15s ease;
}

.toc-toggle-icon.expanded {
  transform: rotate(90deg);
}
</style>