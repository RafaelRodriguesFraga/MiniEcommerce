using System.Security.Claims;
using CustomerService.Api.Documentation.Configuration;
using CustomerService.Api.Documentation.Docs;
using CustomerService.Api.Documentation.Docs.Keys;
using CustomerService.Application.Address;
using CustomerService.Application.DTOs.Address;
using CustomerService.Application.Interfaces;
using DotnetBaseKit.Components.Api.Base;
using DotnetBaseKit.Components.Api.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CustomerService.Api.Controllers;

[ApiController]
[Route("api/addresses")]
[Authorize(AuthenticationSchemes = "Bearer")]
public class AddressController : ApiControllerBase
{
    private readonly IAddressServiceApplication _serviceApplication;
    private readonly IUserContext _userContext;
    public AddressController(IResponseFactory responseFactory, IAddressServiceApplication serviceApplication, IUserContext userContext) : base(responseFactory)
    {
        _serviceApplication = serviceApplication;
        _userContext = userContext;
    }

    [HttpGet("{id:guid}")]
    [SwaggerDocumentation(typeof(AddressDocs), nameof(AddressDocKey.GetById))]
    public async Task<IActionResult> GetByIdAsync(Guid id)
    {
        var myUserId = Guid.Parse(_userContext.UserId!);
        var address = await _serviceApplication.GetByIdAsync(id, myUserId);

        return ResponseOk(address);
    }

    [HttpGet]
    [SwaggerDocumentation(typeof(AddressDocs), nameof(AddressDocKey.GetAllByCustomerId))]
    public async Task<IActionResult> GetMyAddresses()
    {
        var myUserId = Guid.Parse(_userContext.UserId!);
        if (myUserId == Guid.Empty)
            return Unauthorized();

        var addresses = await _serviceApplication.GetByCustomerIdAsync(myUserId);

        return ResponseOk(addresses);
    }

    [HttpPost]
    [SwaggerDocumentation(typeof(AddressDocs), nameof(AddressDocKey.Create))]
    public async Task<IActionResult> CreateAsync([FromBody] AddressRequestDto request)
    {
        var myUserId = Guid.Parse(_userContext.UserId!);
        await _serviceApplication.CreateAsync(request, myUserId);

        return ResponseCreated();
    }

    [HttpPut("{id:guid}")]
    [SwaggerDocumentation(typeof(AddressDocs), nameof(AddressDocKey.Update))]
    public async Task<IActionResult> Update(Guid id, [FromBody] AddressUpdateDto request)
    {
        var myUserId = Guid.Parse(_userContext.UserId!);
        await _serviceApplication.UpdateAsync(id, request, myUserId);

        return CreateResponse();
    }

    [HttpDelete("{id:guid}")]
    [SwaggerDocumentation(typeof(AddressDocs), nameof(AddressDocKey.Delete))]
    public async Task<IActionResult> Delete(Guid id)
    {
        var myUserId = Guid.Parse(_userContext.UserId!);
        await _serviceApplication.DeleteAsync(id, myUserId);

        return CreateResponse();
    }
}