/* tslint:disable */
/* eslint-disable */

/**
 * - 0 = Blog
 * - 1 = Favorite
 */
export const CategoryQueryType = {
    Blog: 0,
    Favorite: 1,
} as const;

export type CategoryQueryType = typeof CategoryQueryType[keyof typeof CategoryQueryType];

export function instanceOfCategoryQueryType(value: any): boolean {
    return value === CategoryQueryType.Blog || value === CategoryQueryType.Favorite;
}

export function CategoryQueryTypeFromJSON(json: any): CategoryQueryType {
    return json as CategoryQueryType;
}

export function CategoryQueryTypeToJSON(value?: CategoryQueryType | null): any {
    return value as any;
}
