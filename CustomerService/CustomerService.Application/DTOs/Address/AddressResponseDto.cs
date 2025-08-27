namespace CustomerService.Application.DTOs.Address
{
    public class AddressResponseDto
    {
        public Guid Id { get; set; }

        public Guid CustomerId { get; set; }

        public string Street { get; set; } = string.Empty;

        public string Number { get; set; } = string.Empty;

        public string Neighborhood { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;

        public string State { get; set; } = string.Empty;

        public string ZipCode { get; set; } = string.Empty;

        public string? Complement { get; set; }
    }
}