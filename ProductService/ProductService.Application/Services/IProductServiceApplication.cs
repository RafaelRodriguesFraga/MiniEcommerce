using ProductService.Application.DTOs;

namespace ProductService.Application.Services;

public interface IProductServiceApplication
{
    Task<ProductResponseDto?> GetByIdAsync(Guid id);
    Task<ProductResponseDto?> GetBySlugAsync(string slug);
    Task<IEnumerable<ProductResponseDto>> GetAllActiveAsync(int page, int size);

    Task CreateAsync(ProductRequestDto dto);
    Task UpdateAsync(Guid id, ProductRequestDto dto);
    Task ToggleStatusAsync(Guid id);
    Task DeleteAsync(Guid id);
}