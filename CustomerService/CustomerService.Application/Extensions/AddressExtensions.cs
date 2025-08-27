using CustomerService.Application.DTOs.Address;
using AddressEntity = CustomerService.Domain.Entities.Address;

namespace CustomerService.Application.Extensions;

public static class AddressExtensions
{
    public static AddressEntity ToEntity(this AddressRequestDto dto)
    {
        return new AddressEntity(dto.CustomerId, dto.Street, dto.Number, dto.Complement, dto.Neighborhood, dto.City, dto.State, dto.PostalCode);
    }

    public static AddressResponseDto ToDto(this AddressEntity entity)
    {

        return new AddressResponseDto
        {
            Id = entity.Id,
            CustomerId = entity.CustomerId,
            Street = entity.Street,
            Number = entity.Number,
            Neighborhood = entity.Neighborhood,
            City = entity.City,
            State = entity.State,
            ZipCode = entity.PostalCode,
            Complement = entity.Complement
        };
    }
}