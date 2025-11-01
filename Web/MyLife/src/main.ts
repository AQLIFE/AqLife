import '@/assets/main.css'

import {  createApp } from 'vue'
import { createPinia } from 'pinia'
import App from './App.vue'
import { router } from '@/services/router'
import ElementPlus from 'element-plus'
import 'element-plus/dist/index.css'

const app = createApp(App)

app.use(createPinia()).use(ElementPlus).use(router)

app.mount('#app')
