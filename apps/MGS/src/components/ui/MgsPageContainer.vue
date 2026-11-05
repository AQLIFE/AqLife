<template>
  <div class="page-container" :class="{ 'flush-bottom': flushBottom }">
    <slot />
  </div>
</template>

<script setup lang="ts">
withDefaults(defineProps<{
  flushBottom?: boolean
}>(), {
  flushBottom: false,
})
</script>

<style scoped>
.page-container {
  width: 100%;
  min-width: 0;
  padding: 0 24px 24px;
}

.page-container.flush-bottom {
  padding-bottom: 0;
}

@media (max-width: 640px) {
  .page-container {
    padding-bottom: 16px;
  }

  .page-container.flush-bottom {
    padding-bottom: 0;
  }
}
</style>
