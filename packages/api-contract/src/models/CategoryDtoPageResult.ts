/* tslint:disable */
/* eslint-disable */
import type { CategoryDto } from './CategoryDto';
import { CategoryDtoFromJSON, CategoryDtoToJSON } from './CategoryDto';

export interface CategoryDtoPageResult {
    items?: Array<CategoryDto> | null;
    page?: number;
    pageSize?: number;
    hasMore?: boolean;
}

export function instanceOfCategoryDtoPageResult(value: object): value is CategoryDtoPageResult {
    return true;
}

export function CategoryDtoPageResultFromJSON(json: any): CategoryDtoPageResult {
    if (json == null) return json;
    return {
        items: json['items'] == null ? undefined : (json['items'] as Array<any>).map(CategoryDtoFromJSON),
        page: json['page'] == null ? undefined : json['page'],
        pageSize: json['pageSize'] == null ? undefined : json['pageSize'],
        hasMore: json['hasMore'] == null ? undefined : json['hasMore'],
    };
}

export function CategoryDtoPageResultToJSON(value?: CategoryDtoPageResult | null): any {
    if (value == null) return value;
    return {
        items: value.items == null ? undefined : value.items.map(CategoryDtoToJSON),
        page: value.page,
        pageSize: value.pageSize,
        hasMore: value.hasMore,
    };
}
