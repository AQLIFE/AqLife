# AccountApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|------------- | ------------- | -------------|
| [**apiAccountAvatarPatch**](AccountApi.md#apiaccountavatarpatch) | **PATCH** /api/Account/avatar |  |
| [**apiAccountDelete**](AccountApi.md#apiaccountdelete) | **DELETE** /api/Account |  |
| [**apiAccountGet**](AccountApi.md#apiaccountget) | **GET** /api/Account |  |
| [**apiAccountLoginPost**](AccountApi.md#apiaccountloginpost) | **POST** /api/Account/login |  |
| [**apiAccountPost**](AccountApi.md#apiaccountpost) | **POST** /api/Account |  |
| [**apiAccountProfilePatch**](AccountApi.md#apiaccountprofilepatch) | **PATCH** /api/Account/profile |  |
| [**apiAccountSubscriptionsPut**](AccountApi.md#apiaccountsubscriptionsput) | **PUT** /api/Account/subscriptions |  |



## apiAccountAvatarPatch

> string apiAccountAvatarPatch(avatar)



### Example

```ts
import {
  Configuration,
  AccountApi,
} from '@aqlife/api-contract';
import type { ApiAccountAvatarPatchRequest } from '@aqlife/api-contract';

async function example() {
  console.log("🚀 Testing @aqlife/api-contract SDK...");
  const api = new AccountApi();

  const body = {
    // Blob (optional)
    avatar: BINARY_DATA_HERE,
  } satisfies ApiAccountAvatarPatchRequest;

  try {
    const data = await api.apiAccountAvatarPatch(body);
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
| **avatar** | `Blob` |  | [Optional] [Defaults to `undefined`] |

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


## apiAccountDelete

> apiAccountDelete()



### Example

```ts
import {
  Configuration,
  AccountApi,
} from '@aqlife/api-contract';
import type { ApiAccountDeleteRequest } from '@aqlife/api-contract';

async function example() {
  console.log("🚀 Testing @aqlife/api-contract SDK...");
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


## apiAccountGet

> AccountDto apiAccountGet()



### Example

```ts
import {
  Configuration,
  AccountApi,
} from '@aqlife/api-contract';
import type { ApiAccountGetRequest } from '@aqlife/api-contract';

async function example() {
  console.log("🚀 Testing @aqlife/api-contract SDK...");
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

> string apiAccountLoginPost(loginCommand)



### Example

```ts
import {
  Configuration,
  AccountApi,
} from '@aqlife/api-contract';
import type { ApiAccountLoginPostRequest } from '@aqlife/api-contract';

async function example() {
  console.log("🚀 Testing @aqlife/api-contract SDK...");
  const api = new AccountApi();

  const body = {
    // LoginCommand (optional)
    loginCommand: ...,
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
| **loginCommand** | [LoginCommand](LoginCommand.md) |  | [Optional] |

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

> string apiAccountPost(createAccountCommand)



### Example

```ts
import {
  Configuration,
  AccountApi,
} from '@aqlife/api-contract';
import type { ApiAccountPostRequest } from '@aqlife/api-contract';

async function example() {
  console.log("🚀 Testing @aqlife/api-contract SDK...");
  const api = new AccountApi();

  const body = {
    // CreateAccountCommand (optional)
    createAccountCommand: ...,
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
| **createAccountCommand** | [CreateAccountCommand](CreateAccountCommand.md) |  | [Optional] |

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


## apiAccountProfilePatch

> string apiAccountProfilePatch(accountProfile)



### Example

```ts
import {
  Configuration,
  AccountApi,
} from '@aqlife/api-contract';
import type { ApiAccountProfilePatchRequest } from '@aqlife/api-contract';

async function example() {
  console.log("🚀 Testing @aqlife/api-contract SDK...");
  const api = new AccountApi();

  const body = {
    // AccountProfile (optional)
    accountProfile: ...,
  } satisfies ApiAccountProfilePatchRequest;

  try {
    const data = await api.apiAccountProfilePatch(body);
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
| **accountProfile** | [AccountProfile](AccountProfile.md) |  | [Optional] |

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


## apiAccountSubscriptionsPut

> string apiAccountSubscriptionsPut(subscriptionDto)



### Example

```ts
import {
  Configuration,
  AccountApi,
} from '@aqlife/api-contract';
import type { ApiAccountSubscriptionsPutRequest } from '@aqlife/api-contract';

async function example() {
  console.log("🚀 Testing @aqlife/api-contract SDK...");
  const api = new AccountApi();

  const body = {
    // Array<SubscriptionDto> (optional)
    subscriptionDto: ...,
  } satisfies ApiAccountSubscriptionsPutRequest;

  try {
    const data = await api.apiAccountSubscriptionsPut(body);
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

