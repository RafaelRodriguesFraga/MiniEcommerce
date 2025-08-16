using DotnetBaseKit.Components.Domain.Sql.Entities.Base;

namespace UserService.Domain.Entities;

public class UserProfile : BaseEntity
{
    public UserProfile(Guid userId, string name, string email, string avatarUrl)
    {
        UserId = userId;
        Name = name;
        Email = email;
        AvatarUrl = avatarUrl;
        CreatedAt = DateTime.Now;
        UpdatedAt = DateTime.Now;
    }

    public Guid UserId { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string AvatarUrl { get; set; }
    public DateTime UpdatedAt { get; set; }

    public override void Validate()
    {
    }
}