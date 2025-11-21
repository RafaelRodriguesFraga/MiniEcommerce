using AuthService.Api.Documentation.Keys;
using Commons.Swagger.Configuration;
using Commons.Swagger.Docs;

namespace AuthService.Api.Documentation;

public class AuthDocs : BaseDoc<AuthDocKey>
{
    static AuthDocs()
    {
        Docs[AuthDocKey.Login] = new SwaggerDocumentationInfo(
            summary: "Authenticate user",
            description: "Returns access and refresh tokens")
        {
            Responses = new[]
            {
                CommonResponsesDoc.Success,
                CommonResponsesDoc.BadRequest
            }
        };
    }
}