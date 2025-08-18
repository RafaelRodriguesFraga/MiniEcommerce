using UserService.Application.DTOs;
using UserService.Domain.Entities;

namespace UserService.Application.Extensions;

public static class UserProfileExtensions
{
    public static UserProfile ToEntity(this UserProfileRequestDto dto, Guid userId)
    {
        return new UserProfile
        {
            UserId = userId,
            Name = dto.Name,
            Email = dto.Email,
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