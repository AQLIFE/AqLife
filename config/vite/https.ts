import fs from 'node:fs'
import path from 'node:path'
import { monorepoRoot } from './base.ts'

const certificatePath = path.join(monorepoRoot, '.https', 'server.pem')
const privateKeyPath = path.join(monorepoRoot, '.https', 'server-key.pem')

export function getAqlifeHttpsOptions() {
  if (!fs.existsSync(certificatePath) || !fs.existsSync(privateKeyPath)) {
    throw new Error(
      '找不到 HTTPS 开发证书。请先按照 README「HTTPS 开发证书」章节生成 .https/server.pem 和 .https/server-key.pem。',
    )
  }

  return {
    cert: fs.readFileSync(certificatePath),
    key: fs.readFileSync(privateKeyPath),
  }
}
