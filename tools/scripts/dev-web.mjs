import { paths, workspaces } from './lib/paths.mjs'
import { npmRun } from './lib/exec.mjs'

const script = process.argv.includes('--https') ? 'dev:https' : 'dev'

npmRun(script, {
  workspace: workspaces.web,
  cwd: paths.root,
  label: `${script} @aqlife/web`,
})
