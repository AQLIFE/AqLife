# DevelopmentPlanApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|------------- | ------------- | -------------|
| [**apiDevelopmentPlansGet**](DevelopmentPlanApi.md#apidevelopmentplansget) | **GET** /api/development-plans |  |
| [**apiDevelopmentPlansPatch**](DevelopmentPlanApi.md#apidevelopmentplanspatch) | **PATCH** /api/development-plans |  |
| [**apiDevelopmentPlansPost**](DevelopmentPlanApi.md#apidevelopmentplanspost) | **POST** /api/development-plans |  |
| [**apiDevelopmentPlansUIDArchivePost**](DevelopmentPlanApi.md#apidevelopmentplansuidarchivepost) | **POST** /api/development-plans/{UID}/archive |  |



## apiDevelopmentPlansGet

> Array&lt;DevelopmentPlanDto&gt; apiDevelopmentPlansGet(uID, title)



### Example

```ts
import {
  Configuration,
  DevelopmentPlanApi,
} from '@aqlife/api-contract';
import type { ApiDevelopmentPlansGetRequest } from '@aqlife/api-contract';

async function example() {
  console.log("🚀 Testing @aqlife/api-contract SDK...");
  const api = new DevelopmentPlanApi();

  const body = {
    // string (optional)
    uID: 38400000-8cf0-11bd-b23e-10b96e4ef00d,
    // string (optional)
    title: title_example,
  } satisfies ApiDevelopmentPlansGetRequest;

  try {
    const data = await api.apiDevelopmentPlansGet(body);
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
| **uID** | `string` |  | [Optional] [Defaults to `undefined`] |
| **title** | `string` |  | [Optional] [Defaults to `undefined`] |

### Return type

[**Array&lt;DevelopmentPlanDto&gt;**](DevelopmentPlanDto.md)

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


## apiDevelopmentPlansPatch

> string apiDevelopmentPlansPatch(updateDevelopmentPlanCommand)



### Example

```ts
import {
  Configuration,
  DevelopmentPlanApi,
} from '@aqlife/api-contract';
import type { ApiDevelopmentPlansPatchRequest } from '@aqlife/api-contract';

async function example() {
  console.log("🚀 Testing @aqlife/api-contract SDK...");
  const api = new DevelopmentPlanApi();

  const body = {
    // UpdateDevelopmentPlanCommand (optional)
    updateDevelopmentPlanCommand: ...,
  } satisfies ApiDevelopmentPlansPatchRequest;

  try {
    const data = await api.apiDevelopmentPlansPatch(body);
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
| **updateDevelopmentPlanCommand** | [UpdateDevelopmentPlanCommand](UpdateDevelopmentPlanCommand.md) |  | [Optional] |

### Return type

**string**

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: `application/json`, `text/json`, `application/*+json`
- **Accept**: `text/plain`, `application/json`, `text/json`


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | OK |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#api-endpoints) [[Back to Model list]](../README.md#models) [[Back to README]](../README.md)


## apiDevelopmentPlansPost

> string apiDevelopmentPlansPost(createDevelopmentPlanCommand)



### Example

```ts
import {
  Configuration,
  DevelopmentPlanApi,
} from '@aqlife/api-contract';
import type { ApiDevelopmentPlansPostRequest } from '@aqlife/api-contract';

async function example() {
  console.log("🚀 Testing @aqlife/api-contract SDK...");
  const api = new DevelopmentPlanApi();

  const body = {
    // CreateDevelopmentPlanCommand (optional)
    createDevelopmentPlanCommand: ...,
  } satisfies ApiDevelopmentPlansPostRequest;

  try {
    const data = await api.apiDevelopmentPlansPost(body);
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
| **createDevelopmentPlanCommand** | [CreateDevelopmentPlanCommand](CreateDevelopmentPlanCommand.md) |  | [Optional] |

### Return type

**string**

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: `application/json`, `text/json`, `application/*+json`
- **Accept**: `text/plain`, `application/json`, `text/json`


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | OK |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#api-endpoints) [[Back to Model list]](../README.md#models) [[Back to README]](../README.md)


## apiDevelopmentPlansUIDArchivePost

> string apiDevelopmentPlansUIDArchivePost(uID)



### Example

```ts
import {
  Configuration,
  DevelopmentPlanApi,
} from '@aqlife/api-contract';
import type { ApiDevelopmentPlansUIDArchivePostRequest } from '@aqlife/api-contract';

async function example() {
  console.log("🚀 Testing @aqlife/api-contract SDK...");
  const api = new DevelopmentPlanApi();

  const body = {
    // string
    uID: 38400000-8cf0-11bd-b23e-10b96e4ef00d,
  } satisfies ApiDevelopmentPlansUIDArchivePostRequest;

  try {
    const data = await api.apiDevelopmentPlansUIDArchivePost(body);
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
| **uID** | `string` |  | [Defaults to `undefined`] |

### Return type

**string**

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

