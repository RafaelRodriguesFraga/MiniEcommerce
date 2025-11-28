using DotnetBaseKit.Components.Application.Base;
using CustomerService.Application.DTOs;

namespace CustomerService.Application;

public interface ICustomerServiceApplication : IBaseServiceApplication
{
    Task<CustomerResponseDto?> GetByUserIdAsync(Guid id);
    Task<CustomerResponseDto> GetOrCreateAsync();
    Task<CustomerResponseDto> UpdateAsync(Guid authServiceId, CustomerUpdateDto dto);
}