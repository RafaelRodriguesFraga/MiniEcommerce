using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
namespace AuthService.Shared;

public class JwkHelper
{
    public static string GetRsaThumbprint(RSA rsa)
    {
        var parameters = rsa.ExportParameters(false);
        var jsonWebKey = new JsonWebKey
        {
            Kty = "RSA",
            E = Base64UrlEncoder.Encode(parameters.Exponent),
            N = Base64UrlEncoder.Encode(parameters.Modulus)
        };

        var thumbprint = jsonWebKey.ComputeJwkThumbprint();

        return Base64UrlEncoder.Encode(thumbprint);
    }
};