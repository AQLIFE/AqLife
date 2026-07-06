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

router.beforeEach(async (to, from) => {
  const accountStore = useAccountStore()

  // 确保状态已加载
  if (accountStore.systemAccount === null) {
    const request = new AccountApi(apiConfiguration)
    const account = await request.apiAccountGet()
    accountStore.systemAccount = account;
  }

   // 2. 定义匿名访问白名单
  const publicPaths = ['/home', '/login', '/register']
  // 错误页匹配：检查是否为通配符错误页
  const isErrorPage = to.matched.some(record => record.path === '/:pathMatch(.*)*')
  const isPublic = publicPaths.includes(to.path) || isErrorPage

  // 3. 核心权限拦截逻辑
  // A. 如果是私有路径且没有 Token，强制去登录 (Auth 契约校验 [cite: 217])
  if (!isPublic && !accountStore.bearerToken) {
    ElMessage.warning('请登录')
    return '/login'
  }

  // B. 业务逻辑补充：系统已初始化时，禁止进入注册页 (单账户系统逻辑)
  if (to.path === '/register' && accountStore.systemAccount) {
    ElMessage.warning('已有账户,拒绝注册,请登录!')
    return '/login'
  }

  // C. 体验优化：已登录状态下访问登录页，直接去首页
  if (to.path === '/login' && accountStore.bearerToken) {
    ElMessage.success('登录成功,正在载入后台控制面板')
    return '/home'
  }
})

export {router}
