using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using DotnetBaseKit.Components.Api.Base;
using DotnetBaseKit.Components.Api.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CustomerService.Application;
using CustomerService.Application.DTOs;
using CustomerService.Shared.Filters;

namespace CustomerService.Api.Controllers;

[Route("api/customers")]
[Authorize(AuthenticationSchemes = "Bearer")]
[ApiController]
[RequireUserId]
public class CustomerController : ApiControllerBase
{
    private readonly ICustomerServiceApplication _CustomerServiceApplication;

    public CustomerController(IResponseFactory responseFactory,
        ICustomerServiceApplication CustomerServiceApplication) : base(
        responseFactory)
    {
        _CustomerServiceApplication = CustomerServiceApplication;
    }

    [HttpGet]
    public async Task<IActionResult> GetMeAsync()
    {
        var userId = GetUserId();
        var result = await _CustomerServiceApplication.GetByUserIdAsync(userId);

        return ResponseOk(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CustomerRequestDto dto)
    {
        var userId = GetUserId();
        var userName = HttpContext.User.FindFirst(JwtRegisteredClaimNames.Name)?.Value;
        var userEmail = HttpContext.User.FindFirst(ClaimTypes.Email)?.Value;


        var result = await _CustomerServiceApplication.CreateAsync(dto, userId, userName!, userEmail!);

        return ResponseCreated(result);
    }

    [HttpPatch]
    public async Task<IActionResult> UpdateAsync([FromBody] CustomerUpdateDto dto)
    {
        var userId = GetUserId();
        var result = await _CustomerServiceApplication.UpdateAsync(userId, dto);

        return ResponseOk(result);
    }

    private Guid GetUserId()
    {
        return (Guid)HttpContext.Items["UserId"]!;
    }
}