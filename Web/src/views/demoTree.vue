<!-- tree.vue -->
<template>
  <div class="skill-tree-container">
    <svg class="skill-tree-svg" :width="width" :height="height">
      <!-- 连接线 -->
      <g class="skill-connections">
        <path
          v-for="connection in connections"
          :key="connection.id"
          :d="connection.path"
          class="skill-connection"
        />
      </g>
      <!-- 技能节点 -->
      <g class="skill-nodes">
        <g
          v-for="node in nodes"
          :key="node.id"
          :transform="`translate(${node.x}, ${node.y})`"
        >
          <skill-node
            :skill-name="node.name"
            @click="handleNodeClick(node)"
          />
        </g>
      </g>
    </svg>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { type ISkillNode } from '@/data/SkillData.ts'
import SkillNode from './skillNode.vue'

interface TreeNode {
  id: string
  name: string
  x: number
  y: number
  children: TreeNode[]
  parent: TreeNode | null
}

const props = defineProps<{
  skills: ISkillNode[]
}>()

const width = ref(800)
const height = ref(600)
const nodes = ref<TreeNode[]>([])
const connections = ref<Array<{id: string, path: string}>>([])

// 计算节点位置
const calculatePositions = () => {
  const tree = buildTree()
  layoutTree(tree)
  updateConnections(tree)
}

// 构建树结构
const buildTree = (): TreeNode => {
  const nodeMap = new Map<string, TreeNode>()
  
  // 创建节点映射
  props.skills.forEach(skill => {
    nodeMap.set(skill.Id, {
      id: skill.Id,
      name: skill.Name,
      x: 0,
      y: 0,
      children: [],
      parent: null
    })
  })

  // 建立父子关系
  props.skills.forEach(skill => {
    skill.Prerequisites.forEach(parentId => {
      const parent = nodeMap.get(parentId)
      const child = nodeMap.get(skill.Id)
      if (parent && child) {
        parent.children.push(child)
        child.parent = parent
      }
    })
  })

  // 找到根节点
  const root = Array.from(nodeMap.values()).find(node => !node.parent)
  if (!root) throw new Error('无法找到根节点')

  return root
}

// 布局算法
const layoutTree = (node: TreeNode, level = 0, x = width.value / 2) => {
  const spacing = 100
  const levelWidth = 200
  
  // 设置当前节点位置
  node.x = x
  node.y = level * spacing
  
  // 递归布局子节点
  if (node.children.length > 0) {
    const childSpacing = levelWidth / (node.children.length + 1)
    node.children.forEach((child, index) => {
      const childX = x - levelWidth/2 + (index + 1) * childSpacing
      layoutTree(child, level + 1, childX)
    })
  }
}

// 更新连接线
const updateConnections = (node: TreeNode) => {
  if (node.children.length > 0) {
    node.children.forEach(child => {
      connections.value.push({
        id: `${node.id}-${child.id}`,
        path: `M ${node.x} ${node.y + 50} C ${node.x} ${node.y + 80}, ${child.x} ${child.y - 20}, ${child.x} ${child.y + 50}`
      })
      updateConnections(child)
    })
  }
}

// 处理节点点击
const handleNodeClick = (node: TreeNode) => {
  console.log('点击了节点:', node.name)
}

onMounted(() => {
  calculatePositions()
})
</script>

<style scoped>
.skill-tree-container {
  position: relative;
  overflow: auto;
}

.skill-tree-svg {
  background: #f5f5f5;
}

.skill-connection {
  stroke: #666;
  stroke-width: 2;
  fill: none;
}

.skill-nodes {
  cursor: pointer;
}

.skill-node {
  transition: all 0.3s ease;
}

.skill-node:hover {
  transform: scale(1.05);
}
</style>