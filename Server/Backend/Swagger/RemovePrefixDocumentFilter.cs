using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Backend.Swagger;

class RemovePrefixDocumentFilter(string prefix) : IDocumentFilter
{
    private readonly string _prefix = prefix;

    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        var paths = swaggerDoc.Paths.ToDictionary(
            path => path.Key.StartsWith(_prefix) ? path.Key.Substring(_prefix.Length) : path.Key,
            path => path.Value
        );

        swaggerDoc.Paths = new OpenApiPaths();
        foreach (var path in paths)
        {
            swaggerDoc.Paths.Add(path.Key, path.Value);
        }
    }
}
