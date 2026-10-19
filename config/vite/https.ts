import fs from 'node:fs'
import path from 'node:path'
import { monorepoRoot } from './base'

const certificatePath = path.join(monorepoRoot, '.https', 'localhost.pfx')

// Must match the password used by tools/scripts/prepare-https.mjs.
// This is a local-development certificate only, not a production secret.
const certificatePassword = 'aqlife-local-dev-only'

export function getAqlifeHttpsOptions() {
  if (!fs.existsSync(certificatePath)) {
    throw new Error(
      '找不到本地 HTTPS 证书。请先运行 npm run dev:web 或 npm run dev:mgs，开发脚本会自动准备证书。',
    )
  }

  return {
    pfx: fs.readFileSync(certificatePath),
    passphrase: certificatePassword,
  }
}
