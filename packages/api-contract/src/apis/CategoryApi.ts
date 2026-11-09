/* tslint:disable */
/* eslint-disable */
import * as runtime from '../runtime';
import {
    type CategoryDtoPageResult,
    CategoryDtoPageResultFromJSON,
} from '../models/CategoryDtoPageResult';
import type { CategoryQueryType } from '../models/CategoryQueryType';

export interface ApiCategoryGetRequest {
    cid?: string;
    searchType?: CategoryQueryType;
}

export interface CategoryApiInterface {
    apiCategoryGetRequestOpts(requestParameters: ApiCategoryGetRequest): Promise<runtime.RequestOpts>;
    apiCategoryGetRaw(requestParameters: ApiCategoryGetRequest, initOverrides?: RequestInit | runtime.InitOverrideFunction): Promise<runtime.ApiResponse<CategoryDtoPageResult>>;
    apiCategoryGet(requestParameters?: ApiCategoryGetRequest, initOverrides?: RequestInit | runtime.InitOverrideFunction): Promise<CategoryDtoPageResult>;
}

export class CategoryApi extends runtime.BaseAPI implements CategoryApiInterface {
    async apiCategoryGetRequestOpts(requestParameters: ApiCategoryGetRequest = {}): Promise<runtime.RequestOpts> {
        const queryParameters: any = {};

        if (requestParameters.cid != null) {
            queryParameters['CID'] = requestParameters.cid;
        }

        if (requestParameters.searchType != null) {
            queryParameters['SearchType'] = requestParameters.searchType;
        }

        return {
            path: '/api/Category',
            method: 'GET',
            headers: {},
            query: queryParameters,
        };
    }

    async apiCategoryGetRaw(requestParameters: ApiCategoryGetRequest = {}, initOverrides?: RequestInit | runtime.InitOverrideFunction): Promise<runtime.ApiResponse<CategoryDtoPageResult>> {
        const requestOptions = await this.apiCategoryGetRequestOpts(requestParameters);
        const response = await this.request(requestOptions, initOverrides);
        return new runtime.JSONApiResponse(response, jsonValue => CategoryDtoPageResultFromJSON(jsonValue));
    }

    async apiCategoryGet(requestParameters: ApiCategoryGetRequest = {}, initOverrides?: RequestInit | runtime.InitOverrideFunction): Promise<CategoryDtoPageResult> {
        const response = await this.apiCategoryGetRaw(requestParameters, initOverrides);
        return await response.value();
    }
}
