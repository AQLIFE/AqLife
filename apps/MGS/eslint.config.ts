import type { Linter } from 'eslint'
import base from '../../config/eslint/base.eslint.config'
import pluginOxlint from 'eslint-plugin-oxlint'

const config: Linter.Config[] = [
  ...(Array.isArray(base) ? base : [base]),
  ...pluginOxlint.buildFromOxlintConfigFile('.oxlintrc.json'),
]

export default config
