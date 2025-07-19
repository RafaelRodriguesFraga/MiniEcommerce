using System.Net;
using AuthService.Api.ViewModels.Request;
using AuthService.Api.ViewModels.Response;
using AuthService.Application.DTOs;
using AuthService.Application.DTOs.Request;
using AuthService.Application.Services;
using DotnetBaseKit.Components.Api.Base;
using DotnetBaseKit.Components.Api.Responses;
using Microsoft.AspNetCore.Mvc;


namespace AuthService.Api.Controllers;

[ApiController]
[Route("api/users")]
public class AuthController : ApiControllerBase
{
    private readonly IUserRegistrationService _registrationService;

    public AuthController(IResponseFactory responseFactory, IUserRegistrationService registrationService)
        : base(responseFactory)
    {
        _registrationService = registrationService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserRequestDto request)
    {
         await _registrationService.RegisterUserAsync(request);
            
         return ResponseCreated();
    }
}