using DotnetBaseKit.Components.Domain.Sql.Entities.Base;

namespace CustomerService.Domain.Entities;

public class Customer : BaseEntity
{
    public Customer()
    {

    }
    public Customer(Guid authServiceId, string firstName, string lastName, string email, string cpf, string phone, string? avatarUrl = null)
    {
        AuthServiceId = authServiceId;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Cpf = cpf;
        Phone = phone;
        AvatarUrl = avatarUrl;
        UpdatedAt = DateTime.UtcNow;
    }
    public Guid AuthServiceId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Cpf { get; set; }
    public string Phone { get; set; }
    public string? AvatarUrl { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<Address> Addresses = new List<Address>();
    public override void Validate()
    {
    }

    public void Update(string firstName, string lastName, string cpf, string phone, string avatarUrl)
    {
        FirstName = firstName ?? FirstName;
        LastName = lastName ?? LastName;
        Cpf = cpf ?? Cpf;
        Phone = phone ?? Phone;
        AvatarUrl = avatarUrl ?? AvatarUrl;
        UpdatedAt = DateTime.UtcNow;
    }
}