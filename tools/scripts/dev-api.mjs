import path from 'node:path'
import { paths } from './lib/paths.mjs'
import { run } from './lib/exec.mjs'

console.log('[AqLife] 准备 HTTPS 开发证书...')
run('node', [path.join(paths.root, 'tools/scripts/prepare-https.mjs')], {
  cwd: paths.root,
  label: 'prepare local HTTPS certificate',
})

console.log('[AqLife] 启动 API (HTTPS + HTTP fallback)...')
run('dotnet', ['run', '--project', paths.apiWebProject], {
  cwd: paths.root,
  label: 'dotnet run API/Web',
})
