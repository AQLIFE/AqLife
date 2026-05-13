import type { Component } from 'vue';

// 定义导航项的“图纸”（DTO） [cite: 181]
export interface NavItemDto {
    title: string;
    icon: Component | string; // 图标可以是组件零件，也可以是字符串名 [cite: 191]
    path: string;
    order: number;
}