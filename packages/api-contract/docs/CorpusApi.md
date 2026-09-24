# CorpusApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|------------- | ------------- | -------------|
| [**apiCorpusDelete**](CorpusApi.md#apicorpusdelete) | **DELETE** /api/Corpus |  |
| [**apiCorpusGet**](CorpusApi.md#apicorpusget) | **GET** /api/Corpus |  |
| [**apiCorpusPost**](CorpusApi.md#apicorpuspost) | **POST** /api/Corpus |  |
| [**apiCorpusRandomGet**](CorpusApi.md#apicorpusrandomget) | **GET** /api/Corpus/random |  |



## apiCorpusDelete

> apiCorpusDelete(deleteCorepusCommand)



### Example

```ts
import {
  Configuration,
  CorpusApi,
} from '@aqlife/api-contract';
import type { ApiCorpusDeleteRequest } from '@aqlife/api-contract';

async function example() {
  console.log("🚀 Testing @aqlife/api-contract SDK...");
  const api = new CorpusApi();

  const body = {
    // DeleteCorepusCommand (optional)
    deleteCorepusCommand: ...,
  } satisfies ApiCorpusDeleteRequest;

  try {
    const data = await api.apiCorpusDelete(body);
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
| **deleteCorepusCommand** | [DeleteCorepusCommand](DeleteCorepusCommand.md) |  | [Optional] |

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


## apiCorpusGet

> CorpusDtoPageResult apiCorpusGet(uID, content, page, pageSize)



### Example

```ts
import {
  Configuration,
  CorpusApi,
} from '@aqlife/api-contract';
import type { ApiCorpusGetRequest } from '@aqlife/api-contract';

async function example() {
  console.log("🚀 Testing @aqlife/api-contract SDK...");
  const api = new CorpusApi();

  const body = {
    // string (optional)
    uID: 38400000-8cf0-11bd-b23e-10b96e4ef00d,
    // string (optional)
    content: content_example,
    // number (optional)
    page: 56,
    // number (optional)
    pageSize: 56,
  } satisfies ApiCorpusGetRequest;

  try {
    const data = await api.apiCorpusGet(body);
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
| **content** | `string` |  | [Optional] [Defaults to `undefined`] |
| **page** | `number` |  | [Optional] [Defaults to `undefined`] |
| **pageSize** | `number` |  | [Optional] [Defaults to `undefined`] |

### Return type

[**CorpusDtoPageResult**](CorpusDtoPageResult.md)

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


## apiCorpusPost

> string apiCorpusPost(createCorpusCommand)



### Example

```ts
import {
  Configuration,
  CorpusApi,
} from '@aqlife/api-contract';
import type { ApiCorpusPostRequest } from '@aqlife/api-contract';

async function example() {
  console.log("🚀 Testing @aqlife/api-contract SDK...");
  const api = new CorpusApi();

  const body = {
    // CreateCorpusCommand (optional)
    createCorpusCommand: ...,
  } satisfies ApiCorpusPostRequest;

  try {
    const data = await api.apiCorpusPost(body);
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
| **createCorpusCommand** | [CreateCorpusCommand](CreateCorpusCommand.md) |  | [Optional] |

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


## apiCorpusRandomGet

> string apiCorpusRandomGet()



### Example

```ts
import {
  Configuration,
  CorpusApi,
} from '@aqlife/api-contract';
import type { ApiCorpusRandomGetRequest } from '@aqlife/api-contract';

async function example() {
  console.log("🚀 Testing @aqlife/api-contract SDK...");
  const api = new CorpusApi();

  try {
    const data = await api.apiCorpusRandomGet();
    console.log(data);
  } catch (error) {
    console.error(error);
  }
}

// Run the test
example().catch(console.error);
```

### Parameters

This endpoint does not need any parameter.

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

