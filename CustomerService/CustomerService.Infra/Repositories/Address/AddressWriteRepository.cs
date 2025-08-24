namespace CustomerService.Infra.Repositories.Address;

public class AddressWriteRepository : BaseWriteRepository<AddressEntity>, IAddressWriteRepository
{
    public AddressWriteRepository(BaseContext context) : base(context)
    {
    }
}