<script setup lang="ts">
import { ElMessage } from 'element-plus'
import { RouterView, useRoute } from 'vue-router'
import RegisterStep from './components/RegisterStep.vue'
import EntrancePanel from './components/EntrancePanel.vue'
import { SidebarType } from '@/types/sidebarType'
import { useAccountStore } from '@/stores/useAccountStore'
import { AccountApi } from '@/api'
import { apiConfiguration } from './services/api.ts'
import { computed, onBeforeMount, ref } from 'vue'
import MgsSidebar from '@/components/ui/MgsSidebar.vue'
import MgsTopbar from '@/components/ui/MgsTopbar.vue'

const accountStore = useAccountStore()
const request = new AccountApi(apiConfiguration)
const route = useRoute()
const loading = ref(true)

const currentTitle = computed(() => {
  if (route.path.startsWith('/blog')) return 'Blog'
  if (route.path.startsWith('/todo')) return 'Todo'
  if (route.path.startsWith('/tag')) return 'Tags'
  if (route.path.startsWith('/account')) return 'Account'
  return 'Overview'
})

const authPanel = computed(() => {
  if (route.meta.sidebarType === SidebarType.Register) return RegisterStep
  return EntrancePanel
})

onBeforeMount(async () => {
  try {
    const response = await request.apiAccountGetRaw()
    if (response.raw.status === 200) {
      accountStore.systemAccount = await response.value()
    } else if (response.raw.status !== 204) {
      ElMessage.error('未配置账户,请进入注册流程')
    }
  } finally {
    loading.value = false
  }
})
</script>

<template>
  <div v-if="route.meta.sidebarType === SidebarType.Login || route.meta.sidebarType === SidebarType.Register" class="auth-layout">
    <component :is="authPanel" />
  </div>

  <div v-else-if="!accountStore.bearerToken" class="entrance-layout" v-loading.fullscreen.lock="loading">
    <EntrancePanel />
  </div>

  <div v-else class="mgs-shell" v-loading.fullscreen.lock="loading">
    <MgsSidebar />
    <main class="workspace">
      <MgsTopbar :title="currentTitle" />
      <section class="workspace-content">
        <RouterView v-slot="{ Component }">
          <transition name="mgs-fade" mode="out-in">
            <component :is="Component" />
          </transition>
        </RouterView>
      </section>
    </main>
  </div>
</template>

<style scoped>
.mgs-shell { min-height:100vh;display:flex;background:var(--mgs-bg); }
.workspace { min-width:0;flex:1;height:100vh;display:flex;flex-direction:column;overflow:hidden; }
.workspace-content { min-height:0;flex:1;overflow:auto; }
.auth-layout { min-height:100vh; }\n.entrance-layout { min-height:100vh; }
.mgs-fade-enter-active,.mgs-fade-leave-active { transition:opacity .12s ease,transform .12s ease; }
.mgs-fade-enter-from,.mgs-fade-leave-to { opacity:0;transform:translateY(3px); }
</style>
