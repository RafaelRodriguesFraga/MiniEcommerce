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
    public string Street { get; private set; } = string.Empty;
    public string Number { get; private set; } = string.Empty;
    public string Complement { get; private set; } = string.Empty;
    public string Neighborhood { get; private set; } = string.Empty;
    public string City { get; private set; } = string.Empty;
    public string State { get; private set; } = string.Empty;
    public string PostalCode { get; private set; } = string.Empty;
    public DateTime UpdatedAt { get; private set; }

    public void Update(string street, string number, string complement, string neighborhood, string city, string state, string postalCode)
    {
        Street = street ?? Street;
        Number = number ?? Number;
        Complement = complement ?? Complement;
        Neighborhood = neighborhood ?? Neighborhood;
        City = city ?? City;
        State = state ?? State;
        PostalCode = postalCode ?? PostalCode;
        UpdatedAt = DateTime.Now;
    }

    public override void Validate()
    {
    }
}