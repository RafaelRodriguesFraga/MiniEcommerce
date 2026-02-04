using ProductService.Application.DTOs;
using ProductService.Domain.Entities;

namespace ProductService.Application.Extensions;

public static class CategoryExtensions
{
    public static CategoryResponseDto ToDto(this Category entity)
    {
        return new CategoryResponseDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Slug = entity.Slug
        };
    }
}
