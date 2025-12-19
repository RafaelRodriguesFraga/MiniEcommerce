using DotnetBaseKit.Components.Domain.Sql.Repositories;
using ProductService.Domain.Entities;

namespace ProductService.Domain.Repositories
{
    public interface IProductReadRepository : IBaseReadRepository<Product>
    {
        Task<Product?> GetBySkuAsync(string sku);
        Task<Product?> GetBySlugAsync(string slug);

        Task<bool> ExistsBySkuAsync(string sku);
        Task<bool> ExistsBySlugAsync(string slug);
    }
}