using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

namespace Todo.Infrastructure.Extensions;

public static class StringExtensions
{
    public static RsaSecurityKey ParseAsPkcs8PrivateKey(this string str)
    {
        var signingKeyBytes = Convert.FromBase64String(str);
        var signingKeyRsa = RSA.Create();
        signingKeyRsa.ImportPkcs8PrivateKey(signingKeyBytes, out var _);

        return new RsaSecurityKey(signingKeyRsa);
    }
}
