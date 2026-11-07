import fs from 'node:fs'
import path from 'node:path'
import { paths } from './lib/paths.mjs'
import { run } from './lib/exec.mjs'

/**
 * 启动 API。环境由入口命令明确决定，避免依赖终端中遗留的 APP_ENV。
 */
export function runApi(appEnv) {
  if (appEnv !== 'dev' && appEnv !== 'prod') {
    throw new Error(`无效的 APP_ENV="${appEnv}"，仅支持 dev 或 prod。`)
  }

  const useHttps = appEnv === 'prod'
  const env = {
    APP_ENV: appEnv,
    ASPNETCORE_ENVIRONMENT: useHttps ? 'Production' : 'Development',
    ASPNETCORE_URLS: useHttps ? 'https://0.0.0.0:7110' : 'http://0.0.0.0:5110',
  }

  if (useHttps) {
    const certificatePath = path.resolve(
      process.env.ASPNETCORE_Kestrel__Certificates__Default__Path ??
        path.join(paths.root, '.https', 'server.pfx'),
    )
    if (!fs.existsSync(certificatePath)) {
      throw new Error(
        `HTTPS 证书不存在：${certificatePath}。请按照 README 的证书章节生成 PFX，或通过 ASPNETCORE_Kestrel__Certificates__Default__Path 指定证书路径。`,
      )
    }

    env.ASPNETCORE_Kestrel__Certificates__Default__Path = certificatePath
    console.log('[AqLife] APP_ENV=prod：使用 ASP.NET Core Production 环境和 HTTPS 启动 API...')
  } else {
    console.log('[AqLife] APP_ENV=dev：使用 ASP.NET Core Development 环境和 HTTP 启动 API...')
  }

  run('dotnet', ['run', '--project', paths.apiWebProject, '--no-launch-profile'], {
    cwd: paths.root,
    env,
    label: useHttps ? 'dotnet run API/Web (Production + HTTPS)' : 'dotnet run API/Web (Development + HTTP)',
  })
}
