<template>
  <header class="topbar">
    <div class="breadcrumb">
      <span>MGS</span>
      <span class="separator">/</span>
      <strong>{{ title }}</strong>
    </div>
    <div class="actions">
      <div class="shortcut">⌘ K</div>
      <ElButton text circle :icon="Setting" aria-label="Settings" />
      <ElAvatar :size="28">M</ElAvatar>
    </div>
  </header>
</template>

<script setup lang="ts">
import { ElAvatar, ElButton } from 'element-plus'
import { Setting } from '@element-plus/icons-vue'

defineProps<{ title: string }>()
</script>

<style scoped>
.topbar { height:52px;min-height:52px;box-sizing:border-box;padding:0 22px;border-bottom:1px solid var(--mgs-border);display:flex;align-items:center;justify-content:space-between;background:color-mix(in srgb,var(--mgs-surface) 94%,transparent); }
.breadcrumb { display:flex;align-items:center;gap:8px;font-size:12px;color:var(--mgs-muted); }
.breadcrumb strong { color:var(--mgs-text);font-weight:600; }
.separator { color:var(--mgs-border-strong); }
.actions { display:flex;align-items:center;gap:8px; }
.shortcut { padding:3px 7px;border:1px solid var(--mgs-border);border-radius:5px;color:var(--mgs-muted);font-size:10px;background:var(--mgs-surface-soft); }
</style>
