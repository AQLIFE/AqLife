import {createWebHistory,createRouter} from 'vue-router'

import HomeView from '@/views/HomeView.vue'
import InfoView from '@/views/InfoView.vue'
import ErrorView from '@/views/ErrorView.vue'
// 疑似包依赖存在冲突,导致无法正确解析类型
// - 修改冲突文件命令
// - 手动指定类型
// - 使用Vue-选项式API


export const router = createRouter({
    history:createWebHistory(),
    routes:[
        {path:'/',redirect:'/home'},
        {path:'/home',component:HomeView},
        {path:'/info',component:InfoView},
        {path:'/:pathMatch(.*)*',component:ErrorView},

    ]
})

