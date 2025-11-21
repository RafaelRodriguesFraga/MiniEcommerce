using AuthService.Api.Documentation;
using AuthService.Api.Documentation.Keys;
using AuthService.Application.DTOs.Auth;
using AuthService.Application.DTOs.Login;
using AuthService.Application.DTOs.User;
using AuthService.Application.Services;
using AuthService.Application.Services.Auth;
using AuthService.Application.Services.Token;
using Commons.Swagger.Configuration;
using DotnetBaseKit.Components.Api.Base;
using DotnetBaseKit.Components.Api.Responses;
using Microsoft.AspNetCore.Mvc;


namespace AuthService.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ApiControllerBase
{
    private readonly IUserServiceApplication _userServiceApplication;
    private readonly ITokenServiceApplication _tokenServiceApplication;
    private readonly IAuthServiceApplication _authServiceApplication;

    public AuthController(IResponseFactory responseFactory, IUserServiceApplication userServiceApplication, ITokenServiceApplication tokenServiceApplication,
        IAuthServiceApplication authServiceApplication)
        : base(responseFactory)
    {
        _userServiceApplication = userServiceApplication;
        _tokenServiceApplication = tokenServiceApplication;
        _authServiceApplication = authServiceApplication;
    }

    [HttpGet(".well-known/jwks.json")]
    public IActionResult GetJsonWebKeySet()
    {
        var jwks = _tokenServiceApplication.GetJsonWebKeySet();

        return Ok(jwks);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserRequestDto request)
    {
        await _userServiceApplication.RegisterAsync(request);

        return ResponseCreated();
    }


    [HttpPost("login")]
    [SwaggerDocumentation(typeof(AuthDocs), nameof(AuthDocKey.Login))]
    public async Task<IActionResult> LoginAsync(LoginRequestDto dto)
    {
        var authenticateUser = await _userServiceApplication.AuthenticateAsync(dto);
        var userIsNull = authenticateUser == null;
        if (userIsNull)
        {
            return ResponseBadRequest(authenticateUser);
        }

        var token = _tokenServiceApplication.GenerateToken(authenticateUser!.Id, authenticateUser.Email, authenticateUser.Name);

        return ResponseOk(token);
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPasswordAsync(ResetPasswordDto dto)
    {
        await _authServiceApplication.ResetPasswordAsync(dto.Email, dto.NewPassword);

        return CreateResponse();
    }
}
