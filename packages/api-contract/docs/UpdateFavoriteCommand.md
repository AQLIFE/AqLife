
# UpdateFavoriteCommand


## Properties

Name | Type
------------ | -------------
`uid` | string
`title` | string
`description` | string
`cid` | string
`sourceUrl` | string
`sortOrder` | number
`media` | [Array&lt;FavoriteMediaInput&gt;](FavoriteMediaInput.md)
`tagIds` | Array&lt;string&gt;

## Example

```typescript
import type { UpdateFavoriteCommand } from '@aqlife/api-contract'

// TODO: Update the object below with actual values
const example = {
  "uid": null,
  "title": null,
  "description": null,
  "cid": null,
  "sourceUrl": null,
  "sortOrder": null,
  "media": null,
  "tagIds": null,
} satisfies UpdateFavoriteCommand

console.log(example)

// Convert the instance to a JSON string
const exampleJSON: string = JSON.stringify(example)
console.log(exampleJSON)

// Parse the JSON string back to an object
const exampleParsed = JSON.parse(exampleJSON) as UpdateFavoriteCommand
console.log(exampleParsed)
```

[[Back to top]](#) [[Back to API list]](../README.md#api-endpoints) [[Back to Model list]](../README.md#models) [[Back to README]](../README.md)


