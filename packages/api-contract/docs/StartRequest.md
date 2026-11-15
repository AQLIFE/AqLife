
# StartRequest


## Properties

Name | Type
------------ | -------------
`fileName` | string
`fileSize` | number
`chunkSize` | number

## Example

```typescript
import type { StartRequest } from '@aqlife/api-contract'

// TODO: Update the object below with actual values
const example = {
  "fileName": null,
  "fileSize": null,
  "chunkSize": null,
} satisfies StartRequest

console.log(example)

// Convert the instance to a JSON string
const exampleJSON: string = JSON.stringify(example)
console.log(exampleJSON)

// Parse the JSON string back to an object
const exampleParsed = JSON.parse(exampleJSON) as StartRequest
console.log(exampleParsed)
```

[[Back to top]](#) [[Back to API list]](../README.md#api-endpoints) [[Back to Model list]](../README.md#models) [[Back to README]](../README.md)


