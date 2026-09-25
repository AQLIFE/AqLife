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
      <button
        v-if="isRoot && hasChildren"
        class="toc-toggle"
        type="button"
        :aria-label="isExpanded ? '折叠目录' : '展开目录'"
        :aria-expanded="isExpanded"
        @click="toggleExpand"
      >
        <span
          class="toc-toggle-icon"
          :class="{ expanded: isExpanded }"
        >
          ›
        </span>
      </button>

      <span
        v-else-if="isRoot"
        class="toc-toggle-placeholder"
        aria-hidden="true"
      />

      <a
        class="toc-link"
        :class="{ active: isActive }"
        :href="`#${node.anchor}`"
        @click="handleClick"
      >
        {{ node.title }}
      </a>
    </div>

    <ul
      v-if="hasChildren"
      v-show="isRoot ? isExpanded : true"
      class="toc-children"
    >
      <TocTreeItem
        v-for="child in node.children"
        :key="child.anchor"
        :node="child"
        :root-anchor="rootAnchor"
        :expanded-anchor="expandedAnchor"
        @toggle-root="emit('toggleRoot', $event)"
      />
    </ul>
  </li>
</template>

<style lang="css" scoped>
.toc-node {
  margin: 0;
  padding: 0;
  list-style: none;
}

.toc-node-row {
  display: flex;
  align-items: center;
  min-width: 0;
}

/*
 * 根节点左侧折叠区域
 */
.toc-toggle,
.toc-toggle-placeholder {
  flex: 0 0 28px;
  width: 28px;
}

.toc-toggle {
  display: flex;
  align-items: center;
  justify-content: center;

  height: 30px;

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

/*
 * 导航标题
 */
.toc-link {
  position: relative;

  display: block;
  flex: 1;
  min-width: 0;

  padding: 6px 10px 6px 6px;

  color: var(--el-text-color-regular);
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
  color: var(--el-color-primary);
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
  top: 4px;
  bottom: 4px;
  left: 0;

  width: 3px;

  background-color: var(--el-color-primary);
  border-radius: 0 2px 2px 0;
}

/*
 * 子导航区域
 *
 * 这里同时负责：
 * 1. 子导航整体向右缩进
 * 2. 绘制贯穿子导航的竖线
 *
 * 28px 是父节点折叠区域，
 * 32px 是额外的子层级缩进。
 *
 * 因此子导航比父导航明显更深入。
 */
.toc-children {
  position: relative;

  margin: 0;
  padding: 2px 0 4px 32px;

  list-style: none;
}

/*
 * 子导航层级线
 *
 * 不直接使用 border-left，
 * 而使用伪元素，这样可以精确控制：
 * - 竖线位置
 * - 上下长度
 * - 颜色
 */
.toc-children::before {
  content: '';

  position: absolute;

  top: 0;
  bottom: 4px;
  left: 14px;

  width: 3px;

  background-color: var(--el-border-color-darker);

  pointer-events: none;
}

/*
 * 子节点标题
 */
.toc-children .toc-link {
  padding-left: 8px;
}

/*
 * 子导航之间保持紧凑
 */
.toc-children > .toc-node {
  margin: 0;
}

/*
 * 子导航 hover
 */
.toc-children .toc-link:hover {
  color: var(--el-color-primary);
  background-color: var(--el-fill-color-light);
}

/*
 * 子导航 active
 */
.toc-children .toc-link.active {
  color: var(--el-color-primary);
  background-color: var(--el-color-primary-light-9);
}
</style>