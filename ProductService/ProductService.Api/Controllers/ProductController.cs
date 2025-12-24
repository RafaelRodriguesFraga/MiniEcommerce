using Microsoft.AspNetCore.Mvc;
using ProductService.Application.Services;
using DotnetBaseKit.Components.Api.Base;
using DotnetBaseKit.Components.Api.Responses;
using ProductService.Application.DTOs;

namespace ProductService.Api.Controllers;

[Route("api/products")]
[ApiController]
public class ProductsController : ApiControllerBase
{
    private readonly IProductServiceApplication _service;

    public ProductsController(
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

    [HttpGet]
    public async Task<IActionResult> GetAllAsync([FromQuery] int page = 1, [FromQuery] int size = 10)
    {
        var products = await _service.GetAllActiveAsync(page, size);
        return ResponseOk(products);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetByIdAsync(Guid id)
    {
        var product = await _service.GetByIdAsync(id);
        
        return ResponseOk(product);
    }

    [HttpGet("{slug}")]
    public async Task<IActionResult> GetBySlugAsync(string slug)
    {
        var product = await _service.GetBySlugAsync(slug);

        return ResponseOk(product);
    }
}