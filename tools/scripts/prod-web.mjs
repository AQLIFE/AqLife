import { paths, workspaces } from './lib/paths.mjs'
import { npmRun } from './lib/exec.mjs'

process.env.APP_ENV = 'prod'

npmRun('prod', {
  workspace: workspaces.web,
  cwd: paths.root,
  label: 'vite web (prod)',
})
