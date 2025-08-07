using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace BugAspnetcore62416;

[ApiController]    
public class FakeOpenidConnectWellKnown(JwksOptions jwksOptions) : ControllerBase
{
    [HttpGet(".well-known/openid-configration")]
    public FakeOidcConfig GetConfig()
    {
        return new FakeOidcConfig();
    }

    [HttpGet(".well-known/jwks.json")]
    public JsonWebKeySet GetJwks()
    {
        return jwksOptions.GetJwks();
    }
}

public class FakeOidcConfig
{
    public string issuer { get; set; } = "http://localhost:5001";

    public string jwks_uri { get; set; } = "http://localhost:5001/.well-known/jwks.json";
}