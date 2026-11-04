
# TodoRoadmapDto


## Properties

Name | Type
------------ | -------------
`planId` | string
`planName` | string
`dependencyVersion` | number
`nodes` | [Array&lt;TodoDto&gt;](TodoDto.md)
`dependencies` | [Array&lt;TodoDependencyDto&gt;](TodoDependencyDto.md)
`layouts` | [Array&lt;RoadmapNodeLayoutDto&gt;](RoadmapNodeLayoutDto.md)

## Example

```typescript
import type { TodoRoadmapDto } from '@aqlife/api-contract'

// TODO: Update the object below with actual values
const example = {
  "planId": null,
  "planName": null,
  "dependencyVersion": null,
  "nodes": null,
  "dependencies": null,
  "layouts": null,
} satisfies TodoRoadmapDto

console.log(example)

// Convert the instance to a JSON string
const exampleJSON: string = JSON.stringify(example)
console.log(exampleJSON)

// Parse the JSON string back to an object
const exampleParsed = JSON.parse(exampleJSON) as TodoRoadmapDto
console.log(exampleParsed)
```

[[Back to top]](#) [[Back to API list]](../README.md#api-endpoints) [[Back to Model list]](../README.md#models) [[Back to README]](../README.md)


