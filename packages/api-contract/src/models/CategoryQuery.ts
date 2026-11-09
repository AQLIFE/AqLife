/* tslint:disable */
/* eslint-disable */
import type { CategoryQueryType } from './CategoryQueryType';

export interface CategoryQuery {
    cid?: string;
    searchType?: CategoryQueryType;
}

export function instanceOfCategoryQuery(value: object): value is CategoryQuery {
    return true;
}

export function CategoryQueryFromJSON(json: any): CategoryQuery {
    if (json == null) return json;
    return {
        cid: json['cid'] == null ? undefined : json['cid'],
        searchType: json['searchType'] == null ? undefined : json['searchType'],
    };
}

export function CategoryQueryToJSON(value?: CategoryQuery | null): any {
    if (value == null) return value;
    return {
        cid: value.cid,
        searchType: value.searchType,
    };
}
