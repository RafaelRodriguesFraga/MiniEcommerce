
namespace CustomerService.Infra.Repositories.Address;

public class AddressReadRepository : BaseReadRepository<AddressEntity>, IAddressReadRepository
{
    public AddressReadRepository(BaseContext context) : base(context)
    {
    }

    public async Task<IEnumerable<AddressEntity>> GetByCustomerIdAsync(Guid customerId)
    {
        return await Set
            .Where(p => p.CustomerId == customerId)
            .ToListAsync();
    }
}