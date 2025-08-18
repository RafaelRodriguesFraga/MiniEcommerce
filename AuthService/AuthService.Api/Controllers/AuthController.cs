using AuthService.Application.DTOs;
using AuthService.Application.DTOs.Auth;
using AuthService.Application.DTOs.Login;
using AuthService.Application.DTOs.User;
using AuthService.Application.Services;
using AuthService.Application.Services.Auth;
using AuthService.Application.Services.Token;
using DotnetBaseKit.Components.Api.Base;
using DotnetBaseKit.Components.Api.Responses;
using Microsoft.AspNetCore.Identity.Data;
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

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserRequestDto request)
    {
        await _userServiceApplication.RegisterAsync(request);

        return ResponseCreated();
    }


    [ProducesResponseType(typeof(TokenDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync(LoginRequestDto dto)
    {
        var authenticateUser = await _userServiceApplication.AuthenticateAsync(dto);
        var userIsNull = authenticateUser == null;
        if (userIsNull)
        {
            return ResponseBadRequest(authenticateUser);
        }

        var token = _tokenServiceApplication.GenerateTokenAsync(authenticateUser!.Id, authenticateUser.Email, authenticateUser.Name);

        return ResponseOk(token);
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPasswordAsync(ResetPasswordDto dto)
    {
        await _authServiceApplication.ResetPasswordAsync(dto.Email, dto.NewPassword);

        return CreateResponse();
    }
}