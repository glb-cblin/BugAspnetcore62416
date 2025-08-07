using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace BugAspnetcore62416;

[ApiController]    
public class FakeOpenidConnectToken(JwksOptions jwksOptions) : ControllerBase
{
    [HttpGet("/faketoken")]
    public FakeOidcToken GetConfig()
    {
        var jwt = new JsonWebTokenHandler().CreateToken(new SecurityTokenDescriptor
        {
            Issuer = "http://localhost:5001",
            Audience = "dummyaudience",
            Subject = new ClaimsIdentity([
                new Claim("id", "TheUserId")                
            ]),
            NotBefore = DateTime.UtcNow,
            Expires = DateTime.UtcNow.AddMinutes(30),
            IssuedAt = DateTime.UtcNow,
            SigningCredentials = new SigningCredentials(
                new RsaSecurityKey(jwksOptions.GetPrivateRsaParameters()){KeyId = JwksOptions.KeyId }, 
                SecurityAlgorithms.RsaSha256),
            IncludeKeyIdInHeader = true,
        });

        return new FakeOidcToken()
        {
            access_token = jwt,
            token_type = "Bearer",
            expires_in = TimeSpan.FromMinutes(30).TotalSeconds
        };
    }
}

public class FakeOidcToken
{
    public string access_token { get; set; }

    public string token_type { get; set; }

    public double expires_in { get; set; }
}