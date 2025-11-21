using System.Security.Cryptography;
using AuthService.Application.Settings;
using AuthService.Shared;
using DotnetBaseKit.Components.Application.Base;
using DotnetBaseKit.Components.Shared.Notifications;
using Microsoft.IdentityModel.Tokens;

namespace AuthService.Application.Services.Token;

public class JwkServiceApplication : BaseServiceApplication, IJwkServiceApplication
{
    private readonly KeySettings _keySettings;
    public JwkServiceApplication(NotificationContext notificationContext, KeySettings keySettings) : base(notificationContext)
    {
        _keySettings = keySettings;
    }

    public object GetJsonWebKeySet()
    {
        var publicKeyText = _keySettings.PublicKey;
        if (string.IsNullOrEmpty(publicKeyText))
        {
            throw new Exception("Public key not configured.");
        }

        var rsa = RSA.Create();
        rsa.ImportFromPem(publicKeyText);

        var rsaParameters = rsa.ExportParameters(false);
        var thumbprint = JwkHelper.GetRsaThumbprint(rsa);

        var jwk = new JsonWebKey
        {
            Kty = "RSA",
            Use = "sig",
            Kid = thumbprint,
            E = Base64UrlEncoder.Encode(rsaParameters.Exponent),
            N = Base64UrlEncoder.Encode(rsaParameters.Modulus)
        };

        return new { keys = new[] { jwk } };
    }
}