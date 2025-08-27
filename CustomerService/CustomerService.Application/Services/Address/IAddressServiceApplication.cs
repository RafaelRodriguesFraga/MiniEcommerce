using CustomerService.Application.DTOs.Address;
using DotnetBaseKit.Components.Application.Base;

namespace CustomerService.Application.Address;

public interface IAddressServiceApplication : IBaseServiceApplication
{
    Task<IEnumerable<AddressResponseDto>> GetByCustomerIdAsync(Guid customerId);
    Task<AddressResponseDto> GetByIdAsync(Guid id);
    Task CreateAsync(AddressRequestDto dto);
    Task UpdateAsync(Guid id, AddressUpdateDto dto);
    Task DeleteAsync(Guid id);
}