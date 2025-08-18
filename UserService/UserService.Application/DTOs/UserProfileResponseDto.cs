using UserService.Domain.Entities;

namespace UserService.Application.DTOs;

public class UserProfileResponseDto
{
    public UserProfileResponseDto()
    {

    }


    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }
    public string AvatarUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
}