#!/bin/bash

SWAGGER_URL="http://localhost:5110/swagger/v1/swagger.json"
OUTPUT_DIR="../../packages/api-contract/src"

echo "[AqLife] 正在检查后端 OpenAPI 契约状态..."

if ! curl -s --head --request GET "$SWAGGER_URL" | grep "200 OK" > /dev/null; then
    echo "❌ 错误: 无法连接到后端 $SWAGGER_URL"
    echo "请确保你的 .NET API 项目已启动并在监听 5110 端口。"
    exit 1
fi

echo "[AqLife] 契约校验通过，正在生成 TypeScript 代码..."
npx @openapitools/openapi-generator-cli generate \
  -i "$SWAGGER_URL" \
  -g typescript-fetch \
  -o "$OUTPUT_DIR" \
  --additional-properties=supportsES6=true,typescriptThreePlus=true,useSingleRequestParameter=true,modelPropertyNaming=camelCase \
  --skip-validate-spec

echo "[AqLife] ✅ 契约同步完成！输出目录: $OUTPUT_DIR"
