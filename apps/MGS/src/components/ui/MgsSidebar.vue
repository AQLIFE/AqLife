<template>
  <aside class="sidebar">
    <div class="brand">
      <div class="brand-mark">M</div>
      <div class="brand-copy">
        <strong>MGS</strong>
        <span>Management Studio</span>
      </div>
    </div>

    <nav class="nav">
      <RouterLink class="nav-item" to="/home">
        <ElIcon><House /></ElIcon>
        <span>Overview</span>
      </RouterLink>

      <div class="nav-section">CONTENT</div>
      <RouterLink class="nav-item" to="/blog/list">
        <ElIcon><Document /></ElIcon>
        <span>Blog</span>
      </RouterLink>
      <RouterLink class="nav-item" to="/todo">
        <ElIcon><List /></ElIcon>
        <span>Todo</span>
      </RouterLink>

      <div class="nav-section">SYSTEM</div>
      <RouterLink class="nav-item" to="/tag">
        <ElIcon><PriceTag /></ElIcon>
        <span>Tags</span>
      </RouterLink>
      <RouterLink class="nav-item" to="/account/profile">
        <ElIcon><User /></ElIcon>
        <span>Account</span>
      </RouterLink>
    </nav>

    <div class="sidebar-footer">
      <span class="status-dot"></span>
      <span>Workspace ready</span>
    </div>
  </aside>
</template>

<script setup lang="ts">
import { ElIcon } from 'element-plus'
import { Document, House, List, PriceTag, User } from '@element-plus/icons-vue'
</script>

<style scoped>
.sidebar {
  width: 236px;
  min-width: 236px;
  height: 100vh;
  box-sizing: border-box;
  display: flex;
  flex-direction: column;
  padding: 18px 12px 14px;
  background: var(--mgs-surface);
  border-right: 1px solid var(--mgs-border);
}
.brand { display:flex; align-items:center; gap:10px; padding:4px 10px 24px; }
.brand-mark { width:30px;height:30px;border-radius:8px;display:grid;place-items:center;background:var(--mgs-text);color:#fff;font-weight:700; }
.brand-copy { display:flex;flex-direction:column;line-height:1.1; }
.brand-copy strong { font-size:15px;letter-spacing:.02em; }
.brand-copy span { margin-top:4px;font-size:11px;color:var(--mgs-muted); }
.nav { display:flex;flex-direction:column;gap:3px; }
.nav-section { margin:18px 10px 5px;font-size:10px;font-weight:700;letter-spacing:.12em;color:var(--mgs-muted); }
.nav-item { display:flex;align-items:center;gap:10px;height:38px;padding:0 10px;border-radius:8px;color:var(--mgs-secondary);text-decoration:none;font-size:13px;transition:.15s ease; }
.nav-item:hover { background:var(--mgs-surface-soft);color:var(--mgs-text); }
.nav-item.router-link-active { background:var(--mgs-accent-soft);color:var(--mgs-accent);font-weight:600; }
.nav-item .el-icon { font-size:16px; }
.sidebar-footer { margin-top:auto;padding:10px;font-size:11px;color:var(--mgs-muted);display:flex;align-items:center;gap:7px; }
.status-dot { width:6px;height:6px;border-radius:50%;background:var(--mgs-success); }
</style>
