using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MiniEcommerce.Commons.Swagger.Configuration;

public class SwaggerDocumentationOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var apiDocAttribute = context.MethodInfo.GetCustomAttribute<SwaggerDocumentationAttribute>();
        if (apiDocAttribute == null)
        {
            return;
        }

        // Pega o TIPO do repositório (ex: typeof(CustomerDocs)) do atributo
        var repositoryType = apiDocAttribute.DocType;

        // Força a execução do construtor estático da classe de documentação
        // (ex: CustomerDocs) para garantir que o dicionário 'Docs' seja preenchido.
        RuntimeHelpers.RunClassConstructor(repositoryType.TypeHandle);

        // Procura por um método estático e público chamado "Get" nesse tipo
        var getMethod = repositoryType.GetMethod("Get",
            BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

        if (getMethod == null)
        {
            throw new InvalidOperationException(
                $"Could not find a static public method named 'Get' in {repositoryType.Name}.");
        }

        var parameters = getMethod.GetParameters();
        if (parameters.Length != 1)
        {
            throw new InvalidOperationException(
                $"The 'Get' method in {repositoryType.Name} must have exactly one parameter.");
        }

        var parameterType = parameters[0].ParameterType;

        var enumKey = Enum.Parse(parameterType, apiDocAttribute.Key, true);

        var apiDoc = (SwaggerDocumentationInfo)getMethod.Invoke(null, new object[] { enumKey })!;

        operation.Summary = apiDoc.Summary;
        operation.Description = apiDoc.Description;
        operation.Responses.Clear();

        if (apiDoc.RequestExample != null && operation.RequestBody != null)
        {
            var jsonExample = JsonSerializer.Serialize(apiDoc.RequestExample);
            foreach (var content in operation.RequestBody.Content)
            {
                content.Value.Example = new OpenApiString(jsonExample);
            }
        }

        if (apiDoc.Responses != null)
        {
            foreach (var response in apiDoc.Responses)
            {
                var statusCode = response.statusCode.ToString();
                if (!operation.Responses.ContainsKey(statusCode))
                {
                    operation.Responses[statusCode] = new OpenApiResponse { Description = response.Description };
                }

                if (response.ResponseType != null)
                {
                    operation.Responses[statusCode].Content.Add("application/json", new OpenApiMediaType
                    {
                        Schema = context.SchemaGenerator.GenerateSchema(response.ResponseType, context.SchemaRepository)
                    });
                }

                if (response.Example != null)
                {
                    var jsonExample = JsonSerializer.Serialize(response.Example);
                    foreach (var content in operation.Responses[statusCode].Content)
                    {
                        content.Value.Example = new OpenApiString(jsonExample);
                    }
                }
            }
        }
    }
}