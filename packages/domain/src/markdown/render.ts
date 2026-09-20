import MarkdownIt from 'markdown-it'
import anchor from 'markdown-it-anchor'
import { type Token } from 'markdown-it/index.js'

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
  mdRenderOption.renderer.render([token], mdRenderOption.options, {})


export function buildMarkdownRenderNodes(tokens: Token[]) {
  const nodes: any[] = []
  let htmlBuffer = ''

  function flushHtml() {
    if (!htmlBuffer) return

    nodes.push({
      type: 'html',
      content: htmlBuffer,
    })

    htmlBuffer = ''
  }

  for (let i = 0; i < tokens.length; i++) {
    const token = tokens[i]

    if (token.type === 'fence') {
      flushHtml()

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

    if (token.type === 'blockquote_open') {
      flushHtml()

      const quoteGroup = []
      let j = i

      while (
        j < tokens.length &&
        tokens[j].type !== 'blockquote_close'
      ) {
        quoteGroup.push(tokens[j])
        j++
      }

      quoteGroup.push(tokens[j])

      nodes.push({
        type: 'blockquote',
        tokens: quoteGroup,
      })

      i = j
      continue
    }

    if (token.type === 'table_open') {
      flushHtml()

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

    // 普通 token：不要立刻生成 node
    htmlBuffer += renderTokens(token)
  }

  flushHtml()

  return nodes
}

export function getArticleTitle(content: string): string {
  const tokens = mdRenderOption.parse(content, {})

  for (let i = 0; i < tokens.length; i++) {
    const token = tokens[i]

    if (token.type === 'heading_open' && token.tag === 'h1') {
      const titleToken = tokens[i + 1]

      if (titleToken?.type === 'inline') {
        return sanitizeFileName(titleToken.content)
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