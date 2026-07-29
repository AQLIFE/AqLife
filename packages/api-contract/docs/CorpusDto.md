
# CorpusDto


## Properties

Name | Type
------------ | -------------
`uid` | string
`corpusContent` | string
`createDate` | string

## Example

```typescript
import type { CorpusDto } from '@aqlife/api-contract'

// TODO: Update the object below with actual values
const example = {
  "uid": null,
  "corpusContent": null,
  "createDate": null,
} satisfies CorpusDto

console.log(example)

// Convert the instance to a JSON string
const exampleJSON: string = JSON.stringify(example)
console.log(exampleJSON)

// Parse the JSON string back to an object
const exampleParsed = JSON.parse(exampleJSON) as CorpusDto
console.log(exampleParsed)
```

[[Back to top]](#) [[Back to API list]](../README.md#api-endpoints) [[Back to Model list]](../README.md#models) [[Back to README]](../README.md)


