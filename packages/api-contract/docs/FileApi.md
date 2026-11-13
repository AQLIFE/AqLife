# FileApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|------------- | ------------- | -------------|
| [**apiFileCancelSchedulePatch**](FileApi.md#apifilecancelschedulepatch) | **PATCH** /api/File/CancelSchedule |  |
| [**apiFileDelete**](FileApi.md#apifiledelete) | **DELETE** /api/File |  |
| [**apiFileDownloadGet**](FileApi.md#apifiledownloadget) | **GET** /api/File/download |  |
| [**apiFileGet**](FileApi.md#apifileget) | **GET** /api/File |  |
| [**apiFileHashCheckPost**](FileApi.md#apifilehashcheckpost) | **POST** /api/File/hash/check |  |
| [**apiFileOverviewGet**](FileApi.md#apifileoverviewget) | **GET** /api/File/overview |  |
| [**apiFilePatch**](FileApi.md#apifilepatch) | **PATCH** /api/File |  |
| [**apiFilePreviewCompletePatch**](FileApi.md#apifilepreviewcompletepatch) | **PATCH** /api/File/preview/complete |  |
| [**apiFilePreviewGet**](FileApi.md#apifilepreviewget) | **GET** /api/File/preview |  |
| [**apiFileResumableUploadPost**](FileApi.md#apifileresumableuploadpost) | **POST** /api/File/resumable-upload |  |
| [**apiFileResumableUploadUploadIdChunksIndexPut**](FileApi.md#apifileresumableuploaduploadidchunksindexput) | **PUT** /api/File/resumable-upload/{uploadId}/chunks/{index} |  |
| [**apiFileResumableUploadUploadIdCompletePost**](FileApi.md#apifileresumableuploaduploadidcompletepost) | **POST** /api/File/resumable-upload/{uploadId}/complete |  |
| [**apiFileResumableUploadUploadIdGet**](FileApi.md#apifileresumableuploaduploadidget) | **GET** /api/File/resumable-upload/{uploadId} |  |
| [**apiFileSchedulePatch**](FileApi.md#apifileschedulepatch) | **PATCH** /api/File/schedule |  |
| [**apiFileTagPatch**](FileApi.md#apifiletagpatch) | **PATCH** /api/File/tag |  |
| [**apiFileUploadMultiplePost**](FileApi.md#apifileuploadmultiplepost) | **POST** /api/File/upload/multiple |  |
| [**apiFileUploadPost**](FileApi.md#apifileuploadpost) | **POST** /api/File/upload |  |
| [**apiFileVideoPreviewTicketPost**](FileApi.md#apifilevideopreviewticketpost) | **POST** /api/File/video-preview-ticket |  |
| [**apiFileVideoStreamGet**](FileApi.md#apifilevideostreamget) | **GET** /api/File/video-stream |  |



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


## apiFileHashCheckPost

> FileHashCheckResult apiFileHashCheckPost(checkFileHashQuery)



### Example

```ts
import {
  Configuration,
  FileApi,
} from '@aqlife/api-contract';
import type { ApiFileHashCheckPostRequest } from '@aqlife/api-contract';

async function example() {
  console.log("🚀 Testing @aqlife/api-contract SDK...");
  const api = new FileApi();

  const body = {
    // CheckFileHashQuery (optional)
    checkFileHashQuery: ...,
  } satisfies ApiFileHashCheckPostRequest;

  try {
    const data = await api.apiFileHashCheckPost(body);
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
| **checkFileHashQuery** | [CheckFileHashQuery](CheckFileHashQuery.md) |  | [Optional] |

### Return type

[**FileHashCheckResult**](FileHashCheckResult.md)

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


## apiFileOverviewGet

> FileOverviewDto apiFileOverviewGet(recentCount)



### Example

```ts
import {
  Configuration,
  FileApi,
} from '@aqlife/api-contract';
import type { ApiFileOverviewGetRequest } from '@aqlife/api-contract';

async function example() {
  console.log("🚀 Testing @aqlife/api-contract SDK...");
  const api = new FileApi();

  const body = {
    // number (optional)
    recentCount: 56,
  } satisfies ApiFileOverviewGetRequest;

  try {
    const data = await api.apiFileOverviewGet(body);
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

[**FileOverviewDto**](FileOverviewDto.md)

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


## apiFileResumableUploadPost

> ResumableUploadSession apiFileResumableUploadPost(startVideoUploadCommand)



### Example

```ts
import {
  Configuration,
  FileApi,
} from '@aqlife/api-contract';
import type { ApiFileResumableUploadPostRequest } from '@aqlife/api-contract';

async function example() {
  console.log("🚀 Testing @aqlife/api-contract SDK...");
  const api = new FileApi();

  const body = {
    // StartVideoUploadCommand (optional)
    startVideoUploadCommand: ...,
  } satisfies ApiFileResumableUploadPostRequest;

  try {
    const data = await api.apiFileResumableUploadPost(body);
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
| **startVideoUploadCommand** | [StartVideoUploadCommand](StartVideoUploadCommand.md) |  | [Optional] |

### Return type

[**ResumableUploadSession**](ResumableUploadSession.md)

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


## apiFileResumableUploadUploadIdChunksIndexPut

> apiFileResumableUploadUploadIdChunksIndexPut(uploadId, index, chunk)



### Example

```ts
import {
  Configuration,
  FileApi,
} from '@aqlife/api-contract';
import type { ApiFileResumableUploadUploadIdChunksIndexPutRequest } from '@aqlife/api-contract';

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
  } satisfies ApiFileResumableUploadUploadIdChunksIndexPutRequest;

  try {
    const data = await api.apiFileResumableUploadUploadIdChunksIndexPut(body);
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


## apiFileResumableUploadUploadIdCompletePost

> string apiFileResumableUploadUploadIdCompletePost(uploadId)



### Example

```ts
import {
  Configuration,
  FileApi,
} from '@aqlife/api-contract';
import type { ApiFileResumableUploadUploadIdCompletePostRequest } from '@aqlife/api-contract';

async function example() {
  console.log("🚀 Testing @aqlife/api-contract SDK...");
  const api = new FileApi();

  const body = {
    // string
    uploadId: 38400000-8cf0-11bd-b23e-10b96e4ef00d,
  } satisfies ApiFileResumableUploadUploadIdCompletePostRequest;

  try {
    const data = await api.apiFileResumableUploadUploadIdCompletePost(body);
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


## apiFileResumableUploadUploadIdGet

> ResumableUploadStatus apiFileResumableUploadUploadIdGet(uploadId)



### Example

```ts
import {
  Configuration,
  FileApi,
} from '@aqlife/api-contract';
import type { ApiFileResumableUploadUploadIdGetRequest } from '@aqlife/api-contract';

async function example() {
  console.log("🚀 Testing @aqlife/api-contract SDK...");
  const api = new FileApi();

  const body = {
    // string
    uploadId: 38400000-8cf0-11bd-b23e-10b96e4ef00d,
  } satisfies ApiFileResumableUploadUploadIdGetRequest;

  try {
    const data = await api.apiFileResumableUploadUploadIdGet(body);
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

[**ResumableUploadStatus**](ResumableUploadStatus.md)

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


## apiFileUploadMultiplePost

> Array&lt;string&gt; apiFileUploadMultiplePost(categoryId, files)



### Example

```ts
import {
  Configuration,
  FileApi,
} from '@aqlife/api-contract';
import type { ApiFileUploadMultiplePostRequest } from '@aqlife/api-contract';

async function example() {
  console.log("🚀 Testing @aqlife/api-contract SDK...");
  const api = new FileApi();

  const body = {
    // string (optional)
    categoryId: 38400000-8cf0-11bd-b23e-10b96e4ef00d,
    // Array<Blob> (optional)
    files: /path/to/file.txt,
  } satisfies ApiFileUploadMultiplePostRequest;

  try {
    const data = await api.apiFileUploadMultiplePost(body);
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
| **categoryId** | `string` |  | [Optional] [Defaults to `undefined`] |
| **files** | `Array<Blob>` |  | [Optional] |

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


## apiFileUploadPost

> string apiFileUploadPost(file, categoryId, tags, scheduledTime, isTemplate)



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
    // Blob (optional)
    file: BINARY_DATA_HERE,
    // string (optional)
    categoryId: 38400000-8cf0-11bd-b23e-10b96e4ef00d,
    // Array<string> (optional)
    tags: ...,
    // string (optional)
    scheduledTime: 2013-10-20T19:20:30+01:00,
    // boolean (optional)
    isTemplate: true,
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
| **file** | `Blob` |  | [Optional] [Defaults to `undefined`] |
| **categoryId** | `string` |  | [Optional] [Defaults to `undefined`] |
| **tags** | `Array<string>` |  | [Optional] |
| **scheduledTime** | `string` |  | [Optional] [Defaults to `undefined`] |
| **isTemplate** | `boolean` |  | [Optional] [Defaults to `undefined`] |

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


## apiFileVideoPreviewTicketPost

> apiFileVideoPreviewTicketPost(uid)



### Example

```ts
import {
  Configuration,
  FileApi,
} from '@aqlife/api-contract';
import type { ApiFileVideoPreviewTicketPostRequest } from '@aqlife/api-contract';

async function example() {
  console.log("🚀 Testing @aqlife/api-contract SDK...");
  const api = new FileApi();

  const body = {
    // string (optional)
    uid: 38400000-8cf0-11bd-b23e-10b96e4ef00d,
  } satisfies ApiFileVideoPreviewTicketPostRequest;

  try {
    const data = await api.apiFileVideoPreviewTicketPost(body);
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
| **uid** | `string` |  | [Optional] [Defaults to `undefined`] |

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


## apiFileVideoStreamGet

> apiFileVideoStreamGet(uid, ticket)



### Example

```ts
import {
  Configuration,
  FileApi,
} from '@aqlife/api-contract';
import type { ApiFileVideoStreamGetRequest } from '@aqlife/api-contract';

async function example() {
  console.log("🚀 Testing @aqlife/api-contract SDK...");
  const api = new FileApi();

  const body = {
    // string (optional)
    uid: 38400000-8cf0-11bd-b23e-10b96e4ef00d,
    // string (optional)
    ticket: ticket_example,
  } satisfies ApiFileVideoStreamGetRequest;

  try {
    const data = await api.apiFileVideoStreamGet(body);
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
| **uid** | `string` |  | [Optional] [Defaults to `undefined`] |
| **ticket** | `string` |  | [Optional] [Defaults to `undefined`] |

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

