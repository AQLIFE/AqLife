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
        const module = icons[path];
        if (module) {
            iconMap[name] = module.default;
        }
    }
}

export function useNavigation() {
    const router = useRouter();

    const navList = computed<NavItemDto[]>(() => {
        return router.getRoutes()
            .filter(r => r.meta?.showInNav)
            .map(r => {
                const iconName = r.meta.navIcon as string;

                return {
                    title: r.meta.navTitle as string,
                    // 修复问题二：如果 iconMap[iconName] 找不到，给一个默认图标，或者进行强行断言
                    // 方案 A（推荐）：提供一个保底图标或空字符串（取决于你的 NavItemDto 满不满足）
                    icon: iconMap[iconName] || '',

                    // 方案 B：如果你确定路由里的图标名绝对不会写错，可以直接用 as 断言它一定有值
                    // icon: iconMap[iconName] as Component,

                    path: r.path,
                    order: (r.meta.order as number) || 0
                };
            })
            .sort((a, b) => a.order - b.order);
    });

    return { navList };
}
