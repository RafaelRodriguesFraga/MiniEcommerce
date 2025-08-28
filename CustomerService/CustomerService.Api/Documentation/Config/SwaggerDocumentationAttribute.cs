

namespace CustomerService.Api.Documentation.Config;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class SwaggerDocumentationAttribute : Attribute
{
    public Type DocType { get; }
    public string Key { get; }

    public SwaggerDocumentationAttribute(Type docType, string key)
    {
        DocType = docType;
        Key = key;
    }
}