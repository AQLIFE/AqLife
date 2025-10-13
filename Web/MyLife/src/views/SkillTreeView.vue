<script setup lang="ts">
import SkillNode from '@/components/SkillNode.vue';
import { demoData, type SkillTree, type ISkillNode } from '@/data/SkillData'
import { computed, onMounted, type Ref, ref } from 'vue';


const rect: Ref<{ x: number, y: number }> = ref({ x: 0, y: 0 })


function initXY() {
    const tree = document.getElementById('tree')
    const fs = tree?.getBoundingClientRect()
    if (fs != null) return { x: fs.width / 2 - 75, y: fs.height / 2 - 25 };
    else return { x: 0, y: 0 }
}
onMounted(() => {
    rect.value = initXY();
    demoData.forEach(node => {
        positions.value[node.Id] = getNodePosition(node.Id)
    })
    console.log(rect.value)
})


const nodePositions = computed(() => {
    const positions: Record<string, { x: number; y: number }> = {}
    demoData.forEach((skill, index) => {
        positions[skill.Id] = {
            x: rect.value.x + 75, // 矩形中心点
            y: rect.value.y + 75 * index + 25 // 矩形垂直中心点
        }
    })
    return positions
})

const connectionPaths = computed(() => {
    return demoData.flatMap(skill => {
        return skill.Prerequisites.map(prerequisiteId => {
            const start = nodePositions.value[prerequisiteId]
            const end = nodePositions.value[skill.Id]

            // 使用折线连接，通过中间点创建直角转折
            const midX = start.x > end.x ? start.x : end.x

            return `
        M ${start.x},${start.y}
        L ${midX},${start.y}
        L ${midX},${end.y}
        L ${end.x},${end.y}
      `
        })
    })
})


const maxCount = computed(() => {
    return Math.max(...demoData.map(item =>
        demoData.filter(s => s.Prerequisites?.includes(item.Id)).length
    ))
})

const nodeLevels = computed(() => {
    const levels: Record<string, number> = {}
    demoData.forEach(node => {
        levels[node.Id] = getLevel(node, demoData)
    })
    return levels
})

const nodeIndices = computed(() => {
    const indices: Record<string, number> = {}
    Object.entries(groupByLevel()).forEach(([level, nodes]) => {
        nodes.forEach((node, index) => {
            indices[node.Id] = index
        })
    })
    return indices
})

function groupByLevel() {
    const groups: Record<number, ISkillNode[]> = {}
    demoData.forEach(node => {
        const level = nodeLevels.value[node.Id]
        if (!groups[level]) groups[level] = []
        groups[level].push(node)
    })
    return groups
}

function getCount(source: SkillTree) {
    let max = 0;
    for (const obj of source) {
        const cnt = getIndex(obj, source)
        max = cnt > max ? cnt : max
    }
    return max
}

function getNodePosition(nodeId: string) {
    const node = demoData.find(n => n.Id === nodeId)!
    const level = nodeLevels.value[nodeId]
    const index = nodeIndices.value[nodeId]

    return {
        x: 200 * index + 150,
        y: 75 * level + 50
    }
}

// 使用ref存储位置数据，避免重复计算
const positions = ref<Record<string, { x: number; y: number }>>({})


function getIndex(node: ISkillNode, source: SkillTree): number {
    let cnt = 0;
    for (const item of source) {
        if (item.Prerequisites[0] == node.Prerequisites[0] && node.Id != item.Id) cnt += 1;
        else if (item.Prerequisites[0] == node.Prerequisites[0] && node.Id == item.Id) return cnt
    }
    return 0;
}

/**
 * 返回对应节点的层级
 * @param item 节点元素
 * @param source 源树
 */
function getLevel(node: ISkillNode, source: SkillTree): number {
    if (node.Prerequisites != null) {
        const next = source.find(e => node.Prerequisites[0] == e.Id)
        if (next != null || next != undefined)
            return getLevel(next, source) + 1;
        else {
            return 0;
        }
    }
    else return 0;
}

</script>

<template>
    <svg id="tree">
        <!-- <SkillNode :x="rect.x + 200 * getIndex(item, demoData)" :y="rect.y + 75 * getLevel(item, demoData)"
            :skill-name="item.Name" :title="item.Description" :node-id="item.Id" v-for="item, index in demoData"
            :key="index" /> -->

        <SkillNode v-for="item, index in demoData"  :x="positions[item.Id].x" :y="positions[item.Id].y" :skill-name="item.Name" :title="item.Description"
            :node-id="item.Id" :key="index" />
        <path v-for="(path, index) in connectionPaths" :key="'line-' + index" :d="path" stroke="#666" stroke-width="2"
            fill="none" marker-end="url(#arrow)" />
    </svg>
</template>

<style lang="css" scoped>
#tree {
    width: 100%;
    height: 100%;
}

defs {
    .marker {
        fill: #666;
        stroke: #666;
        opacity: 0.8;
    }
}

#arrow {
    markerWidth: 8;
    markerHeight: 8;
    markerUnits: strokeWidth;
}

path {
    transition: stroke-dashoffset 0.3s ease;
    stroke-dasharray: 5;
    stroke-dashoffset: 0;
}
</style>
