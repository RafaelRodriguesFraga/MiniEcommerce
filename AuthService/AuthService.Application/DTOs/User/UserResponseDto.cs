namespace AuthService.Application.DTOs.User;

public class UserResponseDto
{
    public UserResponseDto(Guid id, string name, string email)
    {
        Id = id;
        Name = name;
        Email = email;
    }


    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }

    public static implicit operator UserResponseDto(Domain.Entities.User user)
    {
        return new UserResponseDto(user.Id, user.Name, user.Email);
    }
}