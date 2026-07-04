
# TodoDto


## Properties

Name | Type
------------ | -------------
`ftid` | string
`desc` | string
`status` | string
`createdAt` | string
`completedAt` | string
`priority` | number

## Example

```typescript
import type { TodoDto } from '@aqlife/api-contract'

// TODO: Update the object below with actual values
const example = {
  "ftid": null,
  "desc": null,
  "status": null,
  "createdAt": null,
  "completedAt": null,
  "priority": null,
} satisfies TodoDto

console.log(example)

// Convert the instance to a JSON string
const exampleJSON: string = JSON.stringify(example)
console.log(exampleJSON)

// Parse the JSON string back to an object
const exampleParsed = JSON.parse(exampleJSON) as TodoDto
console.log(exampleParsed)
```

[[Back to top]](#) [[Back to API list]](../README.md#api-endpoints) [[Back to Model list]](../README.md#models) [[Back to README]](../README.md)


