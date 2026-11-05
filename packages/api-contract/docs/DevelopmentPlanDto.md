
# DevelopmentPlanDto


## Properties

Name | Type
------------ | -------------
`uid` | string
`name` | string
`description` | string
`isArchived` | boolean
`createdAt` | string
`updatedAt` | string
`dependencyVersion` | number

## Example

```typescript
import type { DevelopmentPlanDto } from '@aqlife/api-contract'

// TODO: Update the object below with actual values
const example = {
  "uid": null,
  "name": null,
  "description": null,
  "isArchived": null,
  "createdAt": null,
  "updatedAt": null,
  "dependencyVersion": null,
} satisfies DevelopmentPlanDto

console.log(example)

// Convert the instance to a JSON string
const exampleJSON: string = JSON.stringify(example)
console.log(exampleJSON)

// Parse the JSON string back to an object
const exampleParsed = JSON.parse(exampleJSON) as DevelopmentPlanDto
console.log(exampleParsed)
```

[[Back to top]](#) [[Back to API list]](../README.md#api-endpoints) [[Back to Model list]](../README.md#models) [[Back to README]](../README.md)


