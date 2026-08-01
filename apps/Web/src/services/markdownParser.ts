import type Token from 'markdown-it/lib/token.mjs'


export interface TocNode {
    level: number
    title: string
    anchor: string
}

export const extractToc = (tokens: Token[]) => {
    const result: TocNode[] = []
    tokens.forEach((item, index) => {
        if (item.type === "heading_open")
            result.push({
                level: Number(item.tag.substring(1)),
                title: tokens[index + 1].content,
                anchor: tokens[index + 1].content
            })

    })
    return result
}