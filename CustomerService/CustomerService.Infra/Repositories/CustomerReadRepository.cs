namespace CustomerService.Infra.Repositories;

public class CustomerReadRepository : BaseReadRepository<Customer>, ICustomerReadRepository
{
    public CustomerReadRepository(BaseContext context) : base(context)
    {
    }

    public async Task<Customer?> GetByUserIdAsync(Guid userId)
    {
        return await Set.Where(p => p.AuthServiceId == userId).FirstOrDefaultAsync();
    }
}