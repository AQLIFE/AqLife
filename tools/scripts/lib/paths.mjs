import path from 'node:path'
import { fileURLToPath } from 'node:url'

const scriptsDir = path.dirname(fileURLToPath(import.meta.url))

/** Monorepo 根目录 (AqLife/) */
export const monorepoRoot = path.resolve(scriptsDir, '../../..')

export const paths = {
  root: monorepoRoot,
  apiWebProject: path.join(monorepoRoot, 'services/API/Web/Web.csproj'),
  openapiGenerator: path.join(monorepoRoot, 'tools/openapi-generator'),
  apps: {
    web: path.join(monorepoRoot, 'apps/Web'),
    mgs: path.join(monorepoRoot, 'apps/MGS'),
  },
  packages: {
    apiContract: path.join(monorepoRoot, 'packages/api-contract'),
    apiClient: path.join(monorepoRoot, 'packages/api-client'),
    domain: path.join(monorepoRoot, 'packages/domain'),
    icons: path.join(monorepoRoot, 'packages/icons'),
  },
}

export const workspaces = {
  web: '@aqlife/web',
  mgs: '@aqlife/mgs',
  openapiGenerator: '@aqlife/openapi-generator',
}
