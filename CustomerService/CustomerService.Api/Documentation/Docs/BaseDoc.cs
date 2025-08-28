using CustomerService.Api.Documentation.Config;

public abstract class BaseDoc<TKey> where TKey : Enum
{
    protected static readonly Dictionary<TKey, ApiDocInfo> Docs = new();

    public static ApiDocInfo Get(TKey key)
    {
        if (!Docs.TryGetValue(key, out var doc))
            throw new KeyNotFoundException($"Documentation for key '{key}' not found.");

        return doc;
    }
}