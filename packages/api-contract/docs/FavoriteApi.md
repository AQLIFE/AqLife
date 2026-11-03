# FavoriteApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|------------- | ------------- | -------------|
| [**apiFavoriteDelete**](FavoriteApi.md#apifavoritedelete) | **DELETE** /api/Favorite |  |
| [**apiFavoriteGet**](FavoriteApi.md#apifavoriteget) | **GET** /api/Favorite |  |
| [**apiFavoritePost**](FavoriteApi.md#apifavoritepost) | **POST** /api/Favorite |  |
| [**apiFavoritePublishPatch**](FavoriteApi.md#apifavoritepublishpatch) | **PATCH** /api/Favorite/publish |  |
| [**apiFavoritePut**](FavoriteApi.md#apifavoriteput) | **PUT** /api/Favorite |  |



## apiFavoriteDelete

> apiFavoriteDelete(uID)



### Example

```ts
import {
  Configuration,
  FavoriteApi,
} from '@aqlife/api-contract';
import type { ApiFavoriteDeleteRequest } from '@aqlife/api-contract';

async function example() {
  console.log("🚀 Testing @aqlife/api-contract SDK...");
  const api = new FavoriteApi();

  const body = {
    // string (optional)
    uID: 38400000-8cf0-11bd-b23e-10b96e4ef00d,
  } satisfies ApiFavoriteDeleteRequest;

  try {
    const data = await api.apiFavoriteDelete(body);
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

### Return type

`void` (Empty response body)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: Not defined


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | OK |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#api-endpoints) [[Back to Model list]](../README.md#models) [[Back to README]](../README.md)


## apiFavoriteGet

> FavoriteDtoPageResult apiFavoriteGet(uID, contentType, tagId, page, pageSize, includeUnpublished)



### Example

```ts
import {
  Configuration,
  FavoriteApi,
} from '@aqlife/api-contract';
import type { ApiFavoriteGetRequest } from '@aqlife/api-contract';

async function example() {
  console.log("🚀 Testing @aqlife/api-contract SDK...");
  const api = new FavoriteApi();

  const body = {
    // string (optional)
    uID: 38400000-8cf0-11bd-b23e-10b96e4ef00d,
    // string (optional)
    contentType: contentType_example,
    // string (optional)
    tagId: 38400000-8cf0-11bd-b23e-10b96e4ef00d,
    // number (optional)
    page: 56,
    // number (optional)
    pageSize: 56,
    // boolean (optional)
    includeUnpublished: true,
  } satisfies ApiFavoriteGetRequest;

  try {
    const data = await api.apiFavoriteGet(body);
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
| **contentType** | `string` |  | [Optional] [Defaults to `undefined`] |
| **tagId** | `string` |  | [Optional] [Defaults to `undefined`] |
| **page** | `number` |  | [Optional] [Defaults to `undefined`] |
| **pageSize** | `number` |  | [Optional] [Defaults to `undefined`] |
| **includeUnpublished** | `boolean` |  | [Optional] [Defaults to `undefined`] |

### Return type

[**FavoriteDtoPageResult**](FavoriteDtoPageResult.md)

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


## apiFavoritePost

> string apiFavoritePost(createFavoriteCommand)



### Example

```ts
import {
  Configuration,
  FavoriteApi,
} from '@aqlife/api-contract';
import type { ApiFavoritePostRequest } from '@aqlife/api-contract';

async function example() {
  console.log("🚀 Testing @aqlife/api-contract SDK...");
  const api = new FavoriteApi();

  const body = {
    // CreateFavoriteCommand (optional)
    createFavoriteCommand: ...,
  } satisfies ApiFavoritePostRequest;

  try {
    const data = await api.apiFavoritePost(body);
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
| **createFavoriteCommand** | [CreateFavoriteCommand](CreateFavoriteCommand.md) |  | [Optional] |

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


## apiFavoritePublishPatch

> string apiFavoritePublishPatch(setFavoritePublishedCommand)



### Example

```ts
import {
  Configuration,
  FavoriteApi,
} from '@aqlife/api-contract';
import type { ApiFavoritePublishPatchRequest } from '@aqlife/api-contract';

async function example() {
  console.log("🚀 Testing @aqlife/api-contract SDK...");
  const api = new FavoriteApi();

  const body = {
    // SetFavoritePublishedCommand (optional)
    setFavoritePublishedCommand: ...,
  } satisfies ApiFavoritePublishPatchRequest;

  try {
    const data = await api.apiFavoritePublishPatch(body);
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
| **setFavoritePublishedCommand** | [SetFavoritePublishedCommand](SetFavoritePublishedCommand.md) |  | [Optional] |

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


## apiFavoritePut

> string apiFavoritePut(updateFavoriteCommand)



### Example

```ts
import {
  Configuration,
  FavoriteApi,
} from '@aqlife/api-contract';
import type { ApiFavoritePutRequest } from '@aqlife/api-contract';

async function example() {
  console.log("🚀 Testing @aqlife/api-contract SDK...");
  const api = new FavoriteApi();

  const body = {
    // UpdateFavoriteCommand (optional)
    updateFavoriteCommand: ...,
  } satisfies ApiFavoritePutRequest;

  try {
    const data = await api.apiFavoritePut(body);
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
| **updateFavoriteCommand** | [UpdateFavoriteCommand](UpdateFavoriteCommand.md) |  | [Optional] |

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

