let bearerToken: string | null = null

export const tokenStore = {
  get(): string | null {
    return bearerToken
  },
  set(token: string | null) {
    bearerToken = token?.trim() ? token.trim() : null
  },
  clear() {
    bearerToken = null
  },
}
