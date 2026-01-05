using DotnetBaseKit.Components.Domain.Sql.Repositories;
using ProductService.Domain.Entities;

namespace ProductService.Domain.Repositories;

public interface IProductWriteRepository : IBaseWriteRepository<Product>
{
    Task DeleteByIdAsync(Guid id);
}