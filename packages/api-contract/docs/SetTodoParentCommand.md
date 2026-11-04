
# SetTodoParentCommand


## Properties

Name | Type
------------ | -------------
`uid` | string
`parentId` | string

## Example

```typescript
import type { SetTodoParentCommand } from '@aqlife/api-contract'

// TODO: Update the object below with actual values
const example = {
  "uid": null,
  "parentId": null,
} satisfies SetTodoParentCommand

console.log(example)

// Convert the instance to a JSON string
const exampleJSON: string = JSON.stringify(example)
console.log(exampleJSON)

// Parse the JSON string back to an object
const exampleParsed = JSON.parse(exampleJSON) as SetTodoParentCommand
console.log(exampleParsed)
```

[[Back to top]](#) [[Back to API list]](../README.md#api-endpoints) [[Back to Model list]](../README.md#models) [[Back to README]](../README.md)


