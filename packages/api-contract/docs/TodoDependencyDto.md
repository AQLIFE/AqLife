
# TodoDependencyDto


## Properties

Name | Type
------------ | -------------
`uid` | string
`predecessorId` | string
`successorId` | string
`createdAt` | string

## Example

```typescript
import type { TodoDependencyDto } from '@aqlife/api-contract'

// TODO: Update the object below with actual values
const example = {
  "uid": null,
  "predecessorId": null,
  "successorId": null,
  "createdAt": null,
} satisfies TodoDependencyDto

console.log(example)

// Convert the instance to a JSON string
const exampleJSON: string = JSON.stringify(example)
console.log(exampleJSON)

// Parse the JSON string back to an object
const exampleParsed = JSON.parse(exampleJSON) as TodoDependencyDto
console.log(exampleParsed)
```

[[Back to top]](#) [[Back to API list]](../README.md#api-endpoints) [[Back to Model list]](../README.md#models) [[Back to README]](../README.md)


