
# UploadStatus


## Properties

Name | Type
------------ | -------------
`uploadId` | string
`fileSize` | number
`chunkSize` | number
`chunkCount` | number
`uploadedChunks` | Array&lt;number&gt;

## Example

```typescript
import type { UploadStatus } from '@aqlife/api-contract'

// TODO: Update the object below with actual values
const example = {
  "uploadId": null,
  "fileSize": null,
  "chunkSize": null,
  "chunkCount": null,
  "uploadedChunks": null,
} satisfies UploadStatus

console.log(example)

// Convert the instance to a JSON string
const exampleJSON: string = JSON.stringify(example)
console.log(exampleJSON)

// Parse the JSON string back to an object
const exampleParsed = JSON.parse(exampleJSON) as UploadStatus
console.log(exampleParsed)
```

[[Back to top]](#) [[Back to API list]](../README.md#api-endpoints) [[Back to Model list]](../README.md#models) [[Back to README]](../README.md)


