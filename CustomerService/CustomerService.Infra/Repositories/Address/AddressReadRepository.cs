namespace CustomerService.Infra.Repositories.Address;

public class AddressReadRepository : BaseReadRepository<AddressEntity>, IAddressReadRepository
{
    public AddressReadRepository(BaseContext context) : base(context)
    {
    }
    
}