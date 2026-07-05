import { paths } from './lib/paths.mjs'
import { run } from './lib/exec.mjs'

console.log('[AqLife] 安装 Monorepo 依赖...')
run('npm', ['install'], { cwd: paths.root, label: 'npm install (root)' })
console.log('\n[AqLife] ✅ Bootstrap 完成')
