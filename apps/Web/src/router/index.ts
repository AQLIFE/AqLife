import { createWebHistory, createRouter } from 'vue-router'
import 'vue-router'
import { WebIconName } from '@aqlife/icons'
import { SidebarType } from '@/types/sidebarType'
import { routes } from './routes'

declare module 'vue-router' {
  interface RouteMeta {
    navTitle: string
    navIcon: WebIconName
    showInNav: boolean
    order?: number
    sidebarType?: SidebarType
  }
}

export const router = createRouter({
  history: createWebHistory(),
  routes,
})
