import { mdRenderOption } from "./render"
import { type TocItem, type HeadingLevel, type TocNode } from "./type"


function isHeadingLevel(value: number): value is HeadingLevel {
  return value > 1 && value <= 6
}

const extractToc = (markdown: string): TocItem[] => {
  const tokens = mdRenderOption.parse(markdown, {})
  const result: TocItem[] = []

  tokens.forEach((token, index) => {
   
    if (token.type !== 'heading_open') {
      return
    }

    const titleToken = tokens[index + 1]

    if (!titleToken) {
      return
    }

    const level = Number(token.tag.substring(1))

    if (!isHeadingLevel(level)) {
      return
    }

    result.push({
      level,
      title: titleToken.content,
      anchor: token.attrGet('id') ?? '',
    })
  })

  return result
}

export function resolveImageUid(href: string): string | null {
  const value = href.trim()

  // 新格式：Markdown 图片地址直接是 UUID
  const uuidPattern =
    /^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$/

  if (uuidPattern.test(value)) {
    return value
  }

  // 旧格式：绝对 URL
  try {
    return extractFileUid(new URL(value))
  } catch {
    return null
  }
}

export function extractFileUid(url: URL): string | null {
  // Query parameter names may differ in casing across existing preview links.
  const queryUid = url.searchParams.get('UID')
    ?? url.searchParams.get('uid')
    ?? url.searchParams.get('Uid')

  if (queryUid) {
    return queryUid
  }

  const match = url.pathname.match(
    /([0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12})\/?$/i
  )

  return match?.[1] ?? null
}


export const buildTocTree = (items: TocItem[]): TocNode[] => {
  const result: TocNode[] = []
  const stack: TocNode[] = []

  for (const item of items) {
    const node: TocNode = {
      ...item,
      children: [],
    }

    while (
      stack.length > 0 &&
      stack[stack.length - 1].level >= node.level
    ) {
      stack.pop()
    }

    if (stack.length === 0) {
      result.push(node)
    } else {
      stack[stack.length - 1].children.push(node)
    }

    stack.push(node)
  }

  return result
}

export const extractTocTree = (markdown: string): TocNode[] => {
  const items = extractToc(markdown)

  return buildTocTree(items)
}