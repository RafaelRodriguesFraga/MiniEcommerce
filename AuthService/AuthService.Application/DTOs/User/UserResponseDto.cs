using AuthService.Domain.Enums;

namespace AuthService.Application.DTOs.User;

public class UserResponseDto
{
    public UserResponseDto(Guid id, string name, string email, UserRole role)
    {
        Id = id;
        Name = name;
        Email = email;
        Role = role;
    }


    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public UserRole Role { get; set; }

    public static implicit operator UserResponseDto(Domain.Entities.User user)
    {
        return new UserResponseDto(user.Id, user.Name, user.Email, user.Role);
    }
}