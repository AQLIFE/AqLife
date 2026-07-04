
# SubscriptionFullDto


## Properties

Name | Type
------------ | -------------
`aliasName` | string
`subscriptionLink` | string
`subscriptionPlatform` | string
`subscriptionIcon` | string
`newIconFile` | Blob

## Example

```typescript
import type { SubscriptionFullDto } from '@aqlife/api-contract'

// TODO: Update the object below with actual values
const example = {
  "aliasName": null,
  "subscriptionLink": null,
  "subscriptionPlatform": null,
  "subscriptionIcon": null,
  "newIconFile": null,
} satisfies SubscriptionFullDto

console.log(example)

// Convert the instance to a JSON string
const exampleJSON: string = JSON.stringify(example)
console.log(exampleJSON)

// Parse the JSON string back to an object
const exampleParsed = JSON.parse(exampleJSON) as SubscriptionFullDto
console.log(exampleParsed)
```

[[Back to top]](#) [[Back to API list]](../README.md#api-endpoints) [[Back to Model list]](../README.md#models) [[Back to README]](../README.md)


