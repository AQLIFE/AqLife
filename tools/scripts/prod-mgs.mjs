import { paths, workspaces } from './lib/paths.mjs'
import { npmRun } from './lib/exec.mjs'

process.env.APP_ENV = 'prod'

npmRun('prod', {
  workspace: workspaces.mgs,
  cwd: paths.root,
  label: 'vite mgs (prod)',
})
