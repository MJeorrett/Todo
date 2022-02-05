using Microsoft.IdentityModel.Tokens;

namespace Todo.WebApi.Configuration;

public record AppIdentityOptions(
    RsaSecurityKey SigningKey,
    string Issuer)
{
    
}
