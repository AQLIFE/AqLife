<template>
  <div class="toolbar">
    <div class="filters"><slot /></div>
    <div class="actions"><slot name="actions" /></div>
  </div>
</template>

<style scoped>
.toolbar { display:flex;align-items:center;justify-content:space-between;gap:16px;padding:0 28px 14px; }
.filters { display:flex;align-items:center;gap:8px;min-width:0;flex:1; }
.actions { display:flex;align-items:center;gap:8px; }
</style>
