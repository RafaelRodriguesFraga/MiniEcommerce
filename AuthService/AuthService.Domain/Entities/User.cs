using AuthService.Domain.Enums;
using DotnetBaseKit.Components.Domain.Sql.Entities.Base;

namespace AuthService.Domain.Entities;

public class User : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string Password { get; private set; } = string.Empty;
    public DateTime UpdatedAt { get; private set; }
    public UserRole Role { get; set; }

    public User()
    {
    }

    public User(string name, string email, string password)
    {
        Id = Guid.NewGuid();
        Name = name;
        Email = email;
        Password = password;
        CreatedAt = DateTime.Now;
        UpdatedAt = DateTime.Now;
    }

    public static User Create(string name, string email, string password)
    {
        return new User(name, email, password);
    }

    public override void Validate()
    {
    }

    public void SetPassword(string newPassword)
    {
        Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
        UpdatedAt = DateTime.UtcNow;
    }
}