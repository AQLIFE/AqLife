<template>
  <ElRow id="Home">{{ corpus }}</ElRow>
</template>

<script setup lang="ts">
import { onBeforeMount, ref, type Ref } from 'vue'
import { CorpusApi } from '@/api'
import { apiConfiguration } from '@/services/api'
import { ElRow } from 'element-plus'

const corpusAPI = new CorpusApi(apiConfiguration)
const corpus: Ref<string|undefined> = ref()

onBeforeMount(async () => {
  corpus.value = await corpusAPI.apiCorpusRandomGet()
})
</script>

<style scoped>
/* #Home {
    flex-grow: 1;
} */
</style>
