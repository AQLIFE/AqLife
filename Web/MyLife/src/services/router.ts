import {createWebHistory,createRouter} from 'vue-router'

import HomeView from '@/views/HomeView.vue'
import ErrorView from '@/views/ErrorView.vue'

export const router = createRouter({
    history:createWebHistory(),
    routes:[
        {path:'/',redirect:'/home'},
        {path:'/home',component:HomeView},
        {path:'/:pathMatch(.*)*',component:ErrorView},

    ]
})

