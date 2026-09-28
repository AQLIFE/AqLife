# OverviewApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|------------- | ------------- | -------------|
| [**apiOverviewGet**](OverviewApi.md#apioverviewget) | **GET** /api/Overview |  |



## apiOverviewGet

> OverviewDto apiOverviewGet(recentCount)



### Example

```ts
import {
  Configuration,
  OverviewApi,
} from '@aqlife/api-contract';
import type { ApiOverviewGetRequest } from '@aqlife/api-contract';

async function example() {
  console.log("🚀 Testing @aqlife/api-contract SDK...");
  const api = new OverviewApi();

  const body = {
    // number (optional)
    recentCount: 56,
  } satisfies ApiOverviewGetRequest;

  try {
    const data = await api.apiOverviewGet(body);
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
| **recentCount** | `number` |  | [Optional] [Defaults to `undefined`] |

### Return type

[**OverviewDto**](OverviewDto.md)

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

