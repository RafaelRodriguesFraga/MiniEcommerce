using DotnetBaseKit.Components.Application.Base;
using DotnetBaseKit.Components.Shared.Notifications;
using ProductService.Application.DTOs;
using ProductService.Application.Extensions;
using ProductService.Domain.Entities;
using ProductService.Domain.Repositories;


namespace ProductService.Application.Services;

public class ProductServiceApplication : BaseServiceApplication, IProductServiceApplication
{
    private readonly IProductReadRepository _readRepository;
    private readonly IProductWriteRepository _writeRepository;

    public ProductServiceApplication(
        NotificationContext notificationContext,
        IProductReadRepository readRepository,
        IProductWriteRepository writeRepository) : base(notificationContext)
    {
        _readRepository = readRepository;
        _writeRepository = writeRepository;
    }

    public async Task<ProductResponseDto?> GetByIdAsync(Guid id)
    {
        var product = await _readRepository.GetByIdAsync(id);
        return product?.ToDto();
    }

    public async Task<ProductResponseDto?> GetBySlugAsync(string slug)
    {
        var product = await _readRepository.GetBySlugAsync(slug);
        return product?.ToDto();
    }

    public async Task<IEnumerable<ProductResponseDto>> GetAllActiveAsync(int page, int size)
    {
    
        var query = await _readRepository.GetAllAsync(); 
        
        return query
            .Where(p => p.Active)
            .Skip((page - 1) * size)
            .Take(size)
            .Select(p => p.ToDto());
    }


    public async Task CreateAsync(ProductRequestDto dto)
    {
        if (dto.Invalid)
        {
            _notificationContext.AddNotifications(dto.Notifications);
            return;
        }

        var productSkuExists = await _readRepository.ExistsBySkuAsync(dto.Sku);
        if (productSkuExists)
        {
            _notificationContext.AddNotification("Sku", "Product with this SKU already exists.");
            return;
        }

        var product = dto.ToEntity();

        var productSlugExists = await _readRepository.ExistsBySlugAsync(product.Slug);
        if (productSlugExists)
        {
            _notificationContext.AddNotification("Name", "A product with a similar name already exists (Slug conflict).");
            return;
        }

        await _writeRepository.InsertAsync(product);
    }

    public async Task UpdateAsync(Guid id, ProductRequestDto dto)
    {
        var product = await _readRepository.GetByIdAsync(id);
        if (product == null)
        {
            _notificationContext.AddNotification("Product", "Product not found.");
            return;
        }

        var productSkuExists = await _readRepository.ExistsBySkuAsync(dto.Sku);

        if (dto.Sku != product.Sku && productSkuExists)
        {
            _notificationContext.AddNotification("Sku", "SKU is already in use by another product.");
            return;
        }

        product.Update(
            dto.Name,
            dto.Description,
            dto.Price,
            dto.Category,
            dto.ImageUrl
        );

        await _writeRepository.UpdateAsync(product);
    }

    public async Task ToggleStatusAsync(Guid id)
    {
        var product = await _readRepository.GetByIdAsync(id);
        if (product == null)
        {
            _notificationContext.AddNotification("Product", "Product not found.");
            return;
        }

        if (product.Active)
        {

            product.Deactivate();
        }
        else
        {
            product.Activate();
        }

        await _writeRepository.UpdateAsync(product);
    }
}