using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using DotnetBaseKit.Components.Application.Base;
using DotnetBaseKit.Components.Shared.Notifications;
using Microsoft.AspNetCore.Http;
using CustomerService.Application.DTOs;
using CustomerService.Application.Extensions;
using CustomerService.Domain.Entities;
using CustomerService.Domain.Repositories;

namespace CustomerService.Application;

public class CustomerServiceApplication : BaseServiceApplication, ICustomerServiceApplication
{
    private readonly ICustomerReadRepository _customerReadRepository;
    private readonly ICustomerWriteRepository _customerWriteRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public object HttpContext { get; private set; }

    public CustomerServiceApplication(NotificationContext notificationContext,
        ICustomerReadRepository customerReadRepository,
        ICustomerWriteRepository customerWriteRepository,
        IHttpContextAccessor httpContextAccessor) : base(notificationContext)
    {
        _customerReadRepository = customerReadRepository;
        _customerWriteRepository = customerWriteRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<CustomerResponseDto> GetByUserIdAsync(Guid id)
    {
        var user = await _customerReadRepository.GetByIdAsync(id);
        if (user == null)
        {
            var userName = _httpContextAccessor.HttpContext.User.FindFirst(JwtRegisteredClaimNames.Name)?.Value ?? "";
            var userEmail = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Email)?.Value ?? "";

            var customer = new Customer(id, userName, userEmail, string.Empty);
            await _customerWriteRepository.InsertAsync(customer);

            return customer.ToDto();
        }



        return user.ToDto();
    }

    public async Task<CustomerResponseDto> CreateAsync(CustomerRequestDto dto, Guid userId, string userName, string userEmail)
    {
        dto.Validate();

        if (dto.Invalid)
        {
            _notificationContext.AddNotifications(dto.Notifications);
            return default!;
        }

        var customer = dto.ToEntity(userId, userName, userEmail);

        await _customerWriteRepository.InsertAsync(customer);

        return customer.ToDto();
    }

    public async Task<CustomerResponseDto> UpdateAsync(Guid customerId, Guid authServiceId, CustomerUpdateDto dto)
    {
        var customer = await _customerReadRepository.GetByIdAndAuthServiceIdAsync(customerId, authServiceId);
        if (customer == null)
        {
            _notificationContext.AddNotification("Customer", "Customer not found for this user");
            return default!;
        }

        customer.Update(dto.Name, dto.Email, dto.AvatarUrl);

        await _customerWriteRepository.UpdateAsync(customer);

        return customer.ToDto();
    }
}