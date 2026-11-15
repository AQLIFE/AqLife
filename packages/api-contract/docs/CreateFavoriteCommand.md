
# CreateFavoriteCommand


## Properties

Name | Type
------------ | -------------
`title` | string
`description` | string
`cid` | string
`sourceUrl` | string
`sortOrder` | number
`media` | Array&lt;string&gt;
`tagIds` | Array&lt;string&gt;

## Example

```typescript
import type { CreateFavoriteCommand } from '@aqlife/api-contract'

// TODO: Update the object below with actual values
const example = {
  "title": null,
  "description": null,
  "cid": null,
  "sourceUrl": null,
  "sortOrder": null,
  "media": null,
  "tagIds": null,
} satisfies CreateFavoriteCommand

console.log(example)

// Convert the instance to a JSON string
const exampleJSON: string = JSON.stringify(example)
console.log(exampleJSON)

// Parse the JSON string back to an object
const exampleParsed = JSON.parse(exampleJSON) as CreateFavoriteCommand
console.log(exampleParsed)
```

[[Back to top]](#) [[Back to API list]](../README.md#api-endpoints) [[Back to Model list]](../README.md#models) [[Back to README]](../README.md)


