using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using DotnetBaseKit.Components.Api.Base;
using DotnetBaseKit.Components.Api.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.Application;
using UserService.Application.DTOs;
using UserService.Shared.Filters;

namespace UserService.Api.Controllers;

[Route("api/user-profile")]
[Authorize(AuthenticationSchemes = "Bearer")]
[ApiController]
[RequireUserId]
public class UserProfileController : ApiControllerBase
{
    private readonly IUserProfileServiceApplication _userProfileServiceApplication;

    public UserProfileController(IResponseFactory responseFactory,
        IUserProfileServiceApplication userProfileServiceApplication) : base(
        responseFactory)
    {
        _userProfileServiceApplication = userProfileServiceApplication;
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMeAsync()
    {
        var userId = GetUserId();
        var result = await _userProfileServiceApplication.GetByUserIdAsync(userId);

        return ResponseOk(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] UserProfileRequestDto dto)
    {
        var userId = GetUserId();
        var userName = HttpContext.User.FindFirst(JwtRegisteredClaimNames.Name)?.Value;
        var userEmail = HttpContext.User.FindFirst(ClaimTypes.Email)?.Value;


        var result = await _userProfileServiceApplication.CreateAsync(dto, userId, userName!, userEmail!);

        return ResponseCreated(result);
    }

    private Guid GetUserId()
    {
        return (Guid)HttpContext.Items["UserId"]!;
    }
}