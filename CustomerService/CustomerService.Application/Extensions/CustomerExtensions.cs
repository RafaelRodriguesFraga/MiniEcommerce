using CustomerService.Application.DTOs;
using CustomerService.Domain.Entities;

namespace CustomerService.Application.Extensions;

public static class CustomerExtensions
{
    public static Customer ToEntity(this CustomerRequestDto dto, Guid userId, string firstName, string lastName, string userEmail)
    {
        return new Customer
        {
            AuthServiceId = userId,
            FirstName = firstName,
            LastName = lastName,
            Email = userEmail,
            AvatarUrl = dto.AvatarUrl
        };
    }

    public static CustomerResponseDto ToDto(this Customer entity)
    {

        return new CustomerResponseDto
        {
            UserId = entity.AuthServiceId,
            Id = entity.Id,
            Email = entity.Email,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            AvatarUrl = entity.AvatarUrl,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }
}