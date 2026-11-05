import fs from 'node:fs'
import path from 'node:path'
import { monorepoRoot } from './base.ts'

const certificatePath = path.join(monorepoRoot, '.https', 'localhost.pfx')

// Must match the password used by tools/scripts/prepare-https.mjs.
// This password protects a local development certificate only.
const certificatePassword = 'aqlife-local-dev-only'

export function getAqlifeHttpsOptions() {
  if (!fs.existsSync(certificatePath)) {
    throw new Error(
      '找不到 HTTPS 开发证书。请先运行 npm run dev:https --workspace=@aqlife/web 或 @aqlife/mgs，或按照 README 生成 LAN 证书。',
    )
  }

  return {
    pfx: fs.readFileSync(certificatePath),
    passphrase: certificatePassword,
  }
}
