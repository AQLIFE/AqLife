import MarkdownIt from 'markdown-it'
import anchor from 'markdown-it-anchor'
import { type Token } from 'markdown-it/index.js'
import { extractFileUid, resolveImageUid } from './parse'

export interface MarkdownLinkContext {
  href: string
  text: string
  token: Token
}

export type MarkdownLinkResolver = (
  context: MarkdownLinkContext,
) => Promise<MarkdownLinkResult | null>

export type MarkdownLinkResult =
  | {
    type: 'image'
    src: string
    alt: string
  }
  | {
    type: 'markdown'
    href: string
  }
  | {
    type: 'link'
    href: string
  }

export const mdRenderOption = new MarkdownIt({
  html: true,
  breaks: true,
  linkify: true,
  typographer: true,
}).use(anchor, {
  level: 2,
})

const renderTokens = (token: Token) =>
  mdRenderOption.renderer.render(
    [token],
    mdRenderOption.options,
    {},
  )


export async function buildMarkdownRenderNodes(
  tokens: Token[],
  resolveLink?: MarkdownLinkResolver,
): Promise<any[]> {
  const nodes: any[] = []

  let htmlBuffer = ''

  async function flushHtml() {
    if (!htmlBuffer) {
      return
    }

    nodes.push({
      type: 'html',
      content: htmlBuffer,
    })

    htmlBuffer = ''
  }

  for (let i = 0; i < tokens.length; i++) {
    const token = tokens[i]

    /*
     * fenced code
     */
    if (token.type === 'fence') {
      await flushHtml()

      nodes.push({
        type: 'component',
        component: token.info === 'mermaid'
          ? 'MermaidPreview'
          : 'CodeBlock',
        content: token.content,
        info: token.info,
      })

      continue
    }

    /*
     * blockquote
     */
    if (token.type === 'blockquote_open') {
      await flushHtml()

      const quoteGroup = []
      let j = i

      while (
        j < tokens.length &&
        tokens[j].type !== 'blockquote_close'
      ) {
        quoteGroup.push(tokens[j])
        j++
      }

      if (j < tokens.length) {
        quoteGroup.push(tokens[j])
      }

      nodes.push({
        type: 'blockquote',
        tokens: quoteGroup,
      })

      i = j
      continue
    }

    /*
     * table
     */
    if (token.type === 'table_open') {
      await flushHtml()

      const tableGroup = []

      while (
        i < tokens.length - 1 &&
        tokens[i].type !== 'table_close'
      ) {
        tableGroup.push(tokens[i++])
      }

      tableGroup.push(tokens[i])

      nodes.push({
        type: 'table',
        tokens: tableGroup,
      })

      continue
    }


    /*
     * Link
     *
     * 例如：
     *
     * ![demo](http://test.net/api/file/preview?UID=xxx)
     *
     */
    /*
 * Link / image
 */
    if (
      token.type === 'inline' &&
      token.children &&
      resolveLink
    ) {
      const children = token.children
      const segment: Token[] = []
      let hasResolvedImage = false

      // 渲染当前连续的普通 Markdown 子 token
      const flushSegment = async () => {
        if (segment.length === 0) return

        const inlineToken = {
          ...token,
          children: [...segment],
        }

        htmlBuffer += renderTokens(inlineToken)
        segment.length = 0
      }

      for (const child of children) {
        if (child.type !== 'image') {
          segment.push(child)
          continue
        }

        const imageHref = child.attrGet('src')
        if (!imageHref) {
          segment.push(child)
          continue
        }

        let uuid: string | null

        try {
          uuid = resolveImageUid(imageHref)
        } catch {
          // 非法或无法解析的 URL，保留默认 Markdown 渲染
          segment.push(child)
          continue
        }

        if (!uuid) {
          segment.push(child)
          continue
        }

        const imageResult = await resolveLink({
          href: uuid,
          text: child.content,
          token: child,
        })

        if (imageResult?.type !== 'image') {
          // 未被自定义解析器接管，保留默认 Markdown 渲染
          segment.push(child)
          continue
        }

        // 先输出图片前面的普通内容，确保顺序正确
        await flushSegment()
        await flushHtml()

        nodes.push({
          type: 'resolved-link',
          result: imageResult,
          text: child.content,
        })

        hasResolvedImage = true
      }

      if (hasResolvedImage) {
        // 输出最后一张图片之后的普通内容
        await flushSegment()
        continue
      }
    }


    /*
     * 普通 token
     */
    htmlBuffer += renderTokens(token)
  }

  await flushHtml()

  return nodes
}

export function getArticleTitle(
  content: string,
): string {
  const tokens = mdRenderOption.parse(content, {})

  for (let i = 0; i < tokens.length; i++) {
    const token = tokens[i]

    if (
      token.type === 'heading_open' &&
      token.tag === 'h1'
    ) {
      const titleToken = tokens[i + 1]

      if (titleToken?.type === 'inline') {
        return sanitizeFileName(
          titleToken.content,
        )
      }
    }
  }

  return '未命名文章'
}

function sanitizeFileName(name: string): string {
  return name
    .replace(/[<>:"/\\|?*]/g, '')
    .trim()
    .slice(0, 64)
}