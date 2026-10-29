# FileApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|------------- | ------------- | -------------|
| [**apiFileCancelSchedulePatch**](FileApi.md#apifilecancelschedulepatch) | **PATCH** /api/File/CancelSchedule |  |
| [**apiFileDelete**](FileApi.md#apifiledelete) | **DELETE** /api/File |  |
| [**apiFileDownloadGet**](FileApi.md#apifiledownloadget) | **GET** /api/File/download |  |
| [**apiFileGet**](FileApi.md#apifileget) | **GET** /api/File |  |
| [**apiFilePatch**](FileApi.md#apifilepatch) | **PATCH** /api/File |  |
| [**apiFilePreviewCompletePatch**](FileApi.md#apifilepreviewcompletepatch) | **PATCH** /api/File/preview/complete |  |
| [**apiFilePreviewGet**](FileApi.md#apifilepreviewget) | **GET** /api/File/preview |  |
| [**apiFileSchedulePatch**](FileApi.md#apifileschedulepatch) | **PATCH** /api/File/schedule |  |
| [**apiFileTagPatch**](FileApi.md#apifiletagpatch) | **PATCH** /api/File/tag |  |
| [**apiFileUploadPost**](FileApi.md#apifileuploadpost) | **POST** /api/File/Upload |  |
| [**apiFileUploadsPost**](FileApi.md#apifileuploadspost) | **POST** /api/File/uploads |  |
| [**apiFileUploadsUploadIdChunksIndexPut**](FileApi.md#apifileuploadsuploadidchunksindexput) | **PUT** /api/File/uploads/{uploadId}/chunks/{index} |  |
| [**apiFileUploadsUploadIdCompletePost**](FileApi.md#apifileuploadsuploadidcompletepost) | **POST** /api/File/uploads/{uploadId}/complete |  |
| [**apiFileUploadsUploadIdGet**](FileApi.md#apifileuploadsuploadidget) | **GET** /api/File/uploads/{uploadId} |  |



## apiFileCancelSchedulePatch

> string apiFileCancelSchedulePatch(cancelScheduledFileCommand)



### Example

```ts
import {
  Configuration,
  FileApi,
} from '@aqlife/api-contract';
import type { ApiFileCancelSchedulePatchRequest } from '@aqlife/api-contract';

async function example() {
  console.log("🚀 Testing @aqlife/api-contract SDK...");
  const api = new FileApi();

  const body = {
    // CancelScheduledFileCommand (optional)
    cancelScheduledFileCommand: ...,
  } satisfies ApiFileCancelSchedulePatchRequest;

  try {
    const data = await api.apiFileCancelSchedulePatch(body);
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
| **cancelScheduledFileCommand** | [CancelScheduledFileCommand](CancelScheduledFileCommand.md) |  | [Optional] |

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


## apiFileDelete

> apiFileDelete(deleteFileCommand)



### Example

```ts
import {
  Configuration,
  FileApi,
} from '@aqlife/api-contract';
import type { ApiFileDeleteRequest } from '@aqlife/api-contract';

async function example() {
  console.log("🚀 Testing @aqlife/api-contract SDK...");
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

> Blob apiFileDownloadGet(uID)



### Example

```ts
import {
  Configuration,
  FileApi,
} from '@aqlife/api-contract';
import type { ApiFileDownloadGetRequest } from '@aqlife/api-contract';

async function example() {
  console.log("🚀 Testing @aqlife/api-contract SDK...");
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

**Blob**

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


## apiFileGet

> FileDtoPageResult apiFileGet(uID, title, categoryUID, page, pageSize, order, scope)



### Example

```ts
import {
  Configuration,
  FileApi,
} from '@aqlife/api-contract';
import type { ApiFileGetRequest } from '@aqlife/api-contract';

async function example() {
  console.log("🚀 Testing @aqlife/api-contract SDK...");
  const api = new FileApi();

  const body = {
    // string (optional)
    uID: 38400000-8cf0-11bd-b23e-10b96e4ef00d,
    // string (optional)
    title: title_example,
    // string (optional)
    categoryUID: 38400000-8cf0-11bd-b23e-10b96e4ef00d,
    // number (optional)
    page: 56,
    // number (optional)
    pageSize: 56,
    // FileOrder | - 0 = Latest - 1 = Earliest - 2 = MostViewed (optional)
    order: ...,
    // FileScope | - 0 = Default：默认搜索策略，包含所有公开文件类型 - 1 = Image：Image 搜索策略 - 2 = Blog：Blog 搜索策略 - 3 = Template：模板搜索策略 - 4 = All：全集搜索策略 - 5 = Video：Video 搜索策略 (optional)
    scope: ...,
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
| **categoryUID** | `string` |  | [Optional] [Defaults to `undefined`] |
| **page** | `number` |  | [Optional] [Defaults to `undefined`] |
| **pageSize** | `number` |  | [Optional] [Defaults to `undefined`] |
| **order** | `FileOrder` | - 0 &#x3D; Latest - 1 &#x3D; Earliest - 2 &#x3D; MostViewed | [Optional] [Defaults to `undefined`] [Enum: 0, 1, 2] |
| **scope** | `FileScope` | - 0 &#x3D; Default：默认搜索策略，包含所有公开文件类型 - 1 &#x3D; Image：Image 搜索策略 - 2 &#x3D; Blog：Blog 搜索策略 - 3 &#x3D; Template：模板搜索策略 - 4 &#x3D; All：全集搜索策略 - 5 &#x3D; Video：Video 搜索策略 | [Optional] [Defaults to `undefined`] [Enum: 0, 1, 2, 3, 4, 5] |

### Return type

[**FileDtoPageResult**](FileDtoPageResult.md)

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

> string apiFilePatch(uID, file)



### Example

```ts
import {
  Configuration,
  FileApi,
} from '@aqlife/api-contract';
import type { ApiFilePatchRequest } from '@aqlife/api-contract';

async function example() {
  console.log("🚀 Testing @aqlife/api-contract SDK...");
  const api = new FileApi();

  const body = {
    // string (optional)
    uID: 38400000-8cf0-11bd-b23e-10b96e4ef00d,
    // Blob (optional)
    file: BINARY_DATA_HERE,
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
| **uID** | `string` |  | [Optional] [Defaults to `undefined`] |
| **file** | `Blob` |  | [Optional] [Defaults to `undefined`] |

### Return type

**string**

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


## apiFilePreviewCompletePatch

> string apiFilePreviewCompletePatch(uID)



### Example

```ts
import {
  Configuration,
  FileApi,
} from '@aqlife/api-contract';
import type { ApiFilePreviewCompletePatchRequest } from '@aqlife/api-contract';

async function example() {
  console.log("🚀 Testing @aqlife/api-contract SDK...");
  const api = new FileApi();

  const body = {
    // string (optional)
    uID: 38400000-8cf0-11bd-b23e-10b96e4ef00d,
  } satisfies ApiFilePreviewCompletePatchRequest;

  try {
    const data = await api.apiFilePreviewCompletePatch(body);
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


## apiFilePreviewGet

> Blob apiFilePreviewGet(uID, isTemplate)



### Example

```ts
import {
  Configuration,
  FileApi,
} from '@aqlife/api-contract';
import type { ApiFilePreviewGetRequest } from '@aqlife/api-contract';

async function example() {
  console.log("🚀 Testing @aqlife/api-contract SDK...");
  const api = new FileApi();

  const body = {
    // string (optional)
    uID: 38400000-8cf0-11bd-b23e-10b96e4ef00d,
    // boolean (optional)
    isTemplate: true,
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
| **isTemplate** | `boolean` |  | [Optional] [Defaults to `undefined`] |

### Return type

**Blob**

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


## apiFileSchedulePatch

> string apiFileSchedulePatch(scheduledFileCommand)



### Example

```ts
import {
  Configuration,
  FileApi,
} from '@aqlife/api-contract';
import type { ApiFileSchedulePatchRequest } from '@aqlife/api-contract';

async function example() {
  console.log("🚀 Testing @aqlife/api-contract SDK...");
  const api = new FileApi();

  const body = {
    // ScheduledFileCommand (optional)
    scheduledFileCommand: ...,
  } satisfies ApiFileSchedulePatchRequest;

  try {
    const data = await api.apiFileSchedulePatch(body);
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
| **scheduledFileCommand** | [ScheduledFileCommand](ScheduledFileCommand.md) |  | [Optional] |

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


## apiFileTagPatch

> string apiFileTagPatch(updateFileTagCommand)



### Example

```ts
import {
  Configuration,
  FileApi,
} from '@aqlife/api-contract';
import type { ApiFileTagPatchRequest } from '@aqlife/api-contract';

async function example() {
  console.log("🚀 Testing @aqlife/api-contract SDK...");
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

> Array&lt;string&gt; apiFileUploadPost(isTemplate, file)



### Example

```ts
import {
  Configuration,
  FileApi,
} from '@aqlife/api-contract';
import type { ApiFileUploadPostRequest } from '@aqlife/api-contract';

async function example() {
  console.log("🚀 Testing @aqlife/api-contract SDK...");
  const api = new FileApi();

  const body = {
    // boolean (optional)
    isTemplate: true,
    // Array<Blob> (optional)
    file: /path/to/file.txt,
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
| **isTemplate** | `boolean` |  | [Optional] [Defaults to `undefined`] |
| **file** | `Array<Blob>` |  | [Optional] |

### Return type

**Array<string>**

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


## apiFileUploadsPost

> UploadSession apiFileUploadsPost(startRequest)



### Example

```ts
import {
  Configuration,
  FileApi,
} from '@aqlife/api-contract';
import type { ApiFileUploadsPostRequest } from '@aqlife/api-contract';

async function example() {
  console.log("🚀 Testing @aqlife/api-contract SDK...");
  const api = new FileApi();

  const body = {
    // StartRequest (optional)
    startRequest: ...,
  } satisfies ApiFileUploadsPostRequest;

  try {
    const data = await api.apiFileUploadsPost(body);
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
| **startRequest** | [StartRequest](StartRequest.md) |  | [Optional] |

### Return type

[**UploadSession**](UploadSession.md)

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


## apiFileUploadsUploadIdChunksIndexPut

> apiFileUploadsUploadIdChunksIndexPut(uploadId, index, chunk)



### Example

```ts
import {
  Configuration,
  FileApi,
} from '@aqlife/api-contract';
import type { ApiFileUploadsUploadIdChunksIndexPutRequest } from '@aqlife/api-contract';

async function example() {
  console.log("🚀 Testing @aqlife/api-contract SDK...");
  const api = new FileApi();

  const body = {
    // string
    uploadId: 38400000-8cf0-11bd-b23e-10b96e4ef00d,
    // number
    index: 56,
    // Blob (optional)
    chunk: BINARY_DATA_HERE,
  } satisfies ApiFileUploadsUploadIdChunksIndexPutRequest;

  try {
    const data = await api.apiFileUploadsUploadIdChunksIndexPut(body);
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
| **uploadId** | `string` |  | [Defaults to `undefined`] |
| **index** | `number` |  | [Defaults to `undefined`] |
| **chunk** | `Blob` |  | [Optional] [Defaults to `undefined`] |

### Return type

`void` (Empty response body)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: `multipart/form-data`
- **Accept**: Not defined


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | OK |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#api-endpoints) [[Back to Model list]](../README.md#models) [[Back to README]](../README.md)


## apiFileUploadsUploadIdCompletePost

> Array&lt;string&gt; apiFileUploadsUploadIdCompletePost(uploadId)



### Example

```ts
import {
  Configuration,
  FileApi,
} from '@aqlife/api-contract';
import type { ApiFileUploadsUploadIdCompletePostRequest } from '@aqlife/api-contract';

async function example() {
  console.log("🚀 Testing @aqlife/api-contract SDK...");
  const api = new FileApi();

  const body = {
    // string
    uploadId: 38400000-8cf0-11bd-b23e-10b96e4ef00d,
  } satisfies ApiFileUploadsUploadIdCompletePostRequest;

  try {
    const data = await api.apiFileUploadsUploadIdCompletePost(body);
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
| **uploadId** | `string` |  | [Defaults to `undefined`] |

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


## apiFileUploadsUploadIdGet

> UploadStatus apiFileUploadsUploadIdGet(uploadId)



### Example

```ts
import {
  Configuration,
  FileApi,
} from '@aqlife/api-contract';
import type { ApiFileUploadsUploadIdGetRequest } from '@aqlife/api-contract';

async function example() {
  console.log("🚀 Testing @aqlife/api-contract SDK...");
  const api = new FileApi();

  const body = {
    // string
    uploadId: 38400000-8cf0-11bd-b23e-10b96e4ef00d,
  } satisfies ApiFileUploadsUploadIdGetRequest;

  try {
    const data = await api.apiFileUploadsUploadIdGet(body);
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
| **uploadId** | `string` |  | [Defaults to `undefined`] |

### Return type

[**UploadStatus**](UploadStatus.md)

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

