
# OverviewDto


## Properties

Name | Type
------------ | -------------
`draftCount` | number
`scheduledCount` | number
`publishedCount` | number
`recentFiles` | [Array&lt;FileDto&gt;](FileDto.md)

## Example

```typescript
import type { OverviewDto } from '@aqlife/api-contract'

// TODO: Update the object below with actual values
const example = {
  "draftCount": null,
  "scheduledCount": null,
  "publishedCount": null,
  "recentFiles": null,
} satisfies OverviewDto

console.log(example)

// Convert the instance to a JSON string
const exampleJSON: string = JSON.stringify(example)
console.log(exampleJSON)

// Parse the JSON string back to an object
const exampleParsed = JSON.parse(exampleJSON) as OverviewDto
console.log(exampleParsed)
```

[[Back to top]](#) [[Back to API list]](../README.md#api-endpoints) [[Back to Model list]](../README.md#models) [[Back to README]](../README.md)


