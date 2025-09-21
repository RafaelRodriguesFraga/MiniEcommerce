namespace CustomerService.Infra.Repositories;

public class CustomerReadRepository : BaseReadRepository<Customer>, ICustomerReadRepository
{
    public CustomerReadRepository(BaseContext context) : base(context)
    {
    }

    public async Task<Customer?> GetByIdAndAuthServiceIdAsync(Guid customerId, Guid authServiceId)
    {
        return await Set
            .Where(c => c.Id == customerId && c.AuthServiceId == authServiceId)
            .FirstOrDefaultAsync();
    }
}