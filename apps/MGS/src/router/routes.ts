import ErrorView from '@/views/ErrorView.vue'
import LoginView from '@/views/LoginView.vue'
import { MgsIconName } from '@aqlife/icons'
import { User, Document, PriceTag, List } from '@element-plus/icons-vue'
import HomeView from '@/views/HomeView.vue'
import AccountView from '@/views/AccountView.vue'
import ProfileView from '@/views/ProfileView.vue'
import SubscriptionsView from '@/views/SubscriptionsView.vue'
import TagView from '@/views/TagView.vue'
import TodoView from '@/views/TodoView.vue'
import { SidebarType } from '@/types/sidebarType'
import RegisterView from '@/views/RegisterView.vue'

import BlogView from '@/views/BlogView.vue'
import BlogTemplateView from '@/views/BlogTemplateView.vue'
import FileUploadView from '@/views/FileUploadView.vue'
import FileDataView from '@/views/FileDataView.vue'

export const routes = [
  {
    path: '/:pathMatch(.*)*',
    component: ErrorView,
    meta: { navTitle: '错误', navIcon: MgsIconName.Error, showInNav: false, order: 0, sidebarType: SidebarType.None },
  },
  { path: '/', redirect: '/home' },
  {
    path: '/login',
    component: LoginView,
    meta: { navTitle: '登录', navIcon: MgsIconName.Login, showInNav: false, order: 1, sidebarType: SidebarType.Login },
  },
  {
    path: '/register',
    component: RegisterView,
    meta: { navTitle: '注册', navIcon: MgsIconName.Register, showInNav: false, order: 2, sidebarType: SidebarType.Register },
  },
  {
    path: '/home',
    component: HomeView,
    meta: { navTitle: '首页', navIcon: MgsIconName.Home, showInNav: false, order: 3, sidebarType: SidebarType.Home },
  },
  {
    path: '/account',
    redirect: '/account/profile',
    component: AccountView,
    meta: { navTitle: 'Account', navIcon: User, showInNav: true, order: 4, sidebarType: SidebarType.None },
    children: [
      { path: 'profile', component: ProfileView },
      { path: 'subscriptions', component: SubscriptionsView },
    ],
  },
  {
    path: '/file',
    redirect: '/file/list',
    meta: { navTitle: 'File', navIcon: Document, showInNav: true, order: 5, sidebarType: SidebarType.Data },
    children: [
      { path: 'list', component: FileDataView },
      { path: 'upload', component: FileUploadView },
      { path: 'new', component: BlogTemplateView },
      { path: 'template', redirect: '/blog/new' },
      { path: ':id/edit', component: BlogView, props: true },
      { path: ':id', component: BlogView, props: true },
    ],
  },
  {
    path: '/tag',
    component: TagView,
    meta: { navTitle: 'Tag', navIcon: PriceTag, showInNav: true, order: 6, sidebarType: SidebarType.None },
  },
  {
    path: '/todo',
    component: TodoView,
    meta: { navTitle: 'Todo', navIcon: List, showInNav: true, order: 7, sidebarType: SidebarType.None },
  },
]
