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

    <div class="sidebar-footer" role="timer" :aria-label="`Time remaining ${formattedTime}`">
      <ElIcon class="timer-icon"><Clock /></ElIcon>
      <div class="timer-copy">
        <span class="timer-label">Time remaining</span>
        <strong class="timer-value">{{ formattedTime }}</strong>
      </div>
    </div>
  </aside>
</template>

<script setup lang="ts">
import { computed, onUnmounted, ref, watch, type Component } from 'vue'
import { ElIcon } from 'element-plus'
import { Clock } from '@element-plus/icons-vue'
import { useRouter } from 'vue-router'
import { MgsIconName, mgsIconRegistry } from '@aqlife/icons'
import { useAccountStore } from '@/stores/useAccountStore'

const router = useRouter()
const accountStore = useAccountStore()

const SESSION_DURATION_MS = 60 * 60 * 1000
const COUNTDOWN_STORAGE_KEY = 'mgs-workspace-countdown-end'
const remainingSeconds = ref(60 * 60)
let countdownInterval: ReturnType<typeof setInterval> | undefined

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

const formattedTime = computed(() => {
  const minutes = Math.floor(remainingSeconds.value / 60)
  const seconds = remainingSeconds.value % 60
  return `${String(minutes).padStart(2, '0')}:${String(seconds).padStart(2, '0')}`
})

function resolveIcon(icon: MgsIconName | Component) {
  return typeof icon === 'string'
    ? mgsIconRegistry[icon as MgsIconName]
    : icon
}

function stopCountdown() {
  if (countdownInterval) {
    clearInterval(countdownInterval)
    countdownInterval = undefined
  }
}

function updateRemainingTime(endTime: number) {
  remainingSeconds.value = Math.max(0, Math.ceil((endTime - Date.now()) / 1000))
  if (remainingSeconds.value === 0) stopCountdown()
}

function startCountdown() {
  stopCountdown()

  const storedEndTime = Number(sessionStorage.getItem(COUNTDOWN_STORAGE_KEY))
  const endTime = storedEndTime > Date.now()
    ? storedEndTime
    : Date.now() + SESSION_DURATION_MS

  sessionStorage.setItem(COUNTDOWN_STORAGE_KEY, String(endTime))
  updateRemainingTime(endTime)

  if (remainingSeconds.value > 0) {
    countdownInterval = setInterval(() => updateRemainingTime(endTime), 1000)
  }
}

watch(
  () => accountStore.bearerToken,
  token => {
    if (token) {
      startCountdown()
    } else {
      stopCountdown()
      sessionStorage.removeItem(COUNTDOWN_STORAGE_KEY)
      remainingSeconds.value = 60 * 60
    }
  },
  { immediate: true },
)

onUnmounted(stopCountdown)
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
.sidebar-footer { margin-top:auto;padding:10px;display:flex;align-items:center;gap:9px;border-top:1px solid var(--mgs-border); }
.timer-icon { color:var(--mgs-accent);font-size:17px; }
.timer-copy { min-width:0;display:flex;flex-direction:column;gap:3px; }
.timer-label { color:var(--mgs-muted);font-size:10px;letter-spacing:.04em; }
.timer-value { color:var(--mgs-text);font-size:15px;font-variant-numeric:tabular-nums;letter-spacing:.04em; }
</style>
