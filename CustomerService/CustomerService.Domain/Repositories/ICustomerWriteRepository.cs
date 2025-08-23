using DotnetBaseKit.Components.Domain.Sql.Repositories;
using CustomerService.Domain.Entities;

namespace CustomerService.Domain.Repositories;

public interface ICustomerWriteRepository : IBaseWriteRepository<Customer>
{

}