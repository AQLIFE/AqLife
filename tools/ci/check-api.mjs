import { paths } from '../scripts/lib/paths.mjs'
import { run } from '../scripts/lib/exec.mjs'

console.log('[AqLife CI] 后端编译检查')

run('dotnet', ['restore', paths.apiWebProject], {
  cwd: paths.root,
  label: 'dotnet restore API/Web',
})

run('dotnet', ['build', paths.apiWebProject, '--no-restore', '-c', 'Release'], {
  cwd: paths.root,
  label: 'dotnet build API/Web (Release)',
})

console.log('\n[AqLife CI] ✅ 后端检查通过')
