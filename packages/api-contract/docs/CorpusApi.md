# CorpusApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|------------- | ------------- | -------------|
| [**apiCorpusDelete**](CorpusApi.md#apicorpusdelete) | **DELETE** /api/Corpus |  |
| [**apiCorpusGet**](CorpusApi.md#apicorpusget) | **GET** /api/Corpus |  |
| [**apiCorpusGuidGet**](CorpusApi.md#apicorpusguidget) | **GET** /api/Corpus/{guid} |  |
| [**apiCorpusPost**](CorpusApi.md#apicorpuspost) | **POST** /api/Corpus |  |
| [**apiCorpusRandomGet**](CorpusApi.md#apicorpusrandomget) | **GET** /api/Corpus/random |  |
| [**apiCorpusSearchGet**](CorpusApi.md#apicorpussearchget) | **GET** /api/Corpus/search |  |



## apiCorpusDelete

> number apiCorpusDelete(guid)



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
    // string (optional)
    guid: 38400000-8cf0-11bd-b23e-10b96e4ef00d,
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
| **guid** | `string` |  | [Optional] [Defaults to `undefined`] |

### Return type

**number**

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


## apiCorpusGet

> Array&lt;string&gt; apiCorpusGet()



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

  try {
    const data = await api.apiCorpusGet();
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

**Array<string>**

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


## apiCorpusGuidGet

> string apiCorpusGuidGet(guid)



### Example

```ts
import {
  Configuration,
  CorpusApi,
} from '@aqlife/api-contract';
import type { ApiCorpusGuidGetRequest } from '@aqlife/api-contract';

async function example() {
  console.log("🚀 Testing @aqlife/api-contract SDK...");
  const api = new CorpusApi();

  const body = {
    // string
    guid: 38400000-8cf0-11bd-b23e-10b96e4ef00d,
  } satisfies ApiCorpusGuidGetRequest;

  try {
    const data = await api.apiCorpusGuidGet(body);
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
| **guid** | `string` |  | [Defaults to `undefined`] |

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


## apiCorpusPost

> string apiCorpusPost(content)



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
    // string (optional)
    content: content_example,
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
| **content** | `string` |  | [Optional] [Defaults to `undefined`] |

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


## apiCorpusSearchGet

> string apiCorpusSearchGet(query)



### Example

```ts
import {
  Configuration,
  CorpusApi,
} from '@aqlife/api-contract';
import type { ApiCorpusSearchGetRequest } from '@aqlife/api-contract';

async function example() {
  console.log("🚀 Testing @aqlife/api-contract SDK...");
  const api = new CorpusApi();

  const body = {
    // string (optional)
    query: query_example,
  } satisfies ApiCorpusSearchGetRequest;

  try {
    const data = await api.apiCorpusSearchGet(body);
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
| **query** | `string` |  | [Optional] [Defaults to `undefined`] |

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

