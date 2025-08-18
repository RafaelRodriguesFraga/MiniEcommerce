namespace UserService.Application.DTOs;

public class UserProfileRequestDto
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string AvatarUrl { get; set; }
}