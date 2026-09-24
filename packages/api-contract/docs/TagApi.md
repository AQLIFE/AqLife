# TagApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|------------- | ------------- | -------------|
| [**apiTagDelete**](TagApi.md#apitagdelete) | **DELETE** /api/Tag |  |
| [**apiTagGet**](TagApi.md#apitagget) | **GET** /api/Tag |  |
| [**apiTagOverviewGet**](TagApi.md#apitagoverviewget) | **GET** /api/Tag/overview |  |
| [**apiTagPatch**](TagApi.md#apitagpatch) | **PATCH** /api/Tag |  |
| [**apiTagPost**](TagApi.md#apitagpost) | **POST** /api/Tag |  |



## apiTagDelete

> apiTagDelete(deleteTagCommand)



### Example

```ts
import {
  Configuration,
  TagApi,
} from '@aqlife/api-contract';
import type { ApiTagDeleteRequest } from '@aqlife/api-contract';

async function example() {
  console.log("🚀 Testing @aqlife/api-contract SDK...");
  const api = new TagApi();

  const body = {
    // DeleteTagCommand (optional)
    deleteTagCommand: ...,
  } satisfies ApiTagDeleteRequest;

  try {
    const data = await api.apiTagDelete(body);
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
| **deleteTagCommand** | [DeleteTagCommand](DeleteTagCommand.md) |  | [Optional] |

### Return type

`void` (Empty response body)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: `application/json`, `text/json`, `application/*+json`
- **Accept**: Not defined


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | OK |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#api-endpoints) [[Back to Model list]](../README.md#models) [[Back to README]](../README.md)


## apiTagGet

> TagDtoPageResult apiTagGet(uID, tag, page, pageSize)



### Example

```ts
import {
  Configuration,
  TagApi,
} from '@aqlife/api-contract';
import type { ApiTagGetRequest } from '@aqlife/api-contract';

async function example() {
  console.log("🚀 Testing @aqlife/api-contract SDK...");
  const api = new TagApi();

  const body = {
    // string (optional)
    uID: 38400000-8cf0-11bd-b23e-10b96e4ef00d,
    // string (optional)
    tag: tag_example,
    // number (optional)
    page: 56,
    // number (optional)
    pageSize: 56,
  } satisfies ApiTagGetRequest;

  try {
    const data = await api.apiTagGet(body);
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
| **tag** | `string` |  | [Optional] [Defaults to `undefined`] |
| **page** | `number` |  | [Optional] [Defaults to `undefined`] |
| **pageSize** | `number` |  | [Optional] [Defaults to `undefined`] |

### Return type

[**TagDtoPageResult**](TagDtoPageResult.md)

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


## apiTagOverviewGet

> Array&lt;BlogCategoryStatistics&gt; apiTagOverviewGet(isAll)



### Example

```ts
import {
  Configuration,
  TagApi,
} from '@aqlife/api-contract';
import type { ApiTagOverviewGetRequest } from '@aqlife/api-contract';

async function example() {
  console.log("🚀 Testing @aqlife/api-contract SDK...");
  const api = new TagApi();

  const body = {
    // boolean (optional)
    isAll: true,
  } satisfies ApiTagOverviewGetRequest;

  try {
    const data = await api.apiTagOverviewGet(body);
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
| **isAll** | `boolean` |  | [Optional] [Defaults to `undefined`] |

### Return type

[**Array&lt;BlogCategoryStatistics&gt;**](BlogCategoryStatistics.md)

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


## apiTagPatch

> string apiTagPatch(updateTagCommand)



### Example

```ts
import {
  Configuration,
  TagApi,
} from '@aqlife/api-contract';
import type { ApiTagPatchRequest } from '@aqlife/api-contract';

async function example() {
  console.log("🚀 Testing @aqlife/api-contract SDK...");
  const api = new TagApi();

  const body = {
    // UpdateTagCommand (optional)
    updateTagCommand: ...,
  } satisfies ApiTagPatchRequest;

  try {
    const data = await api.apiTagPatch(body);
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
| **updateTagCommand** | [UpdateTagCommand](UpdateTagCommand.md) |  | [Optional] |

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


## apiTagPost

> string apiTagPost(createTagCommand)



### Example

```ts
import {
  Configuration,
  TagApi,
} from '@aqlife/api-contract';
import type { ApiTagPostRequest } from '@aqlife/api-contract';

async function example() {
  console.log("🚀 Testing @aqlife/api-contract SDK...");
  const api = new TagApi();

  const body = {
    // CreateTagCommand (optional)
    createTagCommand: ...,
  } satisfies ApiTagPostRequest;

  try {
    const data = await api.apiTagPost(body);
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
| **createTagCommand** | [CreateTagCommand](CreateTagCommand.md) |  | [Optional] |

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

