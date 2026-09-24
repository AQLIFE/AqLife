export interface PaginatedResponse<T> {
    items?: T[]
    page?: number
    pageSize?: number
    hasMore?: boolean
}
export interface PaginatedResult<T> {
    items: T[]
    page: number
    pageSize: number
    hasMore: boolean
}
export function normalizePagination<T>(response: PaginatedResponse<T>): PaginatedResult<T> {
    return {
        items: response.items ?? [],
        page: response.page ?? 1,
        pageSize: response.pageSize ?? 20,
        hasMore: response.hasMore ?? false,
    }
}