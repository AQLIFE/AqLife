
# UploadSession


## Properties

Name | Type
------------ | -------------
`uploadId` | string
`fileName` | string
`fileSize` | number
`chunkSize` | number
`chunkCount` | number
`createdAt` | string

## Example

```typescript
import type { UploadSession } from '@aqlife/api-contract'

// TODO: Update the object below with actual values
const example = {
  "uploadId": null,
  "fileName": null,
  "fileSize": null,
  "chunkSize": null,
  "chunkCount": null,
  "createdAt": null,
} satisfies UploadSession

console.log(example)

// Convert the instance to a JSON string
const exampleJSON: string = JSON.stringify(example)
console.log(exampleJSON)

// Parse the JSON string back to an object
const exampleParsed = JSON.parse(exampleJSON) as UploadSession
console.log(exampleParsed)
```

[[Back to top]](#) [[Back to API list]](../README.md#api-endpoints) [[Back to Model list]](../README.md#models) [[Back to README]](../README.md)


