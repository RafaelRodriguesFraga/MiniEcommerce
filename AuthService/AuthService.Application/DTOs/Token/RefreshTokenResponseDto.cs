using Microsoft.Identity.Client;

namespace AuthService.Application.DTOs.Token
{
    public class RefreshTokenResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime ExpirationDate { get; set; }
        public DateTime IssuedAt { get; set; }
    }
}