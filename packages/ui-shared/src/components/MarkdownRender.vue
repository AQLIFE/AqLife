<template>
    <div id="mdRender" class="preview">
        <template v-for="(node, index) in rNode" :key="index">
            <CodeBlock v-if="node.type === 'component' && node.component === 'CodeBlock'" :info="node.content"
                :infoType="node.info" />
            <MermaidPreview v-else-if="node.type === 'component' && node.component === 'MermaidPreview'"
                :info="node.content" :info-type="node.info" />
            <TipPreview v-else-if="node.type === 'blockquote'" :quoteTokens="node.tokens" />
            <TablePreview v-else-if="node.type === 'table'" :tableTokens="node.tokens" />
            <div v-else-if="node.type === 'html'" v-html="node.content" />
        </template>
    </div>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import CodeBlock from './CodeBlock.vue'
import TipPreview from './TipPreview.vue'
import TablePreview from './TablePreview.vue'
import MermaidPreview from './MermaidPreview.vue';
import { mdRenderOption, renderNodes } from '@aqlife/domain'

const props = defineProps<{ markdown: string }>()
const rNode = computed(() => renderNodes(mdRenderOption.parse(props.markdown, {})))
</script>