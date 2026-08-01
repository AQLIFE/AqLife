import HomeView from '@/views/HomeView.vue'
import AboutView from '@/views/AboutView.vue'
import ErrorView from '@/views/ErrorView.vue'
import BlogView from '@/views/BlogView.vue'
import BlogPreview from '@/views/BlogPreview.vue'
import WishlistView from '@/views/WishlistView.vue'
import { WebIconName } from '@aqlife/icons'
import { SidebarType } from '@/types/sidebarType'

export const routes = [
  {
    path: '/:pathMatch(.*)*',
    component: ErrorView,
    meta: { navTitle: '错误', navIcon: WebIconName.Error, showInNav: false, order: 0 },
  },
  { path: '/', redirect: '/home' },
  {
    path: '/home',
    component: HomeView,
    meta: {
      navTitle: '首页',
      navIcon: WebIconName.Home,
      showInNav: false,
      order: 1,
      sidebarType: SidebarType.None,
    },
  },
  {
    path: '/about',
    component: AboutView,
    meta: {
      navTitle: '关于',
      navIcon: WebIconName.About,
      showInNav: true,
      order: 2,
      sidebarType: SidebarType.About,
    },
  },
  {
    path: '/blog',
    component: BlogView,
    meta: {
      navTitle: '建文',
      navIcon: WebIconName.Blog,
      showInNav: true,
      order: 3,
      sidebarType: SidebarType.None,
    },
  },
  {
    path: '/preview/:id',
    component: BlogPreview,
    meta: {
      navTitle: '预览',
      navIcon: WebIconName.Blog,
      showInNav: false,
      order: 3,
      sidebarType: SidebarType.Preview,
    },
  },
  {
    path: '/wish',
    component: WishlistView,
    meta: {
      navTitle: '心愿单',
      navIcon: WebIconName.Wish,
      showInNav: true,
      order: 4,
      sidebarType: SidebarType.None,
    },
  },
]
