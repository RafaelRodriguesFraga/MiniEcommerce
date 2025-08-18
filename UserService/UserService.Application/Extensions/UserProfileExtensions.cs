using UserService.Application.DTOs;
using UserService.Domain.Entities;

namespace UserService.Application.Extensions;

public static class UserProfileExtensions
{
    public static UserProfile ToEntity(this UserProfileRequestDto dto, Guid userId, string userName, string userEmail)
    {
        return new UserProfile
        {
            UserId = userId,
            Name = userName,
            Email = userEmail,
            AvatarUrl = dto.AvatarUrl
        };
    }

    public static UserProfileResponseDto ToDto(this UserProfile entity)
    {

        return new UserProfileResponseDto
        {
            UserId = entity.UserId,
            Id = entity.Id,
            Email = entity.Email,
            Name = entity.Name,
            AvatarUrl = entity.AvatarUrl,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }
}