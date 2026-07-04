import { computed, type Component } from 'vue'; // 引入 Component 类型
import { useRouter } from 'vue-router';
import { type NavItemDto} from '@/types/navigation'

// 1. 定义图标模块的类型契约，消除 "Unexpected any" [cite: 173, 191]
interface IconModule {
    default: Component;
}

const icons = import.meta.glob<IconModule>('@/assets/icons/*.vue', { eager: true });

const iconMap: Record<string, Component> = {};

for (const path in icons) {
    const name = path.split('/').pop()?.replace('.vue', '');
    if (name) {
        iconMap[name] = icons[path].default;
    }
}

export function useNavigation() {
    const router = useRouter();

    const navList = computed<NavItemDto[]>(() => {
        return router.getRoutes()
            .filter(r => r.meta?.showInNav) // 只选需要显示的
            .map(r => ({
                title: r.meta.navTitle as string,
                icon: iconMap[r.meta.navIcon as string], // 动态匹配图标
                path: r.path,
                order: (r.meta.order as number) || 0
            }))
            .sort((a, b) => a.order - b.order);
    });

    return { navList };
}