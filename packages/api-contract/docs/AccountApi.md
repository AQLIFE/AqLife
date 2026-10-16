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



发送一个图像用以更新账户头像,返回 Guid

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



删除指定ID的账户,无返回

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



返回一个有效的账户,可能为 null

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



登录账户成功后返回 JWT Beearer Token

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



注册账户成功后返回唯一登录账户名

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



更新账户信息(包含公开名称,BIO);返回账户ID

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



更新账户个人主页列表,返回账户ID

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

