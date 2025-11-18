using MiniEcommerce.Commons.Swagger.Configuration;

namespace MiniEcommerce.Commons.Swagger.Docs;

public abstract class BaseDoc<TKey> where TKey : Enum
{
    protected static readonly Dictionary<TKey, SwaggerDocumentationInfo> Docs = new();

    public static SwaggerDocumentationInfo Get(TKey key)
    {
        if (!Docs.TryGetValue(key, out var doc))
            throw new KeyNotFoundException($"Documentation for key '{key}' not found.");

        return doc;
    }
}