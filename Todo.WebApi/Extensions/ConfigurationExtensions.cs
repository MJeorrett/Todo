using Todo.WebApi.Configuration;

namespace Todo.WebApi.Extensions;

public static class ConfigurationExtensions
{
    public static AppIdentityOptions LoadAppIdentityOptions(this IConfiguration configuration)
    {
        const string base64SigningKeyConfigPath = "Identity:Base64SigningKey";
        const string IssuerConfigPath = "Identity:Issuer";

        var base64SigningKey = configuration[base64SigningKeyConfigPath];
        var issuer = configuration[IssuerConfigPath];

        if (string.IsNullOrEmpty(base64SigningKey)) throw new ArgumentException($"No signing key found in configuration at {base64SigningKeyConfigPath}.");
        if (string.IsNullOrEmpty(issuer)) throw new ArgumentException($"No issuer found in configuration at {IssuerConfigPath}.");

        return new AppIdentityOptions(
            base64SigningKey.ParseAsPkcs8PrivateKey(),
            issuer);
    }
}
