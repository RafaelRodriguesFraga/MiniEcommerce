using DotnetBaseKit.Components.Domain.Sql.Repositories;
using AddressEntity = CustomerService.Domain.Entities.Address;

namespace CustomerService.Domain.Repositories.Address;

public interface IAddressWriteRepository : IBaseWriteRepository<AddressEntity>
{

}