import { paths, workspaces } from './lib/paths.mjs'
import { npmRun } from './lib/exec.mjs'

const script = process.argv.includes('--https') ? 'dev:https' : 'dev'

npmRun(script, {
  workspace: workspaces.mgs,
  cwd: paths.root,
  label: `${script} @aqlife/mgs`,
})
