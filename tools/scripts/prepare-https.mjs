import { spawnSync } from 'node:child_process'
import fs from 'node:fs'
import path from 'node:path'
import { fileURLToPath } from 'node:url'

const scriptDir = path.dirname(fileURLToPath(import.meta.url))
const monorepoRoot = path.resolve(scriptDir, '../..')
const certificateDir = path.join(monorepoRoot, '.https')
const certificatePath = path.join(certificateDir, 'localhost.pfx')

// This password protects only a local development certificate. Never use this
// certificate or password in a production deployment.
const certificatePassword = 'aqlife-local-dev-only'

function runDotnet(args, { allowFailure = false } = {}) {
  const result = spawnSync('dotnet', args, {
    cwd: monorepoRoot,
    stdio: 'inherit',
    shell: process.platform === 'win32',
  })

  if (result.error) {
    throw new Error(`无法执行 dotnet ${args.join(' ')}：${result.error.message}`)
  }

  if (result.status !== 0 && !allowFailure) {
    throw new Error(`命令失败：dotnet ${args.join(' ')}`)
  }

  return result.status === 0
}

fs.mkdirSync(certificateDir, { recursive: true })

console.log('[AqLife] 准备本地 HTTPS 开发证书...')

// Trust is supported by Windows/macOS. On Linux the certificate can still be
// exported, but the developer may need to trust it manually in their browser.
const trusted = runDotnet(['dev-certs', 'https', '--trust'], { allowFailure: true })
if (!trusted) {
  console.warn(
    '[AqLife] 自动信任证书失败或当前平台不支持自动信任。证书仍会导出；请按 README 中的说明手动信任。',
  )
}

runDotnet([
  'dev-certs',
  'https',
  '--export-path',
  certificatePath,
  '--password',
  certificatePassword,
  '--format',
  'Pfx',
])

console.log(`[AqLife] HTTPS 证书已准备：${path.relative(monorepoRoot, certificatePath)}`)
