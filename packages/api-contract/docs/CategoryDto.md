
# CategoryDto


## Properties

Name | Type
------------ | -------------
`uid` | string
`categoryName` | string
`categoryDescription` | string
`count` | number
`parentId` | string
`resultIds` | Array&lt;string&gt;

## Example

```typescript
import type { CategoryDto } from '@aqlife/api-contract'

// TODO: Update the object below with actual values
const example = {
  "uid": null,
  "categoryName": null,
  "categoryDescription": null,
  "count": null,
  "parentId": null,
  "resultIds": null,
} satisfies CategoryDto

console.log(example)

// Convert the instance to a JSON string
const exampleJSON: string = JSON.stringify(example)
console.log(exampleJSON)

// Parse the JSON string back to an object
const exampleParsed = JSON.parse(exampleJSON) as CategoryDto
console.log(exampleParsed)
```

[[Back to top]](#) [[Back to API list]](../README.md#api-endpoints) [[Back to Model list]](../README.md#models) [[Back to README]](../README.md)


