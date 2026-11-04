<template>
  <div class="page-container">
    <slot />
  </div>
</template>

<style scoped>
.page-container {
  width: 100%;
  min-width: 0;
  padding: 0 24px 24px;
}

@media (max-width: 640px) {
  .page-container {
    padding-bottom: 16px;
  }
}
</style>
