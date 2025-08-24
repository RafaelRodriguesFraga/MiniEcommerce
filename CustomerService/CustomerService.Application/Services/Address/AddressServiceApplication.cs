using CustomerService.Application.Address;
using DotnetBaseKit.Components.Application.Base;
using DotnetBaseKit.Components.Shared.Notifications;

namespace CustomerService.Application.Services.Address;

public class AddressServiceApplication : BaseServiceApplication, IAddressServiceApplication
{
    public AddressServiceApplication(NotificationContext notificationContext) : base(notificationContext)
    {
    }
}