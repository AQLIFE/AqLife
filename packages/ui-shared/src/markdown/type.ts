import { type FileDto } from "@aqlife/api-contract"

export interface MarkdownFileResolver {
  getFile(uid: string): Promise<FileDto | undefined>
  getVideoUrl?(uid: string): Promise<string | undefined>
}

export interface MarkdownRenderContext {
  fileResolver?: MarkdownFileResolver
  baseUrl?: string
}

export interface MarkdownRenderProps {
  markdown: string
  baseurl?: string
}