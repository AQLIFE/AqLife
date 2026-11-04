# DevelopmentPlanApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|------------- | ------------- | -------------|
| [**apiDevelopmentPlansGet**](DevelopmentPlanApi.md#apidevelopmentplansget) | **GET** /api/development-plans |  |
| [**apiDevelopmentPlansPatch**](DevelopmentPlanApi.md#apidevelopmentplanspatch) | **PATCH** /api/development-plans |  |
| [**apiDevelopmentPlansPost**](DevelopmentPlanApi.md#apidevelopmentplanspost) | **POST** /api/development-plans |  |
| [**apiDevelopmentPlansUidArchivePost**](DevelopmentPlanApi.md#apidevelopmentplansuidarchivepost) | **POST** /api/development-plans/{uid}/archive |  |



## apiDevelopmentPlansGet

> Array&lt;DevelopmentPlanDto&gt; apiDevelopmentPlansGet(includeArchived)



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
    // boolean (optional)
    includeArchived: true,
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
| **includeArchived** | `boolean` |  | [Optional] [Defaults to `undefined`] |

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


## apiDevelopmentPlansUidArchivePost

> string apiDevelopmentPlansUidArchivePost(uid)



### Example

```ts
import {
  Configuration,
  DevelopmentPlanApi,
} from '@aqlife/api-contract';
import type { ApiDevelopmentPlansUidArchivePostRequest } from '@aqlife/api-contract';

async function example() {
  console.log("🚀 Testing @aqlife/api-contract SDK...");
  const api = new DevelopmentPlanApi();

  const body = {
    // string
    uid: 38400000-8cf0-11bd-b23e-10b96e4ef00d,
  } satisfies ApiDevelopmentPlansUidArchivePostRequest;

  try {
    const data = await api.apiDevelopmentPlansUidArchivePost(body);
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
| **uid** | `string` |  | [Defaults to `undefined`] |

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

