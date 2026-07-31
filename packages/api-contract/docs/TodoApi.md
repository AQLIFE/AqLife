# TodoApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|------------- | ------------- | -------------|
| [**apiTodoDelete**](TodoApi.md#apitododelete) | **DELETE** /api/Todo |  |
| [**apiTodoGet**](TodoApi.md#apitodoget) | **GET** /api/Todo |  |
| [**apiTodoPatch**](TodoApi.md#apitodopatch) | **PATCH** /api/Todo |  |
| [**apiTodoPost**](TodoApi.md#apitodopost) | **POST** /api/Todo |  |
| [**apiTodoSearchGet**](TodoApi.md#apitodosearchget) | **GET** /api/Todo/search |  |



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


## apiTodoSearchGet

> Array&lt;TodoDto&gt; apiTodoSearchGet(uID, desc, isTree)



### Example

```ts
import {
  Configuration,
  TodoApi,
} from '@aqlife/api-contract';
import type { ApiTodoSearchGetRequest } from '@aqlife/api-contract';

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
  } satisfies ApiTodoSearchGetRequest;

  try {
    const data = await api.apiTodoSearchGet(body);
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

