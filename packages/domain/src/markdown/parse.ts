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

export function extractFileUid(url: URL): string | null {
  const queryUid = url.searchParams.get('UID')

  if (queryUid) {
    return queryUid
  }

  const match = url.pathname.match(
    /\/api\/file\/preview\/([0-9a-f-]{36})$/i
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