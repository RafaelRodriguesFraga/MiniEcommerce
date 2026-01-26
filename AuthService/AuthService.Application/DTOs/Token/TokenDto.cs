using AuthService.Domain.Enums;

namespace AuthService.Application.DTOs;

public class TokenDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public DateTime ExpirationDate { get; set; }
    public DateTime IssuedAt { get; set; }
}