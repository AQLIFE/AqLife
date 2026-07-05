import { spawnSync } from 'node:child_process'

const WINDOWS_CMDS = new Set(['npm', 'npx', 'dotnet'])

function spawnCommand(command, args, options) {
  const spawnOptions = {
    cwd: options.cwd,
    env: { ...process.env, ...options.env },
    stdio: 'inherit',
  }

  if (process.platform === 'win32' && WINDOWS_CMDS.has(command)) {
    return spawnSync([command, ...args].join(' '), [], { ...spawnOptions, shell: true })
  }

  return spawnSync(command, args, { ...spawnOptions, shell: options.shell ?? false })
}

/**
 * 同步执行命令；非零退出码时终止进程。
 */
export function run(command, args = [], options = {}) {
  const label = options.label ?? `${command} ${args.join(' ')}`.trim()
  console.log(`\n[AqLife] ${label}`)

  const result = spawnCommand(command, args, options)

  if (result.error) {
    console.error(`[AqLife] 命令执行失败: ${result.error.message}`)
    process.exit(1)
  }

  if (result.status !== 0) {
    process.exit(result.status ?? 1)
  }

  return result
}

export function npmRun(script, { workspace, cwd, label } = {}) {
  const args = ['run', script]
  if (workspace) {
    args.push(`--workspace=${workspace}`)
  }
  run('npm', args, { cwd, label: label ?? `npm ${args.join(' ')}` })
}
