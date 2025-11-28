using DotnetBaseKit.Components.Domain.Sql.Repositories;
using CustomerService.Domain.Entities;

namespace CustomerService.Domain.Repositories;

public interface ICustomerReadRepository : IBaseReadRepository<Customer>
{
    Task<Customer?> GetByAuthServiceIdAsync(Guid authServiceId);
}