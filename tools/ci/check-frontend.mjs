import { paths, workspaces } from '../scripts/lib/paths.mjs'
import { npmRun } from '../scripts/lib/exec.mjs'

console.log('[AqLife CI] 前端类型检查')

npmRun('type-check', {
  workspace: workspaces.web,
  cwd: paths.root,
  label: 'type-check @aqlife/web',
})

npmRun('type-check', {
  workspace: workspaces.mgs,
  cwd: paths.root,
  label: 'type-check @aqlife/mgs',
})

console.log('\n[AqLife CI] ✅ 前端检查通过')
