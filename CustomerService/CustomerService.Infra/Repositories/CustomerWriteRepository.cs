using DotnetBaseKit.Components.Domain.Sql.Repositories;
using DotnetBaseKit.Components.Infra.Sql.Context.Base;
using DotnetBaseKit.Components.Infra.Sql.Repositories.Base;
using CustomerService.Domain.Entities;
using CustomerService.Domain.Repositories;

namespace CustomerService.Infra.Repositories;

public class CustomerWriteRepository : BaseWriteRepository<Customer>, ICustomerWriteRepository
{
    public CustomerWriteRepository(BaseContext context) : base(context)
    {
    }
}