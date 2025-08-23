using DotnetBaseKit.Components.Application.Base;
using CustomerService.Application.DTOs;

namespace CustomerService.Application;

public interface ICustomerServiceApplication : IBaseServiceApplication
{
    Task<CustomerResponseDto> GetByUserIdAsync(Guid id);
    Task<CustomerResponseDto> CreateAsync(CustomerRequestDto dto, Guid userId, string userName, string userEmail);
    Task<CustomerResponseDto> UpdateAsync(Guid id, CustomerUpdateDto dto);
}