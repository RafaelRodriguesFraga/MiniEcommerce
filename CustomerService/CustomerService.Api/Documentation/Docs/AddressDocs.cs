using CustomerService.Api.Documentation.Configuration;
using CustomerService.Api.Documentation.Docs.Keys;
using CustomerService.Application.DTOs.Address;
using DotnetBaseKit.Components.Api.Responses;

namespace CustomerService.Api.Documentation.Docs;

public class AddressDocs : BaseDoc<AddressDocKey>
{
    static AddressDocs()
    {
        Docs[AddressDocKey.GetById] = new SwaggerDocumentationInfo(
                        summary: "Get an address by ID",
                        description: "Returns the details of the specified address")
        {
            Responses = new (int, string, Type?, object?)[]
            {
                  (200, "Success", typeof(AddressResponseDto), new
                    {
                        id = Guid.NewGuid(),
                        customerId = Guid.NewGuid(),
                        street = "Av. Paulista",
                        number = "1000",
                        neighborhood = "Bela Vista",
                        city = "São Paulo",
                        state = "SP",
                        postalCode = "01310-100",
                        complement = "Apto 101",
                        createdAt = DateTime.Now,
                        updatedAt = DateTime.Now
                    }),
                    (400, "Bad Request", typeof(Response), new { success = false, errors = new[] { "error" } }),
                    (401, "Unauthorized", null, null)
            }
        };

        Docs[AddressDocKey.GetAllByCustomerId] = new SwaggerDocumentationInfo(
            summary: "Get addresses for a customer",
            description: "Returns all addresses associated with the specified customer")
        {
            Responses = new (int, string, Type?, object?)[]
            {
                (200, "Success", typeof(IEnumerable<AddressResponseDto>), new[]
                {
                    new
                    {
                        id = Guid.NewGuid(),
                        customerId = Guid.NewGuid(),
                        street = "Av. Paulista",
                        number = "1000",
                        neighborhood = "Bela Vista",
                        city = "São Paulo",
                        state = "SP",
                        postalCode = "01310-100",
                        complement = "Apto 101",
                        createdAt = DateTime.Now,
                        updatedAt = DateTime.Now
                    }
                }),
                (400, "Bad Request", typeof(Response), new { success = false, errors = new[] { "error" } }),
                (401, "Unauthorized", null, null)
            }
        };

        Docs[AddressDocKey.Create] = new SwaggerDocumentationInfo(
            summary: "Create a new address",
            description: "Creates a new address for the specified customer")
        {
            RequestExample = new
            {
                customerId = Guid.NewGuid(),
                street = "Av. Paulista",
                number = "1000",
                neighborhood = "Bela Vista",
                city = "São Paulo",
                state = "SP",
                postalCode = "01310-100",
                complement = "Apto 101"
            },
            Responses = new (int, string, Type?, object?)[]
            {
                (201, "Address successfully created", typeof(AddressResponseDto), new
                {
                    id = Guid.NewGuid(),
                    customerId = Guid.NewGuid(),
                    street = "Av. Paulista",
                    number = "1000",
                    neighborhood = "Bela Vista",
                    city = "São Paulo",
                    state = "SP",
                    postalCode = "01310-100",
                    complement = "Apto 101",
                    createdAt = DateTime.Now,
                    updatedAt = DateTime.Now
                }),
                (400, "Bad Request", typeof(Response), new { success = false, errors = new[] { "error" } }),
                (401, "Unauthorized", null, null)
            }
        };

        Docs[AddressDocKey.Update] = new SwaggerDocumentationInfo(
            summary: "Update an address",
            description: "Updates the details of the specified address")
        {
            RequestExample = new
            {
                street = "Av. Paulista",
                number = "1000",
                neighborhood = "Bela Vista",
                city = "São Paulo",
                state = "SP",
                postalCode = "01310-100",
                complement = "Apto 102"
            },
            Responses = new (int, string, Type?, object?)[]
            {
                (200, "Success", typeof(Response), new { success = true }),
                (400, "Bad Request", typeof(Response), new { success = false, errors = new[] { "error" } }),
                (401, "Unauthorized", null, null)
            }
        };

        Docs[AddressDocKey.Delete] = new SwaggerDocumentationInfo(
            summary: "Delete an address",
            description: "Deletes the specified address")
        {
            Responses = new (int, string, Type?, object?)[]
            {
                (200, "Success", typeof(Response), new { success = true }),
                (400, "Bad Request", typeof(Response), new { success = false, errors = new[] { "error" } }),
                (401, "Unauthorized", null, null)
            }
        };
    }
}
