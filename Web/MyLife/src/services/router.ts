import { createWebHistory, createRouter } from 'vue-router'

import HomeView from '@/views/HomeView.vue'
import AboutView from '@/views/AboutView.vue'
import ErrorView from '@/views/ErrorView.vue'
import ShareView from '@/views/ShareView.vue'
import BlogView from '@/views/BlogView.vue'
import PrebuiltView from '@/views/PrebuiltView.vue'
import SkillTreeView from '@/views/SkillTreeView.vue'
import PlanView from '@/views/PlanView.vue'

import { Blog, DevPlan } from './storer'
import WishlistView from '@/views/WishlistView.vue'

// 疑似包依赖存在冲突,导致无法正确解析类型
// - 修改冲突文件命令
// - 手动指定类型
// - 使用Vue-选项式API

const routerRaw = [
    { path: '/', redirect: '/home' },
    { path: '/home', component: HomeView },
    { path: '/about', component: AboutView },
    { path: '/share', component: ShareView },
    { path: '/blog', component: BlogView },
    { path: '/plan', component: PlanView },
    { path: '/skilltree', component:SkillTreeView },
    { path: '/wish', component:WishlistView },
    { path: '/prebuilt', component: PrebuiltView },
    { path: '/:pathMatch(.*)*', component: ErrorView },

]

const router = createRouter({
    history: createWebHistory(),
    routes: routerRaw
})


router.beforeEach((to, from, next) => {
    DevPlan().watchNav(to.path == routerRaw[2].path);
    Blog().watchNav(to.path == routerRaw[4].path)
    next()
})

export { router, routerRaw };
