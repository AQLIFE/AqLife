import { computed, reactive } from 'vue'

// Requests sharing a scope use a counter, so one completion cannot hide another's overlay.
const pendingByScope = reactive(new Map<string, number>())

export function useLoadingTask(scope: string) {
  if (!scope.trim()) throw new Error('Loading scope must not be empty')
  const isLoading = computed(() => (pendingByScope.get(scope) ?? 0) > 0)
  const pendingCount = computed(() => pendingByScope.get(scope) ?? 0)

  function begin() {
    pendingByScope.set(scope, (pendingByScope.get(scope) ?? 0) + 1)
    let finished = false
    return () => {
      if (finished) return
      finished = true
      const next = Math.max(0, (pendingByScope.get(scope) ?? 1) - 1)
      if (next === 0) pendingByScope.delete(scope)
      else pendingByScope.set(scope, next)
    }
  }

  async function run<T>(operation: () => Promise<T> | T): Promise<T> {
    const finish = begin()
    try { return await operation() } finally { finish() }
  }

  return { isLoading, pendingCount, begin, run }
}
