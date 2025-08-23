using DotnetBaseKit.Components.Infra.Sql.Context.Base;
using DotnetBaseKit.Components.Infra.Sql.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using CustomerService.Domain.Entities;
using CustomerService.Domain.Repositories;

namespace CustomerService.Infra.Repositories;

public class CustomerReadRepository : BaseReadRepository<Customer>, ICustomerReadRepository
{
    public CustomerReadRepository(BaseContext context) : base(context)
    {
    }

    public async Task<Customer?> GetByUserIdAsync(Guid userId)
    {
        return await Set.Where(p => p.AuthServiceId == userId).FirstOrDefaultAsync();
    }
}