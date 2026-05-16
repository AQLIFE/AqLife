
# FileMetaEntity


## Properties

Name | Type
------------ | -------------
`uuid` | string
`fileName` | string
`desensitizationName` | string
`fileSize` | number
`fileHash` | string
`uploadTime` | Date

## Example

```typescript
import type { FileMetaEntity } from ''

// TODO: Update the object below with actual values
const example = {
  "uuid": null,
  "fileName": null,
  "desensitizationName": null,
  "fileSize": null,
  "fileHash": null,
  "uploadTime": null,
} satisfies FileMetaEntity

console.log(example)

// Convert the instance to a JSON string
const exampleJSON: string = JSON.stringify(example)
console.log(exampleJSON)

// Parse the JSON string back to an object
const exampleParsed = JSON.parse(exampleJSON) as FileMetaEntity
console.log(exampleParsed)
```

[[Back to top]](#) [[Back to API list]](../README.md#api-endpoints) [[Back to Model list]](../README.md#models) [[Back to README]](../README.md)


