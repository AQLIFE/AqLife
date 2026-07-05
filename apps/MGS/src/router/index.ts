import { createRouter, createWebHistory } from 'vue-router'
import type { Component } from 'vue'
import { MgsIconName } from '@aqlife/icons'
import { SidebarType } from '@/types/sidebarType'
import { routes } from './routes'

declare module 'vue-router' {
  interface RouteMeta {
    navTitle: string
    navIcon: MgsIconName | Component
    showInNav: boolean
    order?: number
    sidebarType?: SidebarType
  }
}

export const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes,
})
