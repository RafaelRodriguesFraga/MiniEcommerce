using DotnetBaseKit.Components.Application.Base;
using DotnetBaseKit.Components.Shared.Notifications;
using CustomerService.Application.DTOs;
using CustomerService.Application.Extensions;
using CustomerService.Domain.Entities;
using CustomerService.Domain.Repositories;
using CustomerService.Application.Interfaces;

namespace CustomerService.Application;

public class CustomerServiceApplication : BaseServiceApplication, ICustomerServiceApplication
{
    private readonly ICustomerReadRepository _customerReadRepository;
    private readonly ICustomerWriteRepository _customerWriteRepository;

    private readonly IUserContext _userContext;

    public object HttpContext { get; private set; }

    public CustomerServiceApplication(NotificationContext notificationContext,
        ICustomerReadRepository customerReadRepository,
        ICustomerWriteRepository customerWriteRepository
,
        IUserContext userContext) : base(notificationContext)
    {
        _customerReadRepository = customerReadRepository;
        _customerWriteRepository = customerWriteRepository;
        _userContext = userContext;
    }

    public async Task<CustomerResponseDto?> GetByUserIdAsync(Guid id)
    {
        var customer = await _customerReadRepository.GetByAuthServiceIdAsync(id);

        return customer?.ToDto();
    }

    public async Task<CustomerResponseDto> GetOrCreateAsync()
    {
        var userId = Guid.Parse(_userContext.UserId);
        if (userId == Guid.Empty)
        {
            _notificationContext.AddNotification("Guid", "UserId is invalid");
            return default!;
        }

        var customer = await _customerReadRepository.GetByAuthServiceIdAsync(userId);

        if (customer == null)
        {
            var nameFromToken = _userContext.Name ?? "Usuário";
            var emailFromToken = _userContext.Email ?? "";
            var names = nameFromToken.Split(' ', 2);

            var firstName = names[0];
            var lastName = names.Length > 1 ? names[1] : "";

            customer = new Customer(
                userId,
                firstName,
                lastName,
                emailFromToken,
                string.Empty,
                string.Empty,
                null);

            await _customerWriteRepository.InsertAsync(customer);
        }

        return customer.ToDto();
    }
    public async Task<CustomerResponseDto> UpdateAsync(Guid authServiceId, CustomerUpdateDto dto)
    {
        var customer = await _customerReadRepository.GetByAuthServiceIdAsync(authServiceId);
        if (customer == null)
        {
            _notificationContext.AddNotification("Customer", "Customer not found for this user");
            return default!;
        }

        customer.Update(dto.FirstName, dto.LastName, dto.Cpf, dto.Phone, dto.AvatarUrl);

        await _customerWriteRepository.UpdateAsync(customer);

        return customer.ToDto();
    }
}