import path from 'node:path'
import { fileURLToPath } from 'node:url'
import { defineConfig, mergeConfig } from 'vite'
import vueDevTools from 'vite-plugin-vue-devtools'
import { createAqlifeViteConfig, getAqlifeAppEnvironment } from '../../config/vite/base.ts'
import { getAqlifeHttpsOptions } from '../../config/vite/https.ts'

const appRoot = path.dirname(fileURLToPath(import.meta.url))

export default defineConfig(() => {
  const useHttps = getAqlifeAppEnvironment() === 'prod'
  return mergeConfig(createAqlifeViteConfig(appRoot, { port: 5173 }), {
    plugins: [vueDevTools()],
    server: {
      ...(useHttps ? { https: getAqlifeHttpsOptions() } : {}),
      allowedHosts: ['.trycloudflare.com'],
    },
  })
})
