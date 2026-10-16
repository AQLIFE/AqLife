using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace AqLife.Extensions.SwaggerDoc
{
    public sealed class EnumDescriptionSchemaFilter : ISchemaFilter
    {
        public void Apply(IOpenApiSchema schema, SchemaFilterContext context)
        {
            var enumType = context.Type;

            if (!enumType.IsEnum || schema is not OpenApiSchema openApiSchema)
                return;

            var fields = enumType.GetFields(BindingFlags.Public | BindingFlags.Static)
                .OrderBy(field => field.MetadataToken)
                .ToArray();

            var names = new JsonArray();
            var descriptions = new JsonArray();
            var displayLines = new List<string>();

            foreach (var field in fields)
            {
                var name = field.Name;
                var value = Convert.ToInt64(field.GetRawConstantValue());

                var description = field
                    .GetCustomAttribute<DescriptionAttribute>()
                    ?.Description;

                names.Add(JsonValue.Create(name));

                descriptions.Add(JsonValue.Create(description ?? string.Empty));

                displayLines.Add(
                    string.IsNullOrWhiteSpace(description)
                        ? $"{value} = {name}"
                        : $"{value} = {name}：{description}");
            }

            openApiSchema.Extensions ??=
                new Dictionary<string, IOpenApiExtension>();

            openApiSchema.Extensions["x-enum-varnames"] =
                new JsonNodeExtension(names);

            openApiSchema.Extensions["x-enum-descriptions"] =
                new JsonNodeExtension(descriptions);

            var enumDescription = string.Join("\n",displayLines.Select(line => $"- {line}"));

            openApiSchema.Description = string.IsNullOrWhiteSpace(
                openApiSchema.Description)
                    ? enumDescription
                    : $"{openApiSchema.Description}\n\n{enumDescription}";
        }
    }
}
