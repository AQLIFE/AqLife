
# LoginCommand


## Properties

Name | Type
------------ | -------------
`accountName` | string
`secretKey` | string

## Example

```typescript
import type { LoginCommand } from '@aqlife/api-contract'

// TODO: Update the object below with actual values
const example = {
  "accountName": null,
  "secretKey": null,
} satisfies LoginCommand

console.log(example)

// Convert the instance to a JSON string
const exampleJSON: string = JSON.stringify(example)
console.log(exampleJSON)

// Parse the JSON string back to an object
const exampleParsed = JSON.parse(exampleJSON) as LoginCommand
console.log(exampleParsed)
```

[[Back to top]](#) [[Back to API list]](../README.md#api-endpoints) [[Back to Model list]](../README.md#models) [[Back to README]](../README.md)


