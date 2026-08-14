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

export function renderNodes (sourceToken:Token[]){
  const nodes: any[] = []
  const allTokens = sourceToken

  for (let i = 0; i < allTokens.length; i++) {
    const token = allTokens[i]
    console.log(i, token.type, token.tag, token.level, token.content)

    if (token.type === 'fence') {
      nodes.push({
        type: 'component',
        component: token.info === 'mermaid' ? 'MermaidPreview' : 'CodeBlock',
        content: token.content,
        info: token.info,
      })
    } else if (token.type === 'blockquote_open') {
      const quoteGroup = []
      let j = i
      while (j < allTokens.length && allTokens[j].type !== 'blockquote_close') {
        quoteGroup.push(allTokens[j])
        j++
      }
      quoteGroup.push(allTokens[j])
      nodes.push({ type: 'blockquote', tokens: quoteGroup })
      i = j
    } else if (token.type === 'table_open') {
      const tableGroup = []
      while (i < allTokens.length - 1 && allTokens[i].type !== 'table_close') {
        tableGroup.push(allTokens[i++])
      }
      tableGroup.push(allTokens[i])
      nodes.push({ type: 'table', tokens: tableGroup })
    } else {
      
      const html = renderTokens(token)
      nodes.push({ type: 'html', content: html })
    }
  }
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