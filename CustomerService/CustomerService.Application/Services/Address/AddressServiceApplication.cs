using CustomerService.Application.Address;
using CustomerService.Application.DTOs.Address;
using CustomerService.Application.Extensions;
using CustomerService.Domain.Repositories.Address;
using DotnetBaseKit.Components.Application.Base;
using DotnetBaseKit.Components.Shared.Notifications;

namespace CustomerService.Application.Services.Address;

public class AddressServiceApplication : BaseServiceApplication, IAddressServiceApplication
{
    private readonly IAddressReadRepository _addressReadRepository;
    private readonly IAddressWriteRepository _addressWriteRepository;

    public AddressServiceApplication(NotificationContext notificationContext, IAddressReadRepository addressReadRepository, IAddressWriteRepository addressWriteRepository) : base(notificationContext)
    {
        _addressReadRepository = addressReadRepository;
        _addressWriteRepository = addressWriteRepository;
    }

    public async Task CreateAsync(AddressRequestDto dto)
    {
        // TODO VALIDATE

        var address = dto.ToEntity();

        await _addressWriteRepository.InsertAsync(address);
    }

    public async Task DeleteAsync(Guid id)
    {
        var address = await _addressReadRepository.GetByIdAsync(id);
        if (address == null)
        {
            _notificationContext.AddNotification("Address", "Address not found");
            return;
        }

        await _addressWriteRepository.DeleteAsync(address);
    }

    public async Task<IEnumerable<AddressResponseDto>> GetByCustomerIdAsync(Guid customerId)
    {
        var addresses = await _addressReadRepository.GetByCustomerIdAsync(customerId);

        return addresses.Select(a => a.ToDto());
    }

    public async Task<AddressResponseDto> GetByIdAsync(Guid id)
    {
        var address = await _addressReadRepository.GetByIdAsync(id);
        if (address == null)
        {
            _notificationContext.AddNotification("Address", "Address not found");
            return default!;
        }

        return address.ToDto();
    }

    public async Task UpdateAsync(Guid id, AddressUpdateDto dto)
    {
        var address = await _addressReadRepository.GetByIdAsync(id);
        if (address == null)
        {
            _notificationContext.AddNotification("Address", "Address not found");
            return;
        }

        address.Update(dto.Street, dto.Number, dto.Complement, dto.Neighborhood, dto.City, dto.State, dto.PostalCode);
        await _addressWriteRepository.UpdateAsync(address);
    }

}
