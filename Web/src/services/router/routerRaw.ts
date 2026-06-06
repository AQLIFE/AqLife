import HomeView from '@/views/HomeView.vue'
import AboutView from '@/views/AboutView.vue'
import ErrorView from '@/views/ErrorView.vue'
import BlogView from '@/views/BlogView.vue'
import BlogPreview from '@/views/BlogPreview.vue'
import WishlistView from '@/views/WishlistView.vue'
import OptionAccountView from '@/views/OptionAccountView.vue'
import { IconName, SidebarType } from '@/types/define'

export const routerRaw = [
  {
    path: '/:pathMatch(.*)*',
    component: ErrorView,
    meta: { navTitle: '错误', navIcon: IconName.Error, showInNav: false, order: 0 },
  },
  { path: '/', redirect: '/home' },
  {
    path: '/home',
    component: HomeView,
    meta: {
      navTitle: '首页',
      navIcon: IconName.Home,
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
      navIcon: IconName.About,
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
      navIcon: IconName.Blog,
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
      navIcon: IconName.Blog,
      showInNav: false,
      order: 3,
      sidebarType: SidebarType.None,
    },
  },
  {
    path: '/wish',
    component: WishlistView,
    meta: {
      navTitle: '心愿单',
      navIcon: IconName.Wish,
      showInNav: true,
      order: 4,
      sidebarType: SidebarType.None,
    },
  },
  {
    path: '/option',
    component: OptionAccountView,
    meta: {
      navTitle: '配置',
      navIcon: IconName.Random,
      showInNav: false,
      order: 5,
      sidebarType: SidebarType.None,
    },
  },
]
