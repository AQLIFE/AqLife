import { fileURLToPath, URL } from 'node:url'
import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import vueDevTools from 'vite-plugin-vue-devtools'
import Icons from 'unplugin-icons/vite'
import { FileSystemIconLoader } from 'unplugin-icons/loaders'

// https://vite.dev/config/
export default defineConfig({
  plugins: [
    vue(),
    vueDevTools(),
    Icons({
      compiler: 'vue3', // 指定编译器为 Vue 3
      autoInstall: true,
      customCollections: {
        // 定义一个名为 'aqlife' 的集合，指向你的共享资产目录
        'aqlife': FileSystemIconLoader('../../packages/ui-shared/icons', (svg) =>
          // 可以在此处对 SVG 进行预处理，例如自动移除 fill 属性以便 CSS 控制颜色
          svg.replace(/^<svg /, '<svg fill="currentColor" ')
        ),
      },
    }),
  ],
  server:{
    host:'0.0.0.0',
    port:5200
  },
  resolve: {
    alias: {
      '@': fileURLToPath(new URL('./src', import.meta.url)),
    },
  },
})
