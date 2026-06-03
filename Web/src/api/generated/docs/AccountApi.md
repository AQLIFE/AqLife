# AccountApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|------------- | ------------- | -------------|
| [**apiAccountDelete**](AccountApi.md#apiaccountdelete) | **DELETE** /api/Account |  |
| [**apiAccountGet**](AccountApi.md#apiaccountget) | **GET** /api/Account |  |
| [**apiAccountLoginPost**](AccountApi.md#apiaccountloginpost) | **POST** /api/Account/login |  |
| [**apiAccountPatch**](AccountApi.md#apiaccountpatch) | **PATCH** /api/Account |  |
| [**apiAccountPost**](AccountApi.md#apiaccountpost) | **POST** /api/Account |  |



## apiAccountDelete

> number apiAccountDelete()



### Example

```ts
import {
  Configuration,
  AccountApi,
} from '';
import type { ApiAccountDeleteRequest } from '';

async function example() {
  console.log("🚀 Testing  SDK...");
  const api = new AccountApi();

  try {
    const data = await api.apiAccountDelete();
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


## apiAccountGet

> AccountDto apiAccountGet()



### Example

```ts
import {
  Configuration,
  AccountApi,
} from '';
import type { ApiAccountGetRequest } from '';

async function example() {
  console.log("🚀 Testing  SDK...");
  const api = new AccountApi();

  try {
    const data = await api.apiAccountGet();
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

[**AccountDto**](AccountDto.md)

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


## apiAccountLoginPost

> string apiAccountLoginPost(loginDto)



### Example

```ts
import {
  Configuration,
  AccountApi,
} from '';
import type { ApiAccountLoginPostRequest } from '';

async function example() {
  console.log("🚀 Testing  SDK...");
  const api = new AccountApi();

  const body = {
    // LoginDto (optional)
    loginDto: ...,
  } satisfies ApiAccountLoginPostRequest;

  try {
    const data = await api.apiAccountLoginPost(body);
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
| **loginDto** | [LoginDto](LoginDto.md) |  | [Optional] |

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


## apiAccountPatch

> string apiAccountPatch(accountDto)



### Example

```ts
import {
  Configuration,
  AccountApi,
} from '';
import type { ApiAccountPatchRequest } from '';

async function example() {
  console.log("🚀 Testing  SDK...");
  const api = new AccountApi();

  const body = {
    // AccountDto (optional)
    accountDto: ...,
  } satisfies ApiAccountPatchRequest;

  try {
    const data = await api.apiAccountPatch(body);
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
| **accountDto** | [AccountDto](AccountDto.md) |  | [Optional] |

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


## apiAccountPost

> string apiAccountPost(secretKey, accountDto)



### Example

```ts
import {
  Configuration,
  AccountApi,
} from '';
import type { ApiAccountPostRequest } from '';

async function example() {
  console.log("🚀 Testing  SDK...");
  const api = new AccountApi();

  const body = {
    // string (optional)
    secretKey: secretKey_example,
    // AccountDto (optional)
    accountDto: ...,
  } satisfies ApiAccountPostRequest;

  try {
    const data = await api.apiAccountPost(body);
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
| **secretKey** | `string` |  | [Optional] [Defaults to `undefined`] |
| **accountDto** | [AccountDto](AccountDto.md) |  | [Optional] |

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

