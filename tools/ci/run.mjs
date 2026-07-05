import path from 'node:path'
import { fileURLToPath } from 'node:url'
import { run } from '../scripts/lib/exec.mjs'

const ciDir = path.dirname(fileURLToPath(import.meta.url))
const steps = process.argv.slice(2)
const onlyFrontend = steps.includes('--frontend')
const onlyApi = steps.includes('--api')

console.log('[AqLife CI] 开始 Monorepo 检查\n')

if (!onlyApi) {
  run('node', [path.join(ciDir, 'check-frontend.mjs')])
}

if (!onlyFrontend) {
  run('node', [path.join(ciDir, 'check-api.mjs')])
}

console.log('\n[AqLife CI] ✅ 全部检查通过')
