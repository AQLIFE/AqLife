import path from 'node:path'
import { fileURLToPath } from 'node:url'

const packageRoot = path.resolve(path.dirname(fileURLToPath(import.meta.url)), '..')

/** unplugin-icons FileSystemIconLoader 使用的 SVG 目录 */
export const iconsDirectory = path.join(packageRoot, 'icons')
