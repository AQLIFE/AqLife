# FileApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|------------- | ------------- | -------------|
| [**apiFileDownloadGet**](FileApi.md#apifiledownloadget) | **GET** /api/File/download |  |
| [**apiFileGet**](FileApi.md#apifileget) | **GET** /api/File |  |
| [**apiFileReceivePost**](FileApi.md#apifilereceivepost) | **POST** /api/File/receive |  |



## apiFileDownloadGet

> apiFileDownloadGet(title, id)



### Example

```ts
import {
  Configuration,
  FileApi,
} from '';
import type { ApiFileDownloadGetRequest } from '';

async function example() {
  console.log("🚀 Testing  SDK...");
  const api = new FileApi();

  const body = {
    // string (optional)
    title: title_example,
    // string (optional)
    id: 38400000-8cf0-11bd-b23e-10b96e4ef00d,
  } satisfies ApiFileDownloadGetRequest;

  try {
    const data = await api.apiFileDownloadGet(body);
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
| **title** | `string` |  | [Optional] [Defaults to `undefined`] |
| **id** | `string` |  | [Optional] [Defaults to `undefined`] |

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


## apiFileGet

> Array&lt;FileDto&gt; apiFileGet(title, id)



### Example

```ts
import {
  Configuration,
  FileApi,
} from '';
import type { ApiFileGetRequest } from '';

async function example() {
  console.log("🚀 Testing  SDK...");
  const api = new FileApi();

  const body = {
    // string (optional)
    title: title_example,
    // string (optional)
    id: 38400000-8cf0-11bd-b23e-10b96e4ef00d,
  } satisfies ApiFileGetRequest;

  try {
    const data = await api.apiFileGet(body);
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
| **title** | `string` |  | [Optional] [Defaults to `undefined`] |
| **id** | `string` |  | [Optional] [Defaults to `undefined`] |

### Return type

[**Array&lt;FileDto&gt;**](FileDto.md)

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


## apiFileReceivePost

> FileDto apiFileReceivePost(file)



### Example

```ts
import {
  Configuration,
  FileApi,
} from '';
import type { ApiFileReceivePostRequest } from '';

async function example() {
  console.log("🚀 Testing  SDK...");
  const api = new FileApi();

  const body = {
    // Blob (optional)
    file: BINARY_DATA_HERE,
  } satisfies ApiFileReceivePostRequest;

  try {
    const data = await api.apiFileReceivePost(body);
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
| **file** | `Blob` |  | [Optional] [Defaults to `undefined`] |

### Return type

[**FileDto**](FileDto.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: `multipart/form-data`
- **Accept**: `text/plain`, `application/json`, `text/json`


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | OK |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#api-endpoints) [[Back to Model list]](../README.md#models) [[Back to README]](../README.md)

