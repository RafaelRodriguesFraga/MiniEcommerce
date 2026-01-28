using Microsoft.AspNetCore.Mvc;
using ProductService.Application.Services;
using DotnetBaseKit.Components.Api.Base;
using DotnetBaseKit.Components.Api.Responses;
using ProductService.Application.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace ProductService.Api.Controllers.Admin;

[Route("api/admin/products")]
[ApiController]
[Authorize(Policy = "AdminOnly")]
public class AdminProductsController : ApiControllerBase
{
    private readonly IProductServiceApplication _service;

    public AdminProductsController(
        IResponseFactory responseFactory,
        IProductServiceApplication service) : base(responseFactory)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] ProductRequestDto dto)
    {
        await _service.CreateAsync(dto);
        return ResponseCreated();
    }


    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] ProductRequestDto dto)
    {
        await _service.UpdateAsync(id, dto);

        return CreateResponse();
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> ToggleStatusAsync(Guid id)
    {
        await _service.ToggleStatusAsync(id);

        return CreateResponse();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        await _service.DeleteAsync(id);

        return CreateResponse();
    }
}