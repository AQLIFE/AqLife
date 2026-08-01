
# FileDto


## Properties

Name | Type
------------ | -------------
`uid` | string
`fileName` | string
`tags` | [Array&lt;TagDto&gt;](TagDto.md)
`fileSize` | number
`fileHash` | string
`uploadTime` | string
`fileType` | string
`fileIntroduction` | string

## Example

```typescript
import type { FileDto } from '@aqlife/api-contract'

// TODO: Update the object below with actual values
const example = {
  "uid": null,
  "fileName": null,
  "tags": null,
  "fileSize": null,
  "fileHash": null,
  "uploadTime": null,
  "fileType": null,
  "fileIntroduction": null,
} satisfies FileDto

console.log(example)

// Convert the instance to a JSON string
const exampleJSON: string = JSON.stringify(example)
console.log(exampleJSON)

// Parse the JSON string back to an object
const exampleParsed = JSON.parse(exampleJSON) as FileDto
console.log(exampleParsed)
```

[[Back to top]](#) [[Back to API list]](../README.md#api-endpoints) [[Back to Model list]](../README.md#models) [[Back to README]](../README.md)


