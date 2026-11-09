import ErrorView from '@/views/ErrorView.vue'
import LoginView from '@/views/LoginView.vue'
import { MgsIconName, mgsIconRegistry } from '@aqlife/icons'
import { User, Document, PriceTag, List, Collection, ChatLineRound, Folder } from '@element-plus/icons-vue'
import HomeView from '@/views/HomeView.vue'
import AccountView from '@/views/AccountView.vue'
import ProfileView from '@/views/ProfileView.vue'
import SubscriptionsView from '@/views/SubscriptionsView.vue'
import TagView from '@/views/TagView.vue'
import CategoryView from '@/views/CategoryView.vue'
import TodoView from '@/views/TodoView.vue'
import CorpusView from '@/views/CorpusView.vue'
import DevelopmentPlanView from '@/views/DevelopmentPlanView.vue'
import { SidebarType } from '@/types/sidebarType'
import RegisterView from '@/views/RegisterView.vue'

import BlogView from '@/views/BlogView.vue'
import BlogTemplateView from '@/views/BlogTemplateView.vue'
import FileUploadView from '@/views/FileUploadView.vue'
import FileDataView from '@/views/FileDataView.vue'
import FavoriteView from '@/views/FavoriteView.vue'

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
    meta: { navTitle: 'Overview', navIcon: MgsIconName.Home, showInNav: true, order: 1, navSection: 'WORKSPACE', sidebarType: SidebarType.Home },
  },
  {
    path: '/account',
    redirect: '/account/profile',
    component: AccountView,
    meta: { navTitle: 'Account', navIcon: User, showInNav: false, order: 4, sidebarType: SidebarType.None },
    children: [
      { path: 'profile', component: ProfileView },
      { path: 'subscriptions', component: SubscriptionsView },
    ],
  },
  {
    path: '/file',
    redirect: '/file/list',
    meta: { navTitle: 'File', navIcon: Document, showInNav: true, order: 2, navSection: 'CONTENT', sidebarType: SidebarType.Data },
    children: [
      { path: 'list', component: FileDataView },
      { path: 'upload', component: FileUploadView },
      { path: 'new', component: BlogTemplateView },
      { path: 'template', redirect: '/file/new' },
      { path: ':id/edit', component: BlogView, props: true },
      { path: ':id', component: BlogView, props: true },
    ],
  },
  {
    path: '/favorite',
    component: FavoriteView,
    meta: { navTitle: 'Favorite', navIcon: Collection, showInNav: true, order: 3, navSection: 'CONTENT', sidebarType: SidebarType.None },
  },
  {
    path: '/tag',
    component: TagView,
    meta: { navTitle: 'Tags', navIcon: PriceTag, showInNav: true, order: 4, navSection: 'SYSTEM', sidebarType: SidebarType.None },
  },
  {
    path: '/category',
    component: CategoryView,
    meta: { navTitle: 'Categories', navIcon: Folder, showInNav: true, order: 5, navSection: 'SYSTEM', sidebarType: SidebarType.None },
  },
  {
    path: '/todo',
    component: TodoView,
    meta: { navTitle: 'Todo', navIcon: mgsIconRegistry[MgsIconName.Plan], showInNav: true, order: 5, navSection: 'CONTENT', sidebarType: SidebarType.None },
  },
  {
    path: '/development-plan',
    component: DevelopmentPlanView,
    meta: { navTitle: 'Plan', navIcon: mgsIconRegistry[MgsIconName.Architecture], showInNav: true, order: 6, navSection: 'CONTENT', sidebarType: SidebarType.None },
  },
  {
    path: '/corpus',
    component: CorpusView,
    meta: { navTitle: '语录', navIcon: ChatLineRound, showInNav: true, order: 7, navSection: 'CONTENT', sidebarType: SidebarType.None },
  },
]
