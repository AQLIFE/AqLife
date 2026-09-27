/* tslint:disable */
/* eslint-disable */
import {
    FileDto,
    FileDtoFromJSON,
    FileDtoToJSON,
} from './FileDto';

export interface OverviewDto {
    draftCount?: number;
    scheduledCount?: number;
    publishedCount?: number;
    recentFiles?: Array<FileDto>;
}

export function instanceOfOverviewDto(value: object): value is OverviewDto {
    return true;
}

export function OverviewDtoFromJSON(json: any): OverviewDto {
    if (json == null) return json;
    return {
        'draftCount': json['draftCount'] == null ? undefined : json['draftCount'],
        'scheduledCount': json['scheduledCount'] == null ? undefined : json['scheduledCount'],
        'publishedCount': json['publishedCount'] == null ? undefined : json['publishedCount'],
        'recentFiles': json['recentFiles'] == null ? undefined : ((json['recentFiles'] as Array<any>).map(FileDtoFromJSON)),
    };
}

export function OverviewDtoToJSON(value?: OverviewDto | null): any {
    if (value == null) return value;
    return {
        'draftCount': value['draftCount'],
        'scheduledCount': value['scheduledCount'],
        'publishedCount': value['publishedCount'],
        'recentFiles': value['recentFiles'] == null ? undefined : ((value['recentFiles'] as Array<any>).map(FileDtoToJSON)),
    };
}
