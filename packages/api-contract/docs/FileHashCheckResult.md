
# FileHashCheckResult


## Properties

Name | Type
------------ | -------------
`isDuplicate` | boolean
`existingFile` | [FileHashMatch](FileHashMatch.md)

## Example

```typescript
import type { FileHashCheckResult } from '@aqlife/api-contract'

// TODO: Update the object below with actual values
const example = {
  "isDuplicate": null,
  "existingFile": null,
} satisfies FileHashCheckResult

console.log(example)

// Convert the instance to a JSON string
const exampleJSON: string = JSON.stringify(example)
console.log(exampleJSON)

// Parse the JSON string back to an object
const exampleParsed = JSON.parse(exampleJSON) as FileHashCheckResult
console.log(exampleParsed)
```

[[Back to top]](#) [[Back to API list]](../README.md#api-endpoints) [[Back to Model list]](../README.md#models) [[Back to README]](../README.md)


