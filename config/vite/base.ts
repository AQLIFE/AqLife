import path from 'node:path'
import { fileURLToPath } from 'node:url'
import vue from '@vitejs/plugin-vue'
import Icons from 'unplugin-icons/vite'
import { FileSystemIconLoader } from 'unplugin-icons/loaders'
import type { PluginOption, UserConfig } from 'vite'

const configDir = path.dirname(fileURLToPath(import.meta.url))
const monorepoRoot = path.resolve(configDir, '../..')
const iconsDir = path.join(monorepoRoot, 'packages/icons/icons')
const apiContractSrcDir = path.join(monorepoRoot, 'packages/api-contract/src')
const apiClientSrcDir = path.join(monorepoRoot, 'packages/api-client/src')
const domainSrcDir = path.join(monorepoRoot, 'packages/domain/src')
const iconsSrcDir = path.join(monorepoRoot, 'packages/icons/src')

const uiSharedSrcDir = path.join(monorepoRoot, 'packages/ui-shared/src')
// const uiSharedSrcDir = path.join(uiSharedDir, 'src')

export function createAqlifeViteConfig(appRoot: string, options?: { port?: number }): UserConfig {
  const appSrc = path.join(appRoot, 'src')

  return {
    plugins: [
      vue(),
      Icons({
        compiler: 'vue3',
        autoInstall: true,
        customCollections: {
          aqlife: FileSystemIconLoader(iconsDir, (svg) =>
            svg.replace(/^<svg /, '<svg fill="currentColor" '),
          ),
        },
      }) as PluginOption,
    ],
  resolve: {
    alias: [
      { find: '@/api/generated', replacement: apiContractSrcDir },
      { find: '@/api', replacement: apiContractSrcDir },
      { find: '@aqlife/api-contract', replacement: path.join(apiContractSrcDir, 'index.ts') },
      { find: '@aqlife/api-client', replacement: path.join(apiClientSrcDir, 'index.ts') },
      { find: '@aqlife/domain', replacement: path.join(domainSrcDir, 'index.ts') },
      { find: '@aqlife/icons', replacement: path.join(iconsSrcDir, 'index.ts') },
      { find: '@aqlife/ui-shared', replacement: path.join(uiSharedSrcDir, 'index.ts') },
      { find: '@', replacement: appSrc },

    ],
  },
    ...(options?.port
      ? {
          server: {
            host: '0.0.0.0',
            port: options.port,
          },
        }
      : {}),
  }
}

export { monorepoRoot, apiContractSrcDir, apiClientSrcDir, domainSrcDir, iconsDir, iconsSrcDir,uiSharedSrcDir  }
