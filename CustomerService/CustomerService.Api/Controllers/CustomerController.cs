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
using CustomerService.Application.Interfaces;

namespace CustomerService.Api.Controllers;

[Route("api/customers")]
[Authorize(AuthenticationSchemes = "Bearer")]
[ApiController]
public class CustomerController : ApiControllerBase
{
    private readonly ICustomerServiceApplication _customerServiceApplication;
    private readonly IUserContext _userContext;


    public CustomerController(IResponseFactory responseFactory,
        ICustomerServiceApplication customerServiceApplication,
        IUserContext userContext) : base(
        responseFactory)
    {
        _customerServiceApplication = customerServiceApplication;
        _userContext = userContext;
    }

    [HttpPost("me")]
    [SwaggerDocumentation(typeof(CustomerDocs), nameof(CustomerDocKey.GetMe))]

    public async Task<IActionResult> GetMe()
    {
        var myUserId = Guid.Parse(_userContext.UserId!);
        if (myUserId == Guid.Empty)
        {
            return Unauthorized();
        }

        var result = await _customerServiceApplication.GetOrCreateAsync();

        return Ok(result);
    }

    [HttpPatch("me")]
    [SwaggerDocumentation(typeof(CustomerDocs), nameof(CustomerDocKey.Update))]
    public async Task<IActionResult> UpdateAsync([FromBody] CustomerUpdateDto dto)
    {
        var myUserId = Guid.Parse(_userContext.UserId!);
        if (myUserId == Guid.Empty)
        {
            return Unauthorized();
        }

        var result = await _customerServiceApplication.UpdateAsync(myUserId, dto);

        return ResponseOk(result);
    }

}