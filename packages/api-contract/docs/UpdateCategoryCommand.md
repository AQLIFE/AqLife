
# UpdateCategoryCommand


## Properties

Name | Type
------------ | -------------
`uid` | string
`name` | string
`desc` | string
`parentId` | string

## Example

```typescript
import type { UpdateCategoryCommand } from '@aqlife/api-contract'

// TODO: Update the object below with actual values
const example = {
  "uid": null,
  "name": null,
  "desc": null,
  "parentId": null,
} satisfies UpdateCategoryCommand

console.log(example)

// Convert the instance to a JSON string
const exampleJSON: string = JSON.stringify(example)
console.log(exampleJSON)

// Parse the JSON string back to an object
const exampleParsed = JSON.parse(exampleJSON) as UpdateCategoryCommand
console.log(exampleParsed)
```

[[Back to top]](#) [[Back to API list]](../README.md#api-endpoints) [[Back to Model list]](../README.md#models) [[Back to README]](../README.md)


