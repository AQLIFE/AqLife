# TodoApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|------------- | ------------- | -------------|
| [**apiTodoDelete**](TodoApi.md#apitododelete) | **DELETE** /api/Todo |  |
| [**apiTodoGet**](TodoApi.md#apitodoget) | **GET** /api/Todo |  |
| [**apiTodoIdGet**](TodoApi.md#apitodoidget) | **GET** /api/Todo/{id} |  |
| [**apiTodoPatch**](TodoApi.md#apitodopatch) | **PATCH** /api/Todo |  |
| [**apiTodoPost**](TodoApi.md#apitodopost) | **POST** /api/Todo |  |



## apiTodoDelete

> boolean apiTodoDelete(guid)



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
    // string (optional)
    guid: 38400000-8cf0-11bd-b23e-10b96e4ef00d,
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
| **guid** | `string` |  | [Optional] [Defaults to `undefined`] |

### Return type

**boolean**

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


## apiTodoGet

> Array&lt;TodoDto&gt; apiTodoGet()



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

  try {
    const data = await api.apiTodoGet();
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

[**Array&lt;TodoDto&gt;**](TodoDto.md)

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


## apiTodoIdGet

> TodoDto apiTodoIdGet(id)



### Example

```ts
import {
  Configuration,
  TodoApi,
} from '@aqlife/api-contract';
import type { ApiTodoIdGetRequest } from '@aqlife/api-contract';

async function example() {
  console.log("🚀 Testing @aqlife/api-contract SDK...");
  const api = new TodoApi();

  const body = {
    // string
    id: 38400000-8cf0-11bd-b23e-10b96e4ef00d,
  } satisfies ApiTodoIdGetRequest;

  try {
    const data = await api.apiTodoIdGet(body);
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
| **id** | `string` |  | [Defaults to `undefined`] |

### Return type

[**TodoDto**](TodoDto.md)

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


## apiTodoPatch

> string apiTodoPatch(guid, status)



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
    // string (optional)
    guid: 38400000-8cf0-11bd-b23e-10b96e4ef00d,
    // string (optional)
    status: status_example,
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
| **guid** | `string` |  | [Optional] [Defaults to `undefined`] |
| **status** | `string` |  | [Optional] [Defaults to `undefined`] |

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


## apiTodoPost

> string apiTodoPost(todoForAdd)



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
    // TodoForAdd (optional)
    todoForAdd: ...,
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
| **todoForAdd** | [TodoForAdd](TodoForAdd.md) |  | [Optional] |

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

