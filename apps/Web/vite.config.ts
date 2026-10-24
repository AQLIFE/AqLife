import path from 'node:path'
import { fileURLToPath } from 'node:url'
import { defineConfig, mergeConfig } from 'vite'
import vueDevTools from 'vite-plugin-vue-devtools'
import { createAqlifeViteConfig } from '../../config/vite/base.ts'
import { getAqlifeHttpsOptions } from '../../config/vite/https.ts'

const appRoot = path.dirname(fileURLToPath(import.meta.url))

export default defineConfig(({ command }) =>
  mergeConfig(
    createAqlifeViteConfig(appRoot),
    {
      plugins: [vueDevTools()],
      ...(command === 'serve'
        ? {
            server: {
              https: getAqlifeHttpsOptions(),
              allowedHosts: ['.trycloudflare.com'],
            },
          }
        : {}),
    },
  ),
)
