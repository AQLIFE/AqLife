# FileApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|------------- | ------------- | -------------|
| [**apiFileDelete**](FileApi.md#apifiledelete) | **DELETE** /api/File |  |
| [**apiFileDownloadGet**](FileApi.md#apifiledownloadget) | **GET** /api/File/download |  |
| [**apiFileGet**](FileApi.md#apifileget) | **GET** /api/File |  |
| [**apiFilePatch**](FileApi.md#apifilepatch) | **PATCH** /api/File |  |
| [**apiFilePreviewGet**](FileApi.md#apifilepreviewget) | **GET** /api/File/preview |  |
| [**apiFileTagPatch**](FileApi.md#apifiletagpatch) | **PATCH** /api/File/tag |  |
| [**apiFileUploadPost**](FileApi.md#apifileuploadpost) | **POST** /api/File/Upload |  |



## apiFileDelete

> apiFileDelete(deleteFileCommand)



### Example

```ts
import {
  Configuration,
  FileApi,
} from '';
import type { ApiFileDeleteRequest } from '';

async function example() {
  console.log("🚀 Testing  SDK...");
  const api = new FileApi();

  const body = {
    // DeleteFileCommand (optional)
    deleteFileCommand: ...,
  } satisfies ApiFileDeleteRequest;

  try {
    const data = await api.apiFileDelete(body);
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
| **deleteFileCommand** | [DeleteFileCommand](DeleteFileCommand.md) |  | [Optional] |

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


## apiFileDownloadGet

> apiFileDownloadGet(uID)



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
    uID: 38400000-8cf0-11bd-b23e-10b96e4ef00d,
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


## apiFileGet

> Array&lt;FileMetadataDto&gt; apiFileGet(uID, title)



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
    uID: 38400000-8cf0-11bd-b23e-10b96e4ef00d,
    // string (optional)
    title: title_example,
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
| **uID** | `string` |  | [Optional] [Defaults to `undefined`] |
| **title** | `string` |  | [Optional] [Defaults to `undefined`] |

### Return type

[**Array&lt;FileMetadataDto&gt;**](FileMetadataDto.md)

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


## apiFilePatch

> string apiFilePatch(updateFileCommand)



### Example

```ts
import {
  Configuration,
  FileApi,
} from '';
import type { ApiFilePatchRequest } from '';

async function example() {
  console.log("🚀 Testing  SDK...");
  const api = new FileApi();

  const body = {
    // UpdateFileCommand (optional)
    updateFileCommand: ...,
  } satisfies ApiFilePatchRequest;

  try {
    const data = await api.apiFilePatch(body);
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
| **updateFileCommand** | [UpdateFileCommand](UpdateFileCommand.md) |  | [Optional] |

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


## apiFilePreviewGet

> apiFilePreviewGet(uID)



### Example

```ts
import {
  Configuration,
  FileApi,
} from '';
import type { ApiFilePreviewGetRequest } from '';

async function example() {
  console.log("🚀 Testing  SDK...");
  const api = new FileApi();

  const body = {
    // string (optional)
    uID: 38400000-8cf0-11bd-b23e-10b96e4ef00d,
  } satisfies ApiFilePreviewGetRequest;

  try {
    const data = await api.apiFilePreviewGet(body);
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


## apiFileTagPatch

> string apiFileTagPatch(updateFileTagCommand)



### Example

```ts
import {
  Configuration,
  FileApi,
} from '';
import type { ApiFileTagPatchRequest } from '';

async function example() {
  console.log("🚀 Testing  SDK...");
  const api = new FileApi();

  const body = {
    // UpdateFileTagCommand (optional)
    updateFileTagCommand: ...,
  } satisfies ApiFileTagPatchRequest;

  try {
    const data = await api.apiFileTagPatch(body);
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
| **updateFileTagCommand** | [UpdateFileTagCommand](UpdateFileTagCommand.md) |  | [Optional] |

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


## apiFileUploadPost

> Array&lt;string&gt; apiFileUploadPost(createFileCommand)



### Example

```ts
import {
  Configuration,
  FileApi,
} from '';
import type { ApiFileUploadPostRequest } from '';

async function example() {
  console.log("🚀 Testing  SDK...");
  const api = new FileApi();

  const body = {
    // CreateFileCommand (optional)
    createFileCommand: ...,
  } satisfies ApiFileUploadPostRequest;

  try {
    const data = await api.apiFileUploadPost(body);
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
| **createFileCommand** | [CreateFileCommand](CreateFileCommand.md) |  | [Optional] |

### Return type

**Array<string>**

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

