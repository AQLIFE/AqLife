
# UpdateTodoCommand


## Properties

Name | Type
------------ | -------------
`uid` | string
`desc` | string
`status` | [TodoStatus](TodoStatus.md)
`priority` | number

## Example

```typescript
import type { UpdateTodoCommand } from '@aqlife/api-contract'

// TODO: Update the object below with actual values
const example = {
  "uid": null,
  "desc": null,
  "status": null,
  "priority": null,
} satisfies UpdateTodoCommand

console.log(example)

// Convert the instance to a JSON string
const exampleJSON: string = JSON.stringify(example)
console.log(exampleJSON)

// Parse the JSON string back to an object
const exampleParsed = JSON.parse(exampleJSON) as UpdateTodoCommand
console.log(exampleParsed)
```

[[Back to top]](#) [[Back to API list]](../README.md#api-endpoints) [[Back to Model list]](../README.md#models) [[Back to README]](../README.md)


