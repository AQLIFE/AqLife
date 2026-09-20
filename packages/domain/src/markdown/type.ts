export type HeadingLevel = 1 | 2 | 3 | 4 | 5 | 6

export interface TocNode {
  level: HeadingLevel
  title: string
  anchor: string
}