<script setup lang="ts">
import { notify, useLoadingTask } from '@aqlife/ui-shared'
import { RouterView, useRoute } from 'vue-router'
import EntrancePanel from './components/EntrancePanel.vue'
import { useAccountStore } from '@/stores/useAccountStore'
import { AccountApi } from '@/api'
import { apiConfiguration } from './services/api.ts'
import { computed, onBeforeMount } from 'vue'
import MgsSidebar from '@/components/ui/MgsSidebar.vue'
import MgsTopbar from '@/components/ui/MgsTopbar.vue'

const accountStore = useAccountStore()
const request = new AccountApi(apiConfiguration)
const route = useRoute()
const bootstrapLoading = useLoadingTask('mgs:app:bootstrap')
const finishBootstrap = bootstrapLoading.begin()
const loading = bootstrapLoading.isLoading

const currentTitle = computed(() => {
  if (route.path.startsWith('/blog')) return 'Blog'
  if (route.path.startsWith('/todo')) return 'Todo'
  if (route.path.startsWith('/corpus')) return '语录管理'
  if (route.path.startsWith('/development-plan')) return '开发路线图'
  if (route.path.startsWith('/tag')) return 'Tags'
  if (route.path.startsWith('/favorite')) return '收藏管理'
  if (route.path.startsWith('/account')) return 'Account'
  return 'Overview'
})

onBeforeMount(async () => {
  try {
    const response = await request.apiAccountGetRaw()
    if (response.raw.status === 200) {
      accountStore.systemAccount = await response.value()
    } else if (response.raw.status !== 204) {
      notify.error('未配置账户,请进入注册流程')
    }
  } catch (error) {
    notify.fromError(error, '账户初始化失败')
  } finally {
    finishBootstrap()
  }
})
</script>

<template>
  <!-- 登录/注册页面交给路由页面自己渲染。 -->
  <div v-if="route.meta.sidebarType === 'Login' || route.meta.sidebarType === 'Register'" class="auth-layout">
    <RouterView />
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
.auth-layout { min-height:100vh; }
.entrance-layout { min-height:100vh;display:flex;align-items:center;justify-content:center; }
.mgs-fade-enter-active,.mgs-fade-leave-active { transition:opacity .12s ease,transform .12s ease; }
.mgs-fade-enter-from,.mgs-fade-leave-to { opacity:0;transform:translateY(3px); }
</style>
