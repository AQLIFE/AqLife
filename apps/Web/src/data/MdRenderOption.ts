import MarkdownIt from 'markdown-it'
import anchor from 'markdown-it-anchor'
export const mdRenderOption = new MarkdownIt({
  html: true, // 启用HTML标签
  breaks: true, // 转换换行符为<br>
  linkify: true, // 自动转换URL为链接
  typographer: true, // 启用语言中性的替换
}).use(anchor, {
  permalink: true,
  permalinkSymbol: '#',
  level: 2,
  permalinkBefore: true,
})
