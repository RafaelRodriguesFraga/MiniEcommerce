using System.Security.Claims;
using DotnetBaseKit.Components.Api.Base;
using DotnetBaseKit.Components.Api.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.Application;

namespace UserService.Api.Controllers;

[Route("api/user-profile")]
[Authorize(AuthenticationSchemes = "Bearer")]
public class UserProfileController : ApiControllerBase
{
    private readonly IUserProfileServiceApplication _userProfileServiceApplication;

    public UserProfileController(IResponseFactory responseFactory, IUserProfileServiceApplication userProfileServiceApplication) : base(
        responseFactory)
    {
        _userProfileServiceApplication = userProfileServiceApplication;
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMeAsync()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var result = await _userProfileServiceApplication.GetByUserIdAsync(Guid.Parse(userId));
        
        return ResponseOk(result);
    }
}