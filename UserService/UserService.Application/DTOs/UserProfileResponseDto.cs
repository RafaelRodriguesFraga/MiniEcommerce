using UserService.Domain.Entities;

namespace UserService.Application.DTOs;

public class UserProfileResponseDto
{
    public UserProfileResponseDto(Guid id, string email, string name, string avatarUrl, DateTime createdAt)
    {
        Id = id;
        Email = email;
        Name = name;
        AvatarUrl = avatarUrl;
        CreatedAt = createdAt;
    }

    public Guid Id { get; }
    public string Email { get; }
    public string Name { get; }
    public string AvatarUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public static implicit operator UserProfileResponseDto(UserProfile userProfile)
    {
        return new UserProfileResponseDto(userProfile.Id, userProfile.Name, userProfile.Email, userProfile.AvatarUrl,
            userProfile.CreatedAt);
    }
}