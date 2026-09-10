import path from 'node:path'
import { fileURLToPath } from 'node:url'
import { defineConfig, mergeConfig } from 'vite'
import vueDevTools from 'vite-plugin-vue-devtools'
import { createAqlifeViteConfig } from '../../config/vite/base.ts'

const appRoot = path.dirname(fileURLToPath(import.meta.url))

export default mergeConfig(
  createAqlifeViteConfig(appRoot, { port: 5200 }),
  defineConfig({
    plugins: [vueDevTools()],
  }),
)
