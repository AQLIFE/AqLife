import { createRouter, createWebHistory } from 'vue-router'
import type { Component } from 'vue'
import { MgsIconName } from '@aqlife/icons'
import { SidebarType } from '@/types/sidebarType'
import { routes } from './routes'
import { useAccountStore } from '@/stores/useAccountStore'
import { AccountApi } from '@/api'
import { apiConfiguration } from '@/services/api'
import { ElMessage, ElMessageBox } from 'element-plus'

declare module 'vue-router' {
  interface RouteMeta {
    navTitle: string
    navIcon: MgsIconName | Component
    showInNav: boolean
    order?: number
    sidebarType?: SidebarType
  }
}


const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes,
})

const accountApi = new AccountApi(apiConfiguration)
const accountStoreCache = useAccountStore()
let systemAccountChecked = false

async function ensureSystemAccount() {
  if (systemAccountChecked) return

  const response = await accountApi.apiAccountGetRaw()
  if (response.raw.status === 200) {
    accountStoreCache.systemAccount = await response.value()
  }

  systemAccountChecked = true
}

router.beforeEach(async (to) => {
  const accountStore = useAccountStore()

  // 登录/注册页需要知道系统是否已经初始化。
  // 不能依赖 App.vue 的异步初始化，否则首次打开 /login 时会出现竞态。
  if (to.path === '/login' || to.path === '/register') {
    await ensureSystemAccount()
  }

  const publicPaths = ['/home', '/login', '/register']
  const isErrorPage = to.matched.some(record => record.path === '/:pathMatch(.*)*')
  const isPublic = publicPaths.includes(to.path) || isErrorPage

  if (!isPublic && !accountStore.bearerToken) {
    ElMessage.warning('请登录')
    return '/home'
  }

  if (to.path === '/login' && accountStore.bearerToken) {
    ElMessage.success('已登录,正在载入后台控制面板')
    return '/home'
  }

  if (to.path === '/login' && !accountStore.systemAccount) {
    ElMessage.warning('系统尚未初始化,请先注册')
    return '/register'
  }

  if (to.path === '/register' && accountStore.systemAccount) {
    ElMessage.warning('已有账户,请登录')
    return '/login'
  }
})

export {router}
