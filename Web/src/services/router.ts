import { createWebHistory, createRouter } from 'vue-router'

import HomeView from '@/views/HomeView.vue'
import AboutView from '@/views/AboutView.vue'
import ErrorView from '@/views/ErrorView.vue'
import ShareView from '@/views/ShareView.vue'
import BlogView from '@/views/BlogView.vue'
import PrebuiltView from '@/views/PrebuiltView.vue'
import SkillTreeView from '@/views/SkillTreeView.vue'
import PlanView from '@/views/PlanView.vue'
import WishlistView from '@/views/WishlistView.vue'
import OptionAccountView from '@/views/OptionAccountView.vue'

import 'vue-router';
import { IconName, SidebarType } from '@/types/define';

declare module 'vue-router' {
  interface RouteMeta {
    navTitle: string;
    navIcon: IconName;      // 强制使用枚举，不再接受随意字符串
    showInNav: boolean;
    order?: number;
    sidebarType?: SidebarType; // 强制使用枚举
  }
}


const routerRaw = [
    { path: '/:pathMatch(.*)*', component: ErrorView ,meta: { navTitle: '错误', navIcon: IconName.Error, showInNav: false, order: 0} },
    { path: '/', redirect: '/home' },
    { path: '/home',        component: HomeView,        meta: { navTitle: "首页",   navIcon: IconName.Home,    showInNav: false,   order: 1 ,  sidebarType:SidebarType.None} },
    { path: '/about',       component: AboutView,       meta: { navTitle: '关于',   navIcon: IconName.About,   showInNav: true,    order: 2 ,  sidebarType:SidebarType.About} },
    { path: '/share',       component: ShareView,       meta: { navTitle: '分享',   navIcon: IconName.Share,   showInNav: false,   order: 3 ,  sidebarType:SidebarType.None} },
    { path: '/blog',        component: BlogView,        meta: { navTitle: '建文',   navIcon: IconName.Blog,    showInNav: true,    order: 4 ,  sidebarType:SidebarType.Blog} },
    { path: '/plan',        component: PlanView,        meta: { navTitle: '计划',   navIcon: IconName.Plan,    showInNav: false,   order: 5 ,  sidebarType:SidebarType.None} },
    { path: '/skilltree',   component: SkillTreeView,   meta: { navTitle: '技能',   navIcon: IconName.Skill,   showInNav: false,   order: 6 ,  sidebarType:SidebarType.None} },
    { path: '/wish',        component: WishlistView,    meta: { navTitle: '心愿单', navIcon: IconName.Wish,    showInNav: true,    order: 7 ,  sidebarType:SidebarType.None} },
    { path: '/prebuilt',    component: PrebuiltView,    meta: { navTitle: '预览',   navIcon: IconName.Random,  showInNav: false,   order: 8 ,  sidebarType:SidebarType.None} },
    { path: '/option',    component: OptionAccountView, meta: { navTitle: '配置',   navIcon: IconName.Random,  showInNav: false,   order: 8 ,  sidebarType:SidebarType.None} },
    
]

const router = createRouter({
    history: createWebHistory(),
    routes: routerRaw
})

export { router, routerRaw };