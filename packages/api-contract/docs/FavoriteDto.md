
# FavoriteDto


## Properties

Name | Type
------------ | -------------
`uid` | string
`title` | string
`description` | string
`contentType` | string
`sourceUrl` | string
`sortOrder` | number
`isPublished` | boolean
`createdAt` | string
`updatedAt` | string
`media` | [Array&lt;FavoriteMediaDto&gt;](FavoriteMediaDto.md)
`tagIds` | Array&lt;string&gt;

## Example

```typescript
import type { FavoriteDto } from '@aqlife/api-contract'

// TODO: Update the object below with actual values
const example = {
  "uid": null,
  "title": null,
  "description": null,
  "contentType": null,
  "sourceUrl": null,
  "sortOrder": null,
  "isPublished": null,
  "createdAt": null,
  "updatedAt": null,
  "media": null,
  "tagIds": null,
} satisfies FavoriteDto

console.log(example)

// Convert the instance to a JSON string
const exampleJSON: string = JSON.stringify(example)
console.log(exampleJSON)

// Parse the JSON string back to an object
const exampleParsed = JSON.parse(exampleJSON) as FavoriteDto
console.log(exampleParsed)
```

[[Back to top]](#) [[Back to API list]](../README.md#api-endpoints) [[Back to Model list]](../README.md#models) [[Back to README]](../README.md)


