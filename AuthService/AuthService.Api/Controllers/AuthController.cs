using AuthService.Application.DTOs.Login;
using AuthService.Application.DTOs.Request;
using AuthService.Application.Services;
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
    private readonly IUserService _userService;
    private readonly ITokenService  _tokenService;

    public AuthController(IResponseFactory responseFactory, IUserService userService, ITokenService tokenService)
        : base(responseFactory)
    {
        _userService = userService;
        _tokenService = tokenService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserRequestDto request)
    {
         await _userService.RegisterAsync(request);
            
         return ResponseCreated();
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync(LoginRequestDto dto)
    {
        var authenticateUser = await _userService.AuthenticateAsync(dto);
        var userIsNull = authenticateUser == null;
        if (userIsNull)
        {
            return ResponseBadRequest(authenticateUser);
        }

        var token = _tokenService.GenerateTokenAsync(authenticateUser!.Id, authenticateUser.Email);

        return ResponseOk(token);
    }
}