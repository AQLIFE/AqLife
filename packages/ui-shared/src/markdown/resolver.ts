export interface MarkdownResourceResolver {
  resolve(
    reference: MarkdownResourceReference,
  ): Promise<MarkdownResolvedResource | undefined>
}

export interface MarkdownResourceReference {
  type: 'file'
  uid: string
}
export interface MarkdownResolvedResource {
  uid: string
  name?: string
  url: string
  mimeType?: string
  extension?: string
}