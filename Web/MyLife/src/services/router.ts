import {createWebHistory,createRouter} from 'vue-router'

import HomeView from '@/views/HomeView.vue'
import AboutView from '@/views/AboutView.vue'
import ErrorView from '@/views/ErrorView.vue'
import ShareView from '@/views/ShareView.vue'
import BlogView from '@/views/BlogView.vue'
import PrebuiltView from '@/views/PrebuiltView.vue'
import PlanView from '@/views/PlanView.vue'
// 疑似包依赖存在冲突,导致无法正确解析类型
// - 修改冲突文件命令
// - 手动指定类型
// - 使用Vue-选项式API

const routerRaw=[
        {path:'/',redirect:'/home'},
        {path:'/home',component:HomeView},
        {path:'/about',component:AboutView},
        {path:'/share',component:ShareView},
        {path:'/blog',component:BlogView},
        {path:'/plan',component:PlanView},
        {path:'/prebuilt',component:PrebuiltView},

        {path:'/:pathMatch(.*)*',component:ErrorView},

    ]

const router = createRouter({
    history:createWebHistory(),
    routes:routerRaw
})

export {router,routerRaw};
