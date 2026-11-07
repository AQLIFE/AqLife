import fs from 'node:fs'
import path from 'node:path'
import { paths } from './lib/paths.mjs'
import { run } from './lib/exec.mjs'

const useHttps = process.argv.includes('--https')
const env = {
  ASPNETCORE_ENVIRONMENT: 'Development',
  ASPNETCORE_URLS: useHttps ? 'https://0.0.0.0:7110' : 'http://0.0.0.0:5110',
}

if (useHttps) {
  const certificatePath = path.join(paths.root, '.https', 'server.pfx')
  if (!fs.existsSync(certificatePath)) {
    throw new Error(
      'HTTPS 证书不存在：.https/server.pfx。请先按照 README「HTTPS 开发证书」章节生成证书，并配置 Kestrel User Secrets。',
    )
  }

  env.ASPNETCORE_Kestrel__Certificates__Default__Path = certificatePath
  console.log('[AqLife] 使用 HTTPS 启动 API...')
} else {
  console.log('[AqLife] 使用 HTTP 启动 API（默认模式）...')
}

run('dotnet', ['run', '--project', paths.apiWebProject, '--no-launch-profile'], {
  cwd: paths.root,
  env,
  label: useHttps ? 'dotnet run API/Web (HTTPS)' : 'dotnet run API/Web (HTTP)',
})
