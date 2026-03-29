using System.Security.Cryptography;

namespace AuthService.Shared
{
    public static class TokenHasher
    {
        public static string HashToken(string token)
        {
            using var sha256 = SHA256.Create();
            var bytes = System.Text.Encoding.UTF8.GetBytes(token);

            var hash = sha256.ComputeHash(bytes);

            return Convert.ToHexString(hash);
        }
    }
}