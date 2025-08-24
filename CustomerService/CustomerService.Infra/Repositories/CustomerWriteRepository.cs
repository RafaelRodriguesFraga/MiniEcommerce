namespace CustomerService.Infra.Repositories;

public class CustomerWriteRepository : BaseWriteRepository<Customer>, ICustomerWriteRepository
{
    public CustomerWriteRepository(BaseContext context) : base(context)
    {
    }
}