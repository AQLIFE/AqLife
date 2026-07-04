import { createWebHistory, createRouter } from 'vue-router'
import 'vue-router'
import { IconName, SidebarType } from '@/types/define'
import { routerRaw } from './routerRaw'

declare module 'vue-router' {
  interface RouteMeta {
    navTitle: string
    navIcon: IconName
    showInNav: boolean
    order?: number
    sidebarType?: SidebarType
  }
}

export const router = createRouter({
  history: createWebHistory(),
  routes: routerRaw,
})
