# AccountApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|------------- | ------------- | -------------|
| [**apiAccountGet**](AccountApi.md#apiaccountget) | **GET** /api/Account |  |
| [**apiAccountLoginPost**](AccountApi.md#apiaccountloginpost) | **POST** /api/Account/login |  |
| [**apiAccountPost**](AccountApi.md#apiaccountpost) | **POST** /api/Account |  |
| [**apiAccountSubscriptionPost**](AccountApi.md#apiaccountsubscriptionpost) | **POST** /api/Account/subscription |  |



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


## apiAccountPost

> string apiAccountPost(name, desc, secretKey)



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
    name: name_example,
    // string (optional)
    desc: desc_example,
    // string (optional)
    secretKey: secretKey_example,
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
| **name** | `string` |  | [Optional] [Defaults to `undefined`] |
| **desc** | `string` |  | [Optional] [Defaults to `undefined`] |
| **secretKey** | `string` |  | [Optional] [Defaults to `undefined`] |

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


## apiAccountSubscriptionPost

> number apiAccountSubscriptionPost(subscriptionDto)



### Example

```ts
import {
  Configuration,
  AccountApi,
} from '';
import type { ApiAccountSubscriptionPostRequest } from '';

async function example() {
  console.log("🚀 Testing  SDK...");
  const api = new AccountApi();

  const body = {
    // Array<SubscriptionDto> (optional)
    subscriptionDto: ...,
  } satisfies ApiAccountSubscriptionPostRequest;

  try {
    const data = await api.apiAccountSubscriptionPost(body);
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
| **subscriptionDto** | `Array<SubscriptionDto>` |  | [Optional] |

### Return type

**number**

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

