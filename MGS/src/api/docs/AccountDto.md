
# AccountDto


## Properties

Name | Type
------------ | -------------
`name` | string
`desc` | string
`avatar` | string
`subscriptions` | [Array&lt;SubscriptionDto&gt;](SubscriptionDto.md)

## Example

```typescript
import type { AccountDto } from ''

// TODO: Update the object below with actual values
const example = {
  "name": null,
  "desc": null,
  "avatar": null,
  "subscriptions": null,
} satisfies AccountDto

console.log(example)

// Convert the instance to a JSON string
const exampleJSON: string = JSON.stringify(example)
console.log(exampleJSON)

// Parse the JSON string back to an object
const exampleParsed = JSON.parse(exampleJSON) as AccountDto
console.log(exampleParsed)
```

[[Back to top]](#) [[Back to API list]](../README.md#api-endpoints) [[Back to Model list]](../README.md#models) [[Back to README]](../README.md)


