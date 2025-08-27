namespace CustomerService.Application.DTOs.Address
{
    public class AddressUpdateDto
    {
        public Guid Id { get; set; }
        public string Street { get; set; } = string.Empty;
        public string Number { get; private set; } = string.Empty;
        public string Complement { get; private set; } = string.Empty;
        public string Neighborhood { get; private set; } = string.Empty;
        public string City { get; private set; } = string.Empty;
        public string State { get; private set; } = string.Empty;
        public string PostalCode { get; private set; } = string.Empty;
    }
}