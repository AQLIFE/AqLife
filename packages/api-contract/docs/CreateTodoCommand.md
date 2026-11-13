
# CreateTodoCommand


## Properties

Name | Type
------------ | -------------
`planId` | string
`title` | string
`description` | string
`sortOrder` | number

## Example

```typescript
import type { CreateTodoCommand } from '@aqlife/api-contract'

// TODO: Update the object below with actual values
const example = {
  "planId": null,
  "title": null,
  "description": null,
  "sortOrder": null,
} satisfies CreateTodoCommand

console.log(example)

// Convert the instance to a JSON string
const exampleJSON: string = JSON.stringify(example)
console.log(exampleJSON)

// Parse the JSON string back to an object
const exampleParsed = JSON.parse(exampleJSON) as CreateTodoCommand
console.log(exampleParsed)
```

[[Back to top]](#) [[Back to API list]](../README.md#api-endpoints) [[Back to Model list]](../README.md#models) [[Back to README]](../README.md)


