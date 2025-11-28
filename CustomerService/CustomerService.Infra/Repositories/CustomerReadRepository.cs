namespace CustomerService.Infra.Repositories;

public class CustomerReadRepository : BaseReadRepository<Customer>, ICustomerReadRepository
{
    public CustomerReadRepository(BaseContext context) : base(context)
    {
    }

    public async Task<Customer?> GetByAuthServiceIdAsync(Guid authServiceId)
    {
        return await Set
            .Where(c => c.AuthServiceId == authServiceId)
            .FirstOrDefaultAsync();
    }
}