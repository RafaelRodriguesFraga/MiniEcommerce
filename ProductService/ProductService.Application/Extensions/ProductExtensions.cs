using ProductService.Application.DTOs;
using ProductService.Domain.Entities;

namespace ProductService.Application.Extensions
{
    public static class ProductExtensions
    {
        public static ProductResponseDto ToDto(this Product entity)
        {

            return new ProductResponseDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                Sku = entity.Sku,
                Slug = entity.Slug,
                Price = entity.Price,
                ImageUrl = entity.ImageUrl,
                Category = entity.Category,
                Active = entity.Active
            };
        }

        public static Product ToEntity(this ProductRequestDto dto)
        {
            return new Product(
                dto.Name,
                dto.Description,
                dto.Price,
                dto.Sku,
                dto.Category,
                dto.ImageUrl
            );
        }
    }
}