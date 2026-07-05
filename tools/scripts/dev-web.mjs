import { paths, workspaces } from './lib/paths.mjs'
import { npmRun } from './lib/exec.mjs'

npmRun('dev', {
  workspace: workspaces.web,
  cwd: paths.root,
  label: 'dev @aqlife/web',
})
