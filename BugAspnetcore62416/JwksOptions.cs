using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

namespace BugAspnetcore62416;

public class JwksOptions
{
    public const string KeyId = "K1";
    public string PublicKey { get; set; }

    public string PrivateKey { get; set; }

    public RSAParameters GetPublicRsaParameters()
    {
        using (var rsa = new RSACryptoServiceProvider())
        {
            rsa.ImportFromPem(PublicKey);
            return rsa.ExportParameters(includePrivateParameters: false);
        }
    }

    public JsonWebKeySet GetJwks()
    {
        var parameters = GetPublicRsaParameters();
        return new JsonWebKeySet()
        {
            Keys =
            {
                new ()
                {
                    Kty = "RSA",
                    E = Base64UrlEncoder.Encode(parameters.Exponent),
                    N = Base64UrlEncoder.Encode(parameters.Modulus)   ,
                    Kid = JwksOptions.KeyId,
                    Alg = "RS256",
                    Use = "sig"
                }
            }
        };
    }


    public RSAParameters GetPrivateRsaParameters()
    {
        using (var rsa = new RSACryptoServiceProvider())
        {
            rsa.ImportFromPem(PrivateKey);
            return rsa.ExportParameters(includePrivateParameters: true);
        }
    }
}