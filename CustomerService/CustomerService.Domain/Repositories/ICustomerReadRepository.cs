using DotnetBaseKit.Components.Domain.Sql.Repositories;
using CustomerService.Domain.Entities;

namespace CustomerService.Domain.Repositories;

public interface ICustomerReadRepository : IBaseReadRepository<Customer>
{
    Task<Customer?> GetByUserIdAsync(Guid userId);
}