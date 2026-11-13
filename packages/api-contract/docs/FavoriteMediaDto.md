
# FavoriteMediaDto


## Properties

Name | Type
------------ | -------------
`favoriteId` | string
`fileId` | string
`role` | string
`sortOrder` | number

## Example

```typescript
import type { FavoriteMediaDto } from '@aqlife/api-contract'

// TODO: Update the object below with actual values
const example = {
  "favoriteId": null,
  "fileId": null,
  "role": null,
  "sortOrder": null,
} satisfies FavoriteMediaDto

console.log(example)

// Convert the instance to a JSON string
const exampleJSON: string = JSON.stringify(example)
console.log(exampleJSON)

// Parse the JSON string back to an object
const exampleParsed = JSON.parse(exampleJSON) as FavoriteMediaDto
console.log(exampleParsed)
```

[[Back to top]](#) [[Back to API list]](../README.md#api-endpoints) [[Back to Model list]](../README.md#models) [[Back to README]](../README.md)


