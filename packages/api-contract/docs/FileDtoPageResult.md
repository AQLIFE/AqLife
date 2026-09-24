
# FileDtoPageResult


## Properties

Name | Type
------------ | -------------
`items` | [Array&lt;FileDto&gt;](FileDto.md)
`page` | number
`pageSize` | number
`hasMore` | boolean

## Example

```typescript
import type { FileDtoPageResult } from '@aqlife/api-contract'

// TODO: Update the object below with actual values
const example = {
  "items": null,
  "page": null,
  "pageSize": null,
  "hasMore": null,
} satisfies FileDtoPageResult

console.log(example)

// Convert the instance to a JSON string
const exampleJSON: string = JSON.stringify(example)
console.log(exampleJSON)

// Parse the JSON string back to an object
const exampleParsed = JSON.parse(exampleJSON) as FileDtoPageResult
console.log(exampleParsed)
```

[[Back to top]](#) [[Back to API list]](../README.md#api-endpoints) [[Back to Model list]](../README.md#models) [[Back to README]](../README.md)


