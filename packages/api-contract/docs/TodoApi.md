# TodoApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|------------- | ------------- | -------------|
| [**apiTodoArchivePatch**](TodoApi.md#apitodoarchivepatch) | **PATCH** /api/Todo/archive |  |
| [**apiTodoDelete**](TodoApi.md#apitododelete) | **DELETE** /api/Todo |  |
| [**apiTodoDependenciesDelete**](TodoApi.md#apitododependenciesdelete) | **DELETE** /api/Todo/dependencies |  |
| [**apiTodoDependenciesPost**](TodoApi.md#apitododependenciespost) | **POST** /api/Todo/dependencies |  |
| [**apiTodoDetailsPatch**](TodoApi.md#apitododetailspatch) | **PATCH** /api/Todo/details |  |
| [**apiTodoGet**](TodoApi.md#apitodoget) | **GET** /api/Todo |  |
| [**apiTodoPost**](TodoApi.md#apitodopost) | **POST** /api/Todo |  |
| [**apiTodoRestorePatch**](TodoApi.md#apitodorestorepatch) | **PATCH** /api/Todo/restore |  |
| [**apiTodoRoadmapGet**](TodoApi.md#apitodoroadmapget) | **GET** /api/Todo/roadmap |  |
| [**apiTodoRoadmapLayoutPut**](TodoApi.md#apitodoroadmaplayoutput) | **PUT** /api/Todo/roadmap/layout |  |
| [**apiTodoStatusPatch**](TodoApi.md#apitodostatuspatch) | **PATCH** /api/Todo/status |  |



## apiTodoArchivePatch

> string apiTodoArchivePatch(archiveTodoCommand)



### Example

```ts
import {
  Configuration,
  TodoApi,
} from '@aqlife/api-contract';
import type { ApiTodoArchivePatchRequest } from '@aqlife/api-contract';

async function example() {
  console.log("🚀 Testing @aqlife/api-contract SDK...");
  const api = new TodoApi();

  const body = {
    // ArchiveTodoCommand (optional)
    archiveTodoCommand: ...,
  } satisfies ApiTodoArchivePatchRequest;

  try {
    const data = await api.apiTodoArchivePatch(body);
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
| **archiveTodoCommand** | [ArchiveTodoCommand](ArchiveTodoCommand.md) |  | [Optional] |

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


## apiTodoDelete

> apiTodoDelete(deleteTodoCommand)



### Example

```ts
import {
  Configuration,
  TodoApi,
} from '@aqlife/api-contract';
import type { ApiTodoDeleteRequest } from '@aqlife/api-contract';

async function example() {
  console.log("🚀 Testing @aqlife/api-contract SDK...");
  const api = new TodoApi();

  const body = {
    // DeleteTodoCommand (optional)
    deleteTodoCommand: ...,
  } satisfies ApiTodoDeleteRequest;

  try {
    const data = await api.apiTodoDelete(body);
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
| **deleteTodoCommand** | [DeleteTodoCommand](DeleteTodoCommand.md) |  | [Optional] |

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


## apiTodoDependenciesDelete

> apiTodoDependenciesDelete(removeTodoDependencyCommand)



### Example

```ts
import {
  Configuration,
  TodoApi,
} from '@aqlife/api-contract';
import type { ApiTodoDependenciesDeleteRequest } from '@aqlife/api-contract';

async function example() {
  console.log("🚀 Testing @aqlife/api-contract SDK...");
  const api = new TodoApi();

  const body = {
    // RemoveTodoDependencyCommand (optional)
    removeTodoDependencyCommand: ...,
  } satisfies ApiTodoDependenciesDeleteRequest;

  try {
    const data = await api.apiTodoDependenciesDelete(body);
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
| **removeTodoDependencyCommand** | [RemoveTodoDependencyCommand](RemoveTodoDependencyCommand.md) |  | [Optional] |

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


## apiTodoDependenciesPost

> string apiTodoDependenciesPost(addTodoDependencyCommand)



### Example

```ts
import {
  Configuration,
  TodoApi,
} from '@aqlife/api-contract';
import type { ApiTodoDependenciesPostRequest } from '@aqlife/api-contract';

async function example() {
  console.log("🚀 Testing @aqlife/api-contract SDK...");
  const api = new TodoApi();

  const body = {
    // AddTodoDependencyCommand (optional)
    addTodoDependencyCommand: ...,
  } satisfies ApiTodoDependenciesPostRequest;

  try {
    const data = await api.apiTodoDependenciesPost(body);
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
| **addTodoDependencyCommand** | [AddTodoDependencyCommand](AddTodoDependencyCommand.md) |  | [Optional] |

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


## apiTodoDetailsPatch

> string apiTodoDetailsPatch(updateTodoCommand)



### Example

```ts
import {
  Configuration,
  TodoApi,
} from '@aqlife/api-contract';
import type { ApiTodoDetailsPatchRequest } from '@aqlife/api-contract';

async function example() {
  console.log("🚀 Testing @aqlife/api-contract SDK...");
  const api = new TodoApi();

  const body = {
    // UpdateTodoCommand (optional)
    updateTodoCommand: ...,
  } satisfies ApiTodoDetailsPatchRequest;

  try {
    const data = await api.apiTodoDetailsPatch(body);
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
| **updateTodoCommand** | [UpdateTodoCommand](UpdateTodoCommand.md) |  | [Optional] |

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


## apiTodoGet

> TodoDtoPageResult apiTodoGet(uID, title, planId, page, pageSize)



### Example

```ts
import {
  Configuration,
  TodoApi,
} from '@aqlife/api-contract';
import type { ApiTodoGetRequest } from '@aqlife/api-contract';

async function example() {
  console.log("🚀 Testing @aqlife/api-contract SDK...");
  const api = new TodoApi();

  const body = {
    // string (optional)
    uID: 38400000-8cf0-11bd-b23e-10b96e4ef00d,
    // string (optional)
    title: title_example,
    // string (optional)
    planId: 38400000-8cf0-11bd-b23e-10b96e4ef00d,
    // number (optional)
    page: 56,
    // number (optional)
    pageSize: 56,
  } satisfies ApiTodoGetRequest;

  try {
    const data = await api.apiTodoGet(body);
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
| **planId** | `string` |  | [Optional] [Defaults to `undefined`] |
| **page** | `number` |  | [Optional] [Defaults to `undefined`] |
| **pageSize** | `number` |  | [Optional] [Defaults to `undefined`] |

### Return type

[**TodoDtoPageResult**](TodoDtoPageResult.md)

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


## apiTodoPost

> string apiTodoPost(createTodoCommand)



### Example

```ts
import {
  Configuration,
  TodoApi,
} from '@aqlife/api-contract';
import type { ApiTodoPostRequest } from '@aqlife/api-contract';

async function example() {
  console.log("🚀 Testing @aqlife/api-contract SDK...");
  const api = new TodoApi();

  const body = {
    // CreateTodoCommand (optional)
    createTodoCommand: ...,
  } satisfies ApiTodoPostRequest;

  try {
    const data = await api.apiTodoPost(body);
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
| **createTodoCommand** | [CreateTodoCommand](CreateTodoCommand.md) |  | [Optional] |

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


## apiTodoRestorePatch

> string apiTodoRestorePatch(restoreTodoCommand)



### Example

```ts
import {
  Configuration,
  TodoApi,
} from '@aqlife/api-contract';
import type { ApiTodoRestorePatchRequest } from '@aqlife/api-contract';

async function example() {
  console.log("🚀 Testing @aqlife/api-contract SDK...");
  const api = new TodoApi();

  const body = {
    // RestoreTodoCommand (optional)
    restoreTodoCommand: ...,
  } satisfies ApiTodoRestorePatchRequest;

  try {
    const data = await api.apiTodoRestorePatch(body);
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
| **restoreTodoCommand** | [RestoreTodoCommand](RestoreTodoCommand.md) |  | [Optional] |

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


## apiTodoRoadmapGet

> TodoRoadmapDto apiTodoRoadmapGet(planId, uID)



### Example

```ts
import {
  Configuration,
  TodoApi,
} from '@aqlife/api-contract';
import type { ApiTodoRoadmapGetRequest } from '@aqlife/api-contract';

async function example() {
  console.log("🚀 Testing @aqlife/api-contract SDK...");
  const api = new TodoApi();

  const body = {
    // string (optional)
    planId: 38400000-8cf0-11bd-b23e-10b96e4ef00d,
    // string (optional)
    uID: 38400000-8cf0-11bd-b23e-10b96e4ef00d,
  } satisfies ApiTodoRoadmapGetRequest;

  try {
    const data = await api.apiTodoRoadmapGet(body);
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
| **planId** | `string` |  | [Optional] [Defaults to `undefined`] |
| **uID** | `string` |  | [Optional] [Defaults to `undefined`] |

### Return type

[**TodoRoadmapDto**](TodoRoadmapDto.md)

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


## apiTodoRoadmapLayoutPut

> string apiTodoRoadmapLayoutPut(saveRoadmapNodeLayoutCommand)



### Example

```ts
import {
  Configuration,
  TodoApi,
} from '@aqlife/api-contract';
import type { ApiTodoRoadmapLayoutPutRequest } from '@aqlife/api-contract';

async function example() {
  console.log("🚀 Testing @aqlife/api-contract SDK...");
  const api = new TodoApi();

  const body = {
    // SaveRoadmapNodeLayoutCommand (optional)
    saveRoadmapNodeLayoutCommand: ...,
  } satisfies ApiTodoRoadmapLayoutPutRequest;

  try {
    const data = await api.apiTodoRoadmapLayoutPut(body);
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
| **saveRoadmapNodeLayoutCommand** | [SaveRoadmapNodeLayoutCommand](SaveRoadmapNodeLayoutCommand.md) |  | [Optional] |

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


## apiTodoStatusPatch

> string apiTodoStatusPatch(updateTodoStatusCommand)



### Example

```ts
import {
  Configuration,
  TodoApi,
} from '@aqlife/api-contract';
import type { ApiTodoStatusPatchRequest } from '@aqlife/api-contract';

async function example() {
  console.log("🚀 Testing @aqlife/api-contract SDK...");
  const api = new TodoApi();

  const body = {
    // UpdateTodoStatusCommand (optional)
    updateTodoStatusCommand: ...,
  } satisfies ApiTodoStatusPatchRequest;

  try {
    const data = await api.apiTodoStatusPatch(body);
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
| **updateTodoStatusCommand** | [UpdateTodoStatusCommand](UpdateTodoStatusCommand.md) |  | [Optional] |

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

