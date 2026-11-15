
# TodoDto


## Properties

Name | Type
------------ | -------------
`uid` | string
`planId` | string
`title` | string
`status` | string
`createdAt` | string
`updatedAt` | string
`completedAt` | string
`sortOrder` | number
`predecessorIds` | Array&lt;string&gt;
`incompleteDependencyCount` | number
`description` | string
`isBlocked` | boolean
`isArchived` | boolean

## Example

```typescript
import type { TodoDto } from '@aqlife/api-contract'

// TODO: Update the object below with actual values
const example = {
  "uid": null,
  "planId": null,
  "title": null,
  "status": null,
  "createdAt": null,
  "updatedAt": null,
  "completedAt": null,
  "sortOrder": null,
  "predecessorIds": null,
  "incompleteDependencyCount": null,
  "description": null,
  "isBlocked": null,
  "isArchived": null,
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


