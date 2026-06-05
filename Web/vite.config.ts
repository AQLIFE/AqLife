import { fileURLToPath, URL } from 'node:url'

import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import vueJsx from '@vitejs/plugin-vue-jsx'
import vueDevTools from 'vite-plugin-vue-devtools'

// https://vite.dev/config/
export default defineConfig({
  plugins: [
    vue(),
    vueJsx(),
    vueDevTools(),
  ],
  resolve: {
    alias: {
      '@': fileURLToPath(new URL('./src', import.meta.url))
    },
  },server:{
    open:true,   //默认启动项目打开页面
    port:5173,   //端口号
    host:"192.168.0.55", //主机名
    proxy:{
      '/api': {	//
        target: "http://192.168.0.55:5110", // 目标地址
        ws: true,
        secure: false,
        changeOrigin: true,// 是否允许跨域代理
    }
  }
},
})
