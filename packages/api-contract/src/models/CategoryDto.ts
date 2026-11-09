/* tslint:disable */
/* eslint-disable */

export interface CategoryDto {
    uid?: string;
    categoryName?: string | null;
    categoryDescription?: string | null;
    categoryCount?: number;
    parentName?: string | null;
}

export function instanceOfCategoryDto(value: object): value is CategoryDto {
    return true;
}

export function CategoryDtoFromJSON(json: any): CategoryDto {
    if (json == null) return json;
    return {
        uid: json['uid'] == null ? undefined : json['uid'],
        categoryName: json['categoryName'] == null ? undefined : json['categoryName'],
        categoryDescription: json['categoryDescription'] == null ? undefined : json['categoryDescription'],
        categoryCount: json['categoryCount'] == null ? undefined : json['categoryCount'],
        parentName: json['parentName'] == null ? undefined : json['parentName'],
    };
}

export function CategoryDtoToJSON(value?: CategoryDto | null): any {
    if (value == null) return value;
    return {
        uid: value.uid,
        categoryName: value.categoryName,
        categoryDescription: value.categoryDescription,
        categoryCount: value.categoryCount,
        parentName: value.parentName,
    };
}
