export type HeadingLevel = 1 | 2 | 3 | 4 | 5 | 6

export interface TocItem {
  level: HeadingLevel
  title: string
  anchor: string
}
export interface TocNode extends TocItem{
  children:TocNode[]
}