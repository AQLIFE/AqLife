<template>
  <div id="Home">{{ corpus }}</div>
</template>

<script setup lang="ts">
import { onBeforeMount, ref, type Ref } from 'vue'
import { CorpusApi } from '@/api'
import { apiConfiguration } from '@/services/api'

const corpusAPI = new CorpusApi(apiConfiguration)
const corpus: Ref<string|undefined> = ref()

onBeforeMount(async () => {
  corpus.value = await corpusAPI.apiCorpusRandomGet()
})
</script>

<style scoped>
#Home {
  display: flex;
  justify-content: center;
  align-items: center;
  height:100%;
  /* text-align: center;    */
}
</style>
