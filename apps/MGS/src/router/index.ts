import { createRouter, createWebHistory } from 'vue-router'
import { routerRaw } from './RouteRaw'
import type { SidebarType } from '@/types/SiderBarType'
import type { IconName } from '@/datas/IconName'

declare module 'vue-router' {
  interface RouteMeta {
    navTitle: string
    navIcon: IconName
    showInNav: boolean
    order?: number
    sidebarType?: SidebarType
  }
}
const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: routerRaw,
})

export default router
