import MarkdownIt from 'markdown-it'
import anchor from 'markdown-it-anchor'
import { type Token } from 'markdown-it/index.js'

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
  permalink: true,
  permalinkSymbol: '#',
  level: 2,
  permalinkBefore: true,
})

const renderTokens = (token: Token) =>
  mdRenderOption.renderer.render(
    [token],
    mdRenderOption.options,
    {},
  )

function getLinkText(tokens: Token[]): string {
  return tokens
    .filter(token => token.type === 'text')
    .map(token => token.content)
    .join('')
}

function getLinkChildren(
  token: Token,
): Token[] {
  return token.children ?? []
}

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
     * inline token
     *
     * 例如：
     *
     * [demo](http://test.net/api/file/preview?UID=xxx)
     *
     * markdown-it 会把真正的 link_open / link_close
     * 放到 token.children 中。
     */
    if (
      token.type === 'inline' &&
      token.children &&
      resolveLink
    ) {
      const children = getLinkChildren(token)

      let hasInternalLink = false
      for (let j = 0; j < children.length; j++) {
        const child = children[j]

        if (child.type !== 'link_open') {
          continue
        }

        const href = child.attrGet('href')
        

        if (!href) {
          continue
        }

        let linkCloseIndex = j + 1

        while (
          linkCloseIndex < children.length &&
          children[linkCloseIndex].type !== 'link_close'
        ) {
          linkCloseIndex++
        }

        const linkChildren = children.slice(
          j + 1,
          linkCloseIndex,
        )

        const text = getLinkText(linkChildren)

        const result = await resolveLink({
          href,
          text,
          token: child,
        })

        if (!result) {
          continue
        }

        hasInternalLink = true

        await flushHtml()

        nodes.push({
          type: 'resolved-link',
          result,
          text,
        })
        break
      }

      if (hasInternalLink) {
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