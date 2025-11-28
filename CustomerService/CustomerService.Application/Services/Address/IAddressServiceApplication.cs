using CustomerService.Application.DTOs.Address;
using DotnetBaseKit.Components.Application.Base;

namespace CustomerService.Application.Address;

public interface IAddressServiceApplication : IBaseServiceApplication
{
    Task<IEnumerable<AddressResponseDto>> GetByCustomerIdAsync(Guid myUserId);
    Task<AddressResponseDto> GetByIdAsync(Guid id, Guid myUserId);
    Task CreateAsync(AddressRequestDto dto, Guid myUserId);
    Task UpdateAsync(Guid id, AddressUpdateDto dto, Guid myUserId);
    Task DeleteAsync(Guid id, Guid myUserId);
}
