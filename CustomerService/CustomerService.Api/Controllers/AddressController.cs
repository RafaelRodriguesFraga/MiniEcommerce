using CustomerService.Application.Address;
using CustomerService.Application.DTOs.Address;
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
    public AddressController(IResponseFactory responseFactory, IAddressServiceApplication serviceApplication) : base(responseFactory)
    {
        _serviceApplication = serviceApplication;
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetByIdAsync(Guid id)
    {
        var address = await _serviceApplication.GetByIdAsync(id);

        return ResponseOk(address);
    }

    [HttpGet("customer/{customerId:guid}")]
    public async Task<IActionResult> GetByCustomerIdAsync(Guid customerId)
    {
        var addresses = await _serviceApplication.GetByCustomerIdAsync(customerId);

        return ResponseOk(addresses);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] AddressRequestDto request)
    {

        await _serviceApplication.CreateAsync(request);

        return ResponseCreated();
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] AddressUpdateDto request)
    {

        await _serviceApplication.UpdateAsync(id, request);

        return CreateResponse(); 
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _serviceApplication.DeleteAsync(id);

        return CreateResponse();
    }
}