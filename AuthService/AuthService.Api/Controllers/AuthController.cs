using AuthService.Api.Documentation;
using AuthService.Api.Documentation.Keys;
using AuthService.Application.DTOs.Auth;
using AuthService.Application.DTOs.Login;
using AuthService.Application.DTOs.Token;
using AuthService.Application.DTOs.User;
using AuthService.Application.Services;
using AuthService.Application.Services.Auth;
using AuthService.Application.Services.Token;
using AuthService.Application.Services.Token.Facade;
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
    private readonly IAuthServiceApplication _authServiceApplication;
    private readonly ITokenFacade _tokenFacade;
    private readonly IJwkServiceApplication _jwkService;

    public AuthController(
        IResponseFactory responseFactory,
        IUserServiceApplication userServiceApplication,
        IAuthServiceApplication authServiceApplication,
        ITokenFacade tokenFacade, IJwkServiceApplication jwkService)
        : base(responseFactory)
    {
        _userServiceApplication = userServiceApplication;
        _authServiceApplication = authServiceApplication;
        _tokenFacade = tokenFacade;
        _jwkService = jwkService;
    }

    [HttpGet(".well-known/jwks.json")]
    public IActionResult GetJsonWebKeySet()
    {
        var jwks = _jwkService.GetJsonWebKeySet();

        return Ok(jwks);
    }

    [HttpPost("register")]
    [SwaggerDocumentation(typeof(AuthDocs), nameof(AuthDocKey.Register))]
    public async Task<IActionResult> RegisterAsync([FromBody] UserRequestDto request)
    {
        await _userServiceApplication.RegisterAsync(request);

        return ResponseCreated();
    }


    [HttpPost("login")]
    [SwaggerDocumentation(typeof(AuthDocs), nameof(AuthDocKey.Login))]
    public async Task<IActionResult> LoginAsync(LoginRequestDto dto)
    {
        var user = await _userServiceApplication.AuthenticateAsync(dto);
        var userIsNull = user == null;
        if (userIsNull)
        {
            return ResponseBadRequest(user);
        }

        var tokenDto = await _tokenFacade.GenerateAndSaveTokensAsync(user.Id, user.Email, user.Name);

        return ResponseOk(tokenDto);
    }


    [HttpPost("refresh-token")]
    [SwaggerDocumentation(typeof(AuthDocs), nameof(AuthDocKey.RefreshToken))]
    public async Task<IActionResult> RefreshTokenAsync([FromBody] RefreshTokenRequestDto dto)
    {
        var tokenDto = await _tokenFacade.RefreshTokenAsync(dto.Token, dto.RefreshToken);

        if (tokenDto == null)
        {
            return Unauthorized("Expired or invalid refresh token");
        }

        return ResponseOk(tokenDto);
    }


    [HttpPost("reset-password")]
    [SwaggerDocumentation(typeof(AuthDocs), nameof(AuthDocKey.ResetPassword))]
    public async Task<IActionResult> ResetPasswordAsync(ResetPasswordDto dto)
    {
        await _authServiceApplication.ResetPasswordAsync(dto.Email, dto.NewPassword);

        return CreateResponse();
    }
}
