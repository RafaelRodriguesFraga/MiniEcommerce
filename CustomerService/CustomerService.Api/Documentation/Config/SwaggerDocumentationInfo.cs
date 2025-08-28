namespace CustomerService.Api.Documentation.Config
{
    public class SwaggerDocumentationInfo
    {
        public SwaggerDocumentationInfo(string summary, string description)
        {
            Summary = summary;
            Description = description;
        }
        public string Summary { get; set; }
        public string Description { get; set; }
        public object? RequestExample { get; set; }
        public (int statusCode, string Description, Type? ResponseType, object? Example)[]? Responses { get; set; }
    }
}