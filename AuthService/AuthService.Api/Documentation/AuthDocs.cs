using AuthService.Api.Documentation.Keys;
using AuthService.Application.DTOs;
using AuthService.Application.DTOs.Auth;
using AuthService.Application.DTOs.Login;
using AuthService.Application.DTOs.Token;
using AuthService.Application.DTOs.User;
using Commons.Swagger.Configuration;
using Commons.Swagger.Docs;
using DotnetBaseKit.Components.Api.Responses;

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
                CommonResponsesDoc.BadRequest,
                CommonResponsesDoc.Unauthorized
            }
        };

        Docs[AuthDocKey.RefreshToken] = new SwaggerDocumentationInfo(
            summary: "Renew Access Token",
            description: "Uses a valid Refresh Token to obtain a new pair of tokens without requiring login again.")
        {
            RequestExample = new RefreshTokenRequestDto
            {
                Token = "eyJhbGciOi...",
                RefreshToken = "Hu7..."
            },
            Responses = new[]
            {
                (200, "Success", typeof(RefreshTokenResponseDto), new RefreshTokenResponseDto
                {
                    Token = "novo-jwt...",
                    RefreshToken = "novo-refresh...",
                    ExpirationDate = DateTime.UtcNow.AddHours(2),
                    IssuedAt = DateTime.UtcNow
                }),
                CommonResponsesDoc.Unauthorized,
                CommonResponsesDoc.BadRequest
            }
        };

        Docs[AuthDocKey.Register] = new SwaggerDocumentationInfo(
            summary: "Register new user",
            description: "Creates a new user account in the system.")
        {
            RequestExample = new
            {
                Name = "Test",
                Email = "test@test.com",
                Password = "123456"
            },
            Responses = new[]
            {
                (201, "Created", null, null),
                CommonResponsesDoc.BadRequest
            }

        };

        Docs[AuthDocKey.ResetPassword] = new SwaggerDocumentationInfo(
            summary: "Reset Password",
            description: "Allows a user to change their password by providing their email and the new password.")
        {
            RequestExample = new ResetPasswordDto
            {
                Email = "joao@teste.com",
                NewPassword = "NovaSenhaSuperSegura456!"
            },
            Responses = new[]
            {
                CommonResponsesDoc.Success,
                CommonResponsesDoc.BadRequest,
            }
        };
    }
}