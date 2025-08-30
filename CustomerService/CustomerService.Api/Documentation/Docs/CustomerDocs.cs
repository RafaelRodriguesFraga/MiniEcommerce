using CustomerService.Api.Documentation.Configuration;
using CustomerService.Api.Documentation.Docs.Keys;
using CustomerService.Application.DTOs;
using DotnetBaseKit.Components.Api.Responses;

namespace CustomerService.Api.Documentation.Docs;

public class CustomerDocs : BaseDoc<CustomerDocKey>
{
    static CustomerDocs()
    {
        Docs[CustomerDocKey.GetMe] = new SwaggerDocumentationInfo(
           summary: "Get the logged user data",
           description: "Returns the details of the customer associated to the logged user")
        {
            Responses = new (int, string, Type?, object?)[]
           {
                (200, "Success", typeof(CustomerResponseDto), new CustomerResponseDto { Id = Guid.NewGuid(), Name = "Name", Email = "example@example.com", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now }),
                (400, "Bad Request", typeof(Response), new {success = false, errors = new List<string>() {"error"}}),
                (401, "Unauthorized", null, null)
           }
        };

        Docs[CustomerDocKey.Create] = new SwaggerDocumentationInfo(
             summary: "Creates a new customer",
             description: "Creates a customer associated with the logged-in user using the provided information")
        {
            RequestExample = new { AvatarUrl = "http://exampleurl.com/avatar.jpg" },
            Responses = new (int, string, Type?, object?)[]
            {
                (201, "Customer successfully created", typeof(CustomerResponseDto), new CustomerResponseDto { Id = Guid.NewGuid(), Name = "Rafael", Email = "rafael@example.com" }),
                (400, "Bad Request", typeof(Response), new {success = false, errors = new List<string>() {"error"}}),
                (401, "Unauthorized", null, null)
            }
        };

        Docs[CustomerDocKey.Update] = new SwaggerDocumentationInfo(
             summary: "Updates a new customer",
             description: "Updates a customer")
        {
            RequestExample = new CustomerUpdateDto { AvatarUrl = "http://exampleurl.com/avatar.jpg" },
            Responses = new (int, string, Type?, object?)[]
            {
                (200, "Success", typeof(Response), new {success = true, errors = new List<string>() {} }),
                (400, "Bad Request", typeof(Response), new {success = false, errors = new List<string>() {"error"}}),
                (401, "Unauthorized", null, null)
            }
        };
    }
}