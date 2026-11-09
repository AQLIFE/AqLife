# CategoryApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|-------------|-------------|-------------|
| [**apiCategoryGet**](CategoryApi.md#apicategoryget) | **GET** /api/Category |  |

## apiCategoryGet

> CategoryDtoPageResult apiCategoryGet(cid, searchType)

### Parameters

| Name | Type | Description | Notes |
|-------------|-------------|-------------|-------------|
| **cid** | `string` | Category UID filter | [Optional] |
| **searchType** | `number` | 0 = Blog, 1 = Favorite | [Optional] |

### Return type

[**CategoryDtoPageResult**](CategoryDtoPageResult.md)

### Authorization

No authorization required
