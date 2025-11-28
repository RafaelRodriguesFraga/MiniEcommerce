using CustomerService.Application.Address;
using CustomerService.Application.DTOs.Address;
using CustomerService.Application.Extensions;
using CustomerService.Domain.Repositories;
using CustomerService.Domain.Repositories.Address;
using DotnetBaseKit.Components.Application.Base;
using DotnetBaseKit.Components.Shared.Notifications;

namespace CustomerService.Application.Services.Address;

public class AddressServiceApplication : BaseServiceApplication, IAddressServiceApplication
{
    private readonly IAddressReadRepository _addressReadRepository;
    private readonly IAddressWriteRepository _addressWriteRepository;
    private readonly ICustomerReadRepository _customerReadRepository;

    public AddressServiceApplication(
        NotificationContext notificationContext,
        IAddressReadRepository addressReadRepository,
        IAddressWriteRepository addressWriteRepository,

        ICustomerReadRepository customerReadRepository) : base(notificationContext)
    {
        _addressReadRepository = addressReadRepository;
        _addressWriteRepository = addressWriteRepository;
        _customerReadRepository = customerReadRepository;
    }

    public async Task<IEnumerable<AddressResponseDto>> GetByCustomerIdAsync(Guid myUserId)
    {
        var customerId = await ResolveCustomerIdOrNotifyAsync(myUserId);
        if (customerId == Guid.Empty)
            return Enumerable.Empty<AddressResponseDto>();

        var addresses = await _addressReadRepository.GetByCustomerIdAsync(customerId);

        return addresses.Select(a => a.ToDto());
    }


    public async Task<AddressResponseDto> GetByIdAsync(Guid id, Guid myUserId)
    {
        var customerId = await ResolveCustomerIdOrNotifyAsync(myUserId);
        if (customerId == Guid.Empty)
            return default!;

        var address = await _addressReadRepository.GetByIdAsync(id);

        if (address == null)
        {
            _notificationContext.AddNotification("Address", "Address not found");
            return default!;
        }

        if (address.CustomerId != customerId)
        {
            _notificationContext.AddNotification("Address.Forbidden", "You cannot access another user's address");
            return default!;
        }

        return address.ToDto();
    }

    public async Task CreateAsync(AddressRequestDto dto, Guid myUserId)
    {
        // TODO: Validate dto if needed

        var customerId = await ResolveCustomerIdOrNotifyAsync(myUserId);
        if (customerId == Guid.Empty)
            return;

        var address = dto.ToEntity(customerId);

        await _addressWriteRepository.InsertAsync(address);
    }

    public async Task UpdateAsync(Guid id, AddressUpdateDto dto, Guid myUserId)
    {

        var customerId = await ResolveCustomerIdOrNotifyAsync(myUserId);
        if (customerId == Guid.Empty)
            return;
        var address = await _addressReadRepository.GetByIdAsync(id);

        if (address == null)
        {
            _notificationContext.AddNotification("Address.NotFound", "Address not found");
            return;
        }

        if (address.CustomerId != customerId)
        {
            _notificationContext.AddNotification("Address.Forbidden", "You cannot update another user's address");
            return;
        }

        address.Update(
            dto.Street,
            dto.Number,
            dto.Complement,
            dto.Neighborhood,
            dto.City,
            dto.State,
            dto.PostalCode
        );

        await _addressWriteRepository.UpdateAsync(address);
    }

    public async Task DeleteAsync(Guid id, Guid myUserId)
    {
        var customerId = await ResolveCustomerIdOrNotifyAsync(myUserId);
        if (customerId == Guid.Empty)
            return;

        var address = await _addressReadRepository.GetByIdAsync(id);

        if (address == null)
        {
            _notificationContext.AddNotification("Address.NotFound", "Address not found");
            return;
        }

        if (address.CustomerId != customerId)
        {
            _notificationContext.AddNotification("Address.Forbidden", "You cannot delete another user's address");
            return;
        }

        await _addressWriteRepository.DeleteAsync(address);
    }

    private async Task<Guid> ResolveCustomerIdOrNotifyAsync(Guid authUserId)
    {
        var customer = await _customerReadRepository.GetByAuthServiceIdAsync(authUserId);
        if (customer == null)
        {
            _notificationContext.AddNotification("Customer", "Customer not found");
            return default;
        }

        return customer.Id;
    }
}
