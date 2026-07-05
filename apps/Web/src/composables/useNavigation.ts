import { computed } from 'vue'
import { useRouter } from 'vue-router'
import { type NavItemDto } from '@/types/navigation'
import { WebIconName, webIconRegistry } from '@aqlife/icons'

export function useNavigation() {
  const router = useRouter()

  const navList = computed<NavItemDto[]>(() => {
    return router
      .getRoutes()
      .filter((r) => r.meta?.showInNav)
      .map((r) => {
        const iconKey = r.meta.navIcon as WebIconName
        return {
          title: r.meta.navTitle as string,
          icon: webIconRegistry[iconKey],
          path: r.path,
          order: (r.meta.order as number) || 0,
        }
      })
      .sort((a, b) => a.order - b.order)
  })

  return { navList }
}
