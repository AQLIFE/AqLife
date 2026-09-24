import { FileDto } from "@aqlife/api-contract"

export interface MarkdownFileResolver {
  getFile(uid: string): Promise<FileDto | undefined>
}

export interface MarkdownRenderContext {
  fileResolver?: MarkdownFileResolver
  baseUrl?: string
}

export interface MarkdownRenderProps {
  markdown: string
  baseurl?: string
}