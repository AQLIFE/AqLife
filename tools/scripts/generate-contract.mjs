import { spawnSync } from 'node:child_process'
import { paths } from './lib/paths.mjs'

const isWindows = process.platform === 'win32'
const scriptName = isWindows ? 'generate.ps1' : 'generate.sh'
const scriptPath = `${paths.openapiGenerator}/${scriptName}`

console.log('[AqLife] 从运行中的 API 同步 OpenAPI 契约...')
console.log('[AqLife] 请确保 API 已在 http://localhost:5110 启动')

if (isWindows) {
  const result = spawnSync(
    'powershell',
    ['-NoProfile', '-ExecutionPolicy', 'Bypass', '-File', scriptPath],
    { cwd: paths.openapiGenerator, stdio: 'inherit' },
  )
  process.exit(result.status ?? 1)
}

const result = spawnSync('bash', [scriptPath], {
  cwd: paths.openapiGenerator,
  stdio: 'inherit',
})
process.exit(result.status ?? 1)
