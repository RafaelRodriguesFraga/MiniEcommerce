using AuthService.Api.Documentation.Keys;
using AuthService.Application.DTOs;
using Commons
namespace AuthService.Api.Documentation;

public class AuthDocs : BaseDoc<AuthDocKey>
{
    static AuthDocs()
    {
        Docs[AuthDocKey.Login] = new SwaggerDocumentationInfo(
            summary: "Authenticate user",
            description: "Returns access and refresh tokens")
        {
            // ... seus exemplos ...
            Responses = new[] {
                (200, "Success", typeof(TokenDto), null),
                CommonResponsesDoc.BadRequest
            }
        };
    }
}