import MarkdownIt from 'markdown-it'
import anchor from 'markdown-it-anchor'

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
