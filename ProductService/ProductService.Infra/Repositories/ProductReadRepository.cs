using DotnetBaseKit.Components.Infra.Sql.Context.Base;
using DotnetBaseKit.Components.Infra.Sql.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using ProductService.Domain.Entities;
using ProductService.Domain.Repositories;

namespace ProductService.Infra.Repositories;

public class ProductReadRepository : BaseReadRepository<Product>, IProductReadRepository
{
    public ProductReadRepository(BaseContext context) : base(context)
    {
    }

    public async Task<Product?> GetBySkuAsync(string sku)
    {
        return await Set
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Sku == sku);
    }

    public async Task<Product?> GetBySlugAsync(string slug)
    {
        return await Set
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Slug == slug);
    }

    public async Task<bool> ExistsBySkuAsync(string sku)
    {
        return await Set.AnyAsync(p => p.Sku == sku);
    }

    public async Task<bool> ExistsBySlugAsync(string slug)
    {
        return await Set.AnyAsync(p => p.Slug == slug);
    }
}