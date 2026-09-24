import {type PaginatedResult} from '@aqlife/domain'
import { ref,type Ref } from 'vue'

export function usePaginatedList<T>(
    fetcher: (page: number, pageSize: number) => Promise<PaginatedResult<T>>,
    options: { pageSize?: number } = {}
) {
    const items = ref<T[]>([]) as Ref<T[]>
    const page = ref(1)
    const pageSize = ref(options.pageSize ?? 20)
    const hasMore = ref(true)
    const loading = ref(false)
    const error = ref<unknown>(null)

    async function load(reset = false) {
        if (loading.value) return
        if (!reset && !hasMore.value) return

        loading.value = true
        error.value = null

        try {
            if (reset) {
                page.value = 1
                items.value = []
                hasMore.value = true
            }

            const result = await fetcher(page.value, pageSize.value)
            items.value = reset ? result.items : [...items.value, ...result.items]
            hasMore.value = result.hasMore
            page.value = result.page + 1
        } catch (e) {
            error.value = e
        } finally {
            loading.value = false
        }
    }

    function refresh() {
        return load(true)
    }

    function loadMore() {
        return load(false)
    }

    return {
        items,
        page,
        pageSize,
        hasMore,
        loading,
        error,
        refresh,
        loadMore,
    }
}