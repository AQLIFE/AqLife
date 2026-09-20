import { mdRenderOption } from "./render"
import { HeadingLevel, TocNode } from "./type"


function createHeadingAnchor(
  title: string,
  usedAnchors: Set<string>,
) {
  const base = title
    .trim()
    .toLowerCase()
    .replace(/\s+/g, '-')

  let anchor = base
  let index = 1

  while (usedAnchors.has(anchor)) {
    anchor = `${base}-${index++}`
  }

  usedAnchors.add(anchor)

  return anchor
}

function isHeadingLevel(value: number): value is HeadingLevel {
  return value >= 1 && value <= 6
}

export const extractToc = (markdown: string): TocNode[] => {
  const tokens = mdRenderOption.parse(markdown, {})
  const result: TocNode[] = []
  const usedAnchors = new Set<string>()

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

    const title = titleToken.content

    result.push({
      level,
      title,
      anchor: createHeadingAnchor(title, usedAnchors),
    })
  })

  return result
}