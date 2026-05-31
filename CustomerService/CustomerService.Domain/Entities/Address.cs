using DotnetBaseKit.Components.Domain.Sql.Entities.Base;

namespace CustomerService.Domain.Entities;

public class Address : BaseEntity
{

    public Address(Guid customerId, string street, string number, string complement, string neighborhood, string city,
        string state, string postalCode, bool isMain, string label)
    {
        CustomerId = customerId;
        Street = street;
        Number = number;
        Complement = complement;
        Neighborhood = neighborhood;
        City = city;
        State = state;
        PostalCode = postalCode;
        IsMain = isMain;
        Label = label;
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
    public string Label { get; private set; }
    public bool IsMain { get; private set; }

    public void Update(string street, string number, string complement, string neighborhood, string city, string state, string postalCode, string label)
    {
        Street = street ?? Street;
        Number = number ?? Number;
        Complement = complement ?? Complement;
        Neighborhood = neighborhood ?? Neighborhood;
        City = city ?? City;
        State = state ?? State;
        PostalCode = postalCode ?? PostalCode;
        Label = label ?? Label;
        UpdatedAt = DateTime.Now;
    }

    public void SetAsMain() => IsMain = true;
    public void UnsetMain() => IsMain = false;

    public override void Validate()
    {
    }
}