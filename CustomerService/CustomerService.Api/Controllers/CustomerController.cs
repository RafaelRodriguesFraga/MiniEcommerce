using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using DotnetBaseKit.Components.Api.Base;
using DotnetBaseKit.Components.Api.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CustomerService.Application;
using CustomerService.Application.DTOs;
using CustomerService.Shared.Filters;
using CustomerService.Api.Documentation.Configuration;
using CustomerService.Api.Documentation.Docs;
using CustomerService.Api.Documentation.Docs.Keys;

namespace CustomerService.Api.Controllers;

[Route("api/customers")]
[Authorize(AuthenticationSchemes = "Bearer")]
[ApiController]
[RequireUserId]
public class CustomerController : ApiControllerBase
{
    private readonly ICustomerServiceApplication _customerServiceApplication;

    public CustomerController(IResponseFactory responseFactory,
        ICustomerServiceApplication customerServiceApplication) : base(
        responseFactory)
    {
        _customerServiceApplication = customerServiceApplication;
    }

    [HttpGet("/me")]
    [SwaggerDocumentation(typeof(CustomerDocs), nameof(CustomerDocKey.GetMe))]
    public async Task<IActionResult> GetMeAsync()
    {
        var userId = GetAuthServiceId();
        var result = await _customerServiceApplication.GetByUserIdAsync(userId);

        return ResponseOk(result);
    }

    [HttpPost]
    [SwaggerDocumentation(typeof(CustomerDocs), nameof(CustomerDocKey.Create))]
    public async Task<IActionResult> CreateAsync([FromBody] CustomerRequestDto dto)
    {
        var userId = GetAuthServiceId();
        var userName = HttpContext.User.FindFirst(JwtRegisteredClaimNames.Name)?.Value;
        var userEmail = HttpContext.User.FindFirst(ClaimTypes.Email)?.Value;


        var result = await _customerServiceApplication.CreateAsync(dto, userId, userName!, userEmail!);

        return ResponseCreated(result);
    }

    [HttpPatch("{customerId:guid}")]    [SwaggerDocumentation(typeof(CustomerDocs), nameof(CustomerDocKey.Update))]
    public async Task<IActionResult> UpdateAsync([FromRoute] Guid customerId, [FromBody] CustomerUpdateDto dto)
    {
        var authServiceId = GetAuthServiceId();
        var result = await _customerServiceApplication.UpdateAsync(customerId, authServiceId, dto);

        return ResponseOk(result);
    }

    private Guid GetAuthServiceId()
    {
        return (Guid)HttpContext.Items["UserId"]!;
    }
}