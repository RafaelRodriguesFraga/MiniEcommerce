using DotnetBaseKit.Components.Domain.Sql.Entities.Base;

namespace CustomerService.Domain.Entities;

public class Address : BaseEntity
{
    public Address(Guid customerId, string street, string number, string complement, string neighborhood, string city,
        string state, string postalCode)
    {
        CustomerId = customerId;
        Street = street;
        Number = number;
        Complement = complement;
        Neighborhood = neighborhood;
        City = city;
        State = state;
        PostalCode = postalCode;
        CreatedAt = DateTime.Now;
        UpdatedAt = DateTime.Now;
    }


    public Guid CustomerId { get; private set; }
    public Customer Customer { get; private set; }
    public string Street { get; private set; }
    public string Number { get; private set; }
    public string Complement { get; private set; }
    public string Neighborhood { get; private set; }
    public string City { get; private set; }
    public string State { get; private set; }
    public string PostalCode { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public override void Validate()
    {
    }
}