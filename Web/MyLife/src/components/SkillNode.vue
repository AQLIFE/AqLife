<script setup lang="ts">
const props = defineProps<{ skillName: string, x: number, y: number, nodeId: string }>()
</script>

<template>
    <g :data-node-id="nodeId">
        <rect :x="props.x" :y="props.y" width="150" height="50" fill="white" stroke="blue" stroke-width="3" />
        <text :x="props.x + 75" :y="props.y + 37" font-family="Cascadia Code PL" font-size="2rem" text-anchor="middle"
            fill="black">{{ skillName }}</text>
    </g>
</template>