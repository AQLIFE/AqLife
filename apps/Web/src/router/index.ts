import { createWebHistory, createRouter } from 'vue-router'
import 'vue-router'
import { WebIconName } from '@aqlife/icons'
import { SidebarType } from '@/types/sidebarType'
import { routes } from './routes'
import { useAuthorInfoStore } from '@/stores/useAuthorInfoStore'

declare module 'vue-router' {
  interface RouteMeta {
    navTitle: string
    navIcon: WebIconName
    showInNav: boolean
    order?: number
    sidebarType?: SidebarType
  }
}

const router = createRouter({
  history: createWebHistory(),
  routes,
})

router.beforeEach(async (to) => {
  const accountStore = useAuthorInfoStore()

  // 防止 unavailable 页面自己再次触发守卫逻辑
  if (to.name === 'account-unavailable') {
    return true
  }

  if (accountStore.status === 'idle') {
    await accountStore.getUser()
  }

  if (
    accountStore.status === 'empty' ||
    accountStore.status === 'error'
  ) {
    return {
      name: 'account-unavailable',
    }
  }

  return true
})

export {router}
