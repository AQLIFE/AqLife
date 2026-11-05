# TodoApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|------------- | ------------- | -------------|
| [**apiTodoDelete**](TodoApi.md#apitododelete) | **DELETE** /api/Todo |  |
| [**apiTodoDependenciesDelete**](TodoApi.md#apitododependenciesdelete) | **DELETE** /api/Todo/dependencies |  |
| [**apiTodoDependenciesPost**](TodoApi.md#apitododependenciespost) | **POST** /api/Todo/dependencies |  |
| [**apiTodoGet**](TodoApi.md#apitodoget) | **GET** /api/Todo |  |
| [**apiTodoParentPatch**](TodoApi.md#apitodoparentpatch) | **PATCH** /api/Todo/parent |  |
| [**apiTodoPatch**](TodoApi.md#apitodopatch) | **PATCH** /api/Todo |  |
| [**apiTodoPost**](TodoApi.md#apitodopost) | **POST** /api/Todo |  |
| [**apiTodoRoadmapGet**](TodoApi.md#apitodoroadmapget) | **GET** /api/Todo/roadmap |  |
| [**apiTodoRoadmapLayoutPut**](TodoApi.md#apitodoroadmaplayoutput) | **PUT** /api/Todo/roadmap/layout |  |
| [**apiTodoSortOrderPatch**](TodoApi.md#apitodosortorderpatch) | **PATCH** /api/Todo/sort-order |  |
| [**apiTodoUidArchivePost**](TodoApi.md#apitodouidarchivepost) | **POST** /api/Todo/{uid}/archive |  |
| [**apiTodoUidRestorePost**](TodoApi.md#apitodouidrestorepost) | **POST** /api/Todo/{uid}/restore |  |



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

> string apiTodoDependenciesDelete(removeTodoDependencyCommand)



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


## apiTodoGet

> TodoDtoPageResult apiTodoGet(uID, desc, isTree, page, pageSize, planId, includeCompleted, includeArchived, parentId)



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
    desc: desc_example,
    // boolean (optional)
    isTree: true,
    // number (optional)
    page: 56,
    // number (optional)
    pageSize: 56,
    // string (optional)
    planId: 38400000-8cf0-11bd-b23e-10b96e4ef00d,
    // boolean (optional)
    includeCompleted: true,
    // boolean (optional)
    includeArchived: true,
    // string (optional)
    parentId: 38400000-8cf0-11bd-b23e-10b96e4ef00d,
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
| **desc** | `string` |  | [Optional] [Defaults to `undefined`] |
| **isTree** | `boolean` |  | [Optional] [Defaults to `undefined`] |
| **page** | `number` |  | [Optional] [Defaults to `undefined`] |
| **pageSize** | `number` |  | [Optional] [Defaults to `undefined`] |
| **planId** | `string` |  | [Optional] [Defaults to `undefined`] |
| **includeCompleted** | `boolean` |  | [Optional] [Defaults to `undefined`] |
| **includeArchived** | `boolean` |  | [Optional] [Defaults to `undefined`] |
| **parentId** | `string` |  | [Optional] [Defaults to `undefined`] |

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


## apiTodoParentPatch

> string apiTodoParentPatch(setTodoParentCommand)



### Example

```ts
import {
  Configuration,
  TodoApi,
} from '@aqlife/api-contract';
import type { ApiTodoParentPatchRequest } from '@aqlife/api-contract';

async function example() {
  console.log("🚀 Testing @aqlife/api-contract SDK...");
  const api = new TodoApi();

  const body = {
    // SetTodoParentCommand (optional)
    setTodoParentCommand: ...,
  } satisfies ApiTodoParentPatchRequest;

  try {
    const data = await api.apiTodoParentPatch(body);
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
| **setTodoParentCommand** | [SetTodoParentCommand](SetTodoParentCommand.md) |  | [Optional] |

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


## apiTodoPatch

> string apiTodoPatch(updateTodoCommand)



### Example

```ts
import {
  Configuration,
  TodoApi,
} from '@aqlife/api-contract';
import type { ApiTodoPatchRequest } from '@aqlife/api-contract';

async function example() {
  console.log("🚀 Testing @aqlife/api-contract SDK...");
  const api = new TodoApi();

  const body = {
    // UpdateTodoCommand (optional)
    updateTodoCommand: ...,
  } satisfies ApiTodoPatchRequest;

  try {
    const data = await api.apiTodoPatch(body);
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


## apiTodoRoadmapGet

> TodoRoadmapDto apiTodoRoadmapGet(planId)



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


## apiTodoSortOrderPatch

> string apiTodoSortOrderPatch(setTodoSortOrderCommand)



### Example

```ts
import {
  Configuration,
  TodoApi,
} from '@aqlife/api-contract';
import type { ApiTodoSortOrderPatchRequest } from '@aqlife/api-contract';

async function example() {
  console.log("🚀 Testing @aqlife/api-contract SDK...");
  const api = new TodoApi();

  const body = {
    // SetTodoSortOrderCommand (optional)
    setTodoSortOrderCommand: ...,
  } satisfies ApiTodoSortOrderPatchRequest;

  try {
    const data = await api.apiTodoSortOrderPatch(body);
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
| **setTodoSortOrderCommand** | [SetTodoSortOrderCommand](SetTodoSortOrderCommand.md) |  | [Optional] |

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


## apiTodoUidArchivePost

> string apiTodoUidArchivePost(uid)



### Example

```ts
import {
  Configuration,
  TodoApi,
} from '@aqlife/api-contract';
import type { ApiTodoUidArchivePostRequest } from '@aqlife/api-contract';

async function example() {
  console.log("🚀 Testing @aqlife/api-contract SDK...");
  const api = new TodoApi();

  const body = {
    // string
    uid: 38400000-8cf0-11bd-b23e-10b96e4ef00d,
  } satisfies ApiTodoUidArchivePostRequest;

  try {
    const data = await api.apiTodoUidArchivePost(body);
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


## apiTodoUidRestorePost

> string apiTodoUidRestorePost(uid)



### Example

```ts
import {
  Configuration,
  TodoApi,
} from '@aqlife/api-contract';
import type { ApiTodoUidRestorePostRequest } from '@aqlife/api-contract';

async function example() {
  console.log("🚀 Testing @aqlife/api-contract SDK...");
  const api = new TodoApi();

  const body = {
    // string
    uid: 38400000-8cf0-11bd-b23e-10b96e4ef00d,
  } satisfies ApiTodoUidRestorePostRequest;

  try {
    const data = await api.apiTodoUidRestorePost(body);
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

