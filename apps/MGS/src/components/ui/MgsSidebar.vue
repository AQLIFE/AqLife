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
      <section v-for="group in navGroups" :key="group.title" class="nav-group">
        <div class="nav-section">{{ group.title }}</div>
        <RouterLink
          v-for="item in group.items"
          :key="item.path"
          class="nav-item"
          :to="item.path"
        >
          <ElIcon><component :is="resolveIcon(item.meta.navIcon)" /></ElIcon>
          <span>{{ item.meta.navTitle }}</span>
        </RouterLink>
      </section>
    </nav>

    <div class="sidebar-footer">
      <span class="status-dot"></span>
      <span>Workspace ready</span>
    </div>
  </aside>
</template>

<script setup lang="ts">
import { computed, type Component } from 'vue'
import { ElIcon } from 'element-plus'
import { useRouter } from 'vue-router'
import { MgsIconName, mgsIconRegistry } from '@aqlife/icons'

const router = useRouter()

const navGroups = computed(() => {
  const groups = new Map<string, { title: string; items: ReturnType<typeof router.getRoutes> }>()

  const visibleRoutes = router.getRoutes()
    .filter(route => route.meta.showInNav && route.meta.navTitle && route.meta.navIcon)
    .sort((a, b) => (a.meta.order ?? 0) - (b.meta.order ?? 0))

  for (const route of visibleRoutes) {
    const title = route.meta.navSection || 'OTHER'
    const group = groups.get(title) ?? { title, items: [] }
    group.items.push(route)
    groups.set(title, group)
  }

  return [...groups.values()]
})

function resolveIcon(icon: MgsIconName | Component) {
  return typeof icon === 'string'
    ? mgsIconRegistry[icon as MgsIconName]
    : icon
}
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
.nav-group + .nav-group { margin-top:12px; }
.nav-section { margin:8px 10px 5px;font-size:10px;font-weight:700;letter-spacing:.12em;color:var(--mgs-muted); }
.nav-item { display:flex;align-items:center;gap:10px;height:38px;padding:0 10px;border-radius:8px;color:var(--mgs-secondary);text-decoration:none;font-size:13px;transition:.15s ease; }
.nav-item:hover { background:var(--mgs-surface-soft);color:var(--mgs-text); }
.nav-item.router-link-active { background:var(--mgs-accent-soft);color:var(--mgs-accent);font-weight:600; }
.nav-item .el-icon { font-size:16px; }
.sidebar-footer { margin-top:auto;padding:10px;font-size:11px;color:var(--mgs-muted);display:flex;align-items:center;gap:7px; }
.status-dot { width:6px;height:6px;border-radius:50%;background:var(--mgs-success); }
</style>
