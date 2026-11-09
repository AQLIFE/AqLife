# CategoryApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|------------- | ------------- | -------------|
| [**apiCategoryGet**](CategoryApi.md#apicategoryget) | **GET** /api/Category |  |



## apiCategoryGet

> CategoryDtoPageResult apiCategoryGet(cID, searchType)



### Example

```ts
import {
  Configuration,
  CategoryApi,
} from '@aqlife/api-contract';
import type { ApiCategoryGetRequest } from '@aqlife/api-contract';

async function example() {
  console.log("🚀 Testing @aqlife/api-contract SDK...");
  const api = new CategoryApi();

  const body = {
    // string (optional)
    cID: 38400000-8cf0-11bd-b23e-10b96e4ef00d,
    // CategoryQueryType | - 0 = Blog - 1 = Favorite (optional)
    searchType: ...,
  } satisfies ApiCategoryGetRequest;

  try {
    const data = await api.apiCategoryGet(body);
    console.log(data);
  } catch (error) {
    console.error(error);
  }
}

// Run the test
example().catch(console.error);
```

### Parameters


| Name | Type | Description  | Notes |
|------------- | ------------- | ------------- | -------------|
| **cID** | `string` |  | [Optional] [Defaults to `undefined`] |
| **searchType** | `CategoryQueryType` | - 0 &#x3D; Blog - 1 &#x3D; Favorite | [Optional] [Defaults to `undefined`] [Enum: 0, 1] |

### Return type

[**CategoryDtoPageResult**](CategoryDtoPageResult.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: `text/plain`, `application/json`, `text/json`


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | OK |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#api-endpoints) [[Back to Model list]](../README.md#models) [[Back to README]](../README.md)

