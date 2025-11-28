using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerService.Application.Interfaces
{
    public interface IUserContext
    {
        string? UserId { get; }
        string? Name { get; }
        string? Email { get; }
    }
}