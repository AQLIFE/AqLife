import { paths } from './lib/paths.mjs'
import { run } from './lib/exec.mjs'

console.log('[AqLife] 启动 API (dotnet run)...')
run('dotnet', ['run', '--project', paths.apiWebProject], {
  cwd: paths.root,
  label: 'dotnet run API/Web',
})
