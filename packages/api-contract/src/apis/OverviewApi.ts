/* tslint:disable */
/* eslint-disable */
import * as runtime from '../runtime';
import { OverviewDto, OverviewDtoFromJSON } from '../models/OverviewDto';

export interface ApiOverviewGetRequest {
    recentCount?: number;
}

export interface OverviewApiInterface {
    apiOverviewGet(requestParameters?: ApiOverviewGetRequest, initOverrides?: RequestInit | runtime.InitOverrideFunction): Promise<OverviewDto>;
}

export class OverviewApi extends runtime.BaseAPI implements OverviewApiInterface {
    async apiOverviewGetRequestOpts(requestParameters: ApiOverviewGetRequest = {}): Promise<runtime.RequestOpts> {
        const queryParameters: any = {};

        if (requestParameters['recentCount'] != null) {
            queryParameters['RecentCount'] = requestParameters['recentCount'];
        }

        return {
            path: '/api/Overview',
            method: 'GET',
            headers: {},
            query: queryParameters,
        };
    }

    async apiOverviewGetRaw(requestParameters: ApiOverviewGetRequest = {}, initOverrides?: RequestInit | runtime.InitOverrideFunction): Promise<runtime.ApiResponse<OverviewDto>> {
        const requestOptions = await this.apiOverviewGetRequestOpts(requestParameters);
        const response = await this.request(requestOptions, initOverrides);
        return new runtime.JSONApiResponse(response, (jsonValue) => OverviewDtoFromJSON(jsonValue));
    }

    async apiOverviewGet(requestParameters: ApiOverviewGetRequest = {}, initOverrides?: RequestInit | runtime.InitOverrideFunction): Promise<OverviewDto> {
        const response = await this.apiOverviewGetRaw(requestParameters, initOverrides);
        return await response.value();
    }
}
