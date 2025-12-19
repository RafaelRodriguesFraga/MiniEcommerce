using ProductService.Domain.Entities;
using ProductService.Domain.Repositories;
using DotnetBaseKit.Components.Infra.Sql.Context.Base;
using DotnetBaseKit.Components.Infra.Sql.Repositories.Base;

namespace ProductService.Infra.Repositories;

public class ProductWriteRepository : BaseWriteRepository<Product>, IProductWriteRepository
{
    public ProductWriteRepository(BaseContext context) : base(context)
    {
    }
}