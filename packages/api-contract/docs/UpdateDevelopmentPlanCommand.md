
# UpdateDevelopmentPlanCommand


## Properties

Name | Type
------------ | -------------
`uid` | string
`name` | string
`description` | string
`planId` | string

## Example

```typescript
import type { UpdateDevelopmentPlanCommand } from '@aqlife/api-contract'

// TODO: Update the object below with actual values
const example = {
  "uid": null,
  "name": null,
  "description": null,
  "planId": null,
} satisfies UpdateDevelopmentPlanCommand

console.log(example)

// Convert the instance to a JSON string
const exampleJSON: string = JSON.stringify(example)
console.log(exampleJSON)

// Parse the JSON string back to an object
const exampleParsed = JSON.parse(exampleJSON) as UpdateDevelopmentPlanCommand
console.log(exampleParsed)
```

[[Back to top]](#) [[Back to API list]](../README.md#api-endpoints) [[Back to Model list]](../README.md#models) [[Back to README]](../README.md)


