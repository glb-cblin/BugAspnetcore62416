using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace BugAspnetcore62416;

[ApiController]    
public class FakeOpenidConnectWellKnown(JwksOptions jwksOptions) : ControllerBase
{
    [HttpGet(".well-known/openid-configuration")]
    public FakeOidcConfig GetConfig()
    {
        return new FakeOidcConfig();
    }

    //uncomment this and comment the previous method to repro the bug 
    //[HttpGet(".well-known/openid-configuration")]
    //public OpenIdConnectConfiguration GetConfig()
    //{
    //    return new OpenIdConnectConfiguration
    //    {
    //        Issuer = "http://localhost:5001",
    //        JwksUri = "http://localhost:5001/.well-known/jwks.json"
    //    };
    //}

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