using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserService.Application.DTOs
{
    public class UserProfileUpdateDto
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? AvatarUrl { get; set; }
    }
}