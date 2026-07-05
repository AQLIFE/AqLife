$SwaggerUrl = "http://localhost:5110/swagger/v1/swagger.json"
$OutputDir = "../../packages/api-contract/src"

Write-Host "[AqLife] 正在检查后端 OpenAPI 契约状态..." -ForegroundColor Cyan

try {
    $response = Invoke-WebRequest -Uri $SwaggerUrl -Method Get -UseBasicParsing -ErrorAction Stop
    if ($response.StatusCode -ne 200) {
        throw "后端返回状态码: $($response.StatusCode)"
    }
}
catch {
    Write-Host "❌ 错误: 无法连接到后端 $SwaggerUrl" -ForegroundColor Red
    Write-Host "请确保你的 .NET API 项目已启动并在监听 5110 端口。" -ForegroundColor Yellow
    exit 1
}

Write-Host "[AqLife] 契约校验通过，正在生成 TypeScript 代码..." -ForegroundColor Cyan

npx @openapitools/openapi-generator-cli generate `
  -i "$SwaggerUrl" `
  -g typescript-fetch `
  -o "$OutputDir" `
  --additional-properties=supportsES6=true,typescriptThreePlus=true,useSingleRequestParameter=true,modelPropertyNaming=camelCase `
  --skip-validate-spec

if ($LASTEXITCODE -eq 0) {
    Write-Host "`n[AqLife] ✅ 契约同步完成！输出目录: $OutputDir" -ForegroundColor Green
} else {
    Write-Host "`n❌ 错误: OpenAPI 生成器执行失败。" -ForegroundColor Red
    exit $LASTEXITCODE
}
