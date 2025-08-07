using BugAspnetcore62416;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddLogging(configure => configure.AddConsole());

builder.Services.AddHttpLogging(logging =>
{
    logging.LoggingFields = HttpLoggingFields.RequestProperties
                            | HttpLoggingFields.RequestHeaders
                            | HttpLoggingFields.RequestQuery
                            | HttpLoggingFields.RequestBody
                            | HttpLoggingFields.ResponseBody;
    logging.RequestBodyLogLimit = 4096;
    logging.ResponseBodyLogLimit = 4096;
    logging.MediaTypeOptions.AddText("application/json");
});

var jwksOptions = new JwksOptions
{
    PrivateKey = "-----BEGIN RSA PRIVATE KEY-----\r\nMIICWwIBAAKBgQCQXQPk5IVL9tCiMVA8+m6Pdpau8SZo1ICiVHk31Z5RkV5J3uW4\r\nLYAKhkNHczBDBwAB5BcuqhouKr86weBr9IwndsuHs5qKmUvz8y6HCiZvMwCvg7UX\r\n6G/9lZcX4uXLdcmKSOrrjPEFtzCPJ3JFLm7pi1dURzDACLSPDxp5lva7PQIDAQAB\r\nAoGAaYRzOjSF8ZY/vK1KtqdddGL7lHcS2gCo3P3ddCAhjgEw59GSGuK2+fpU5r6d\r\nAgD10mwDPcp7RE9eYvYesQmX/o2uzdcJDCeRUPJlYm+RO7i+3EIFU5LFJBBQh6zj\r\n/uK78IS4mN3Fwn+iiBcko37nl+WMBmIta9XuXrriwdZ3esECQQDWijrjxmKXemsy\r\nuaSA6Lbqky2pcY93hQ0rPUc9QxRi3euspzUincmteUniht2sU9GTkGUf4t/Hwewu\r\nLTxHtJYJAkEArEL9yu28+CM2fqVSGPMYvGfLUhAG/gX5/sdZyI6BIA1YgwMoArfg\r\nOiEWjTOv6hwVV9W/shAulRjgSbgq0poolQJAP0WFGKfpa8Mu2kblvR7k00mUreRP\r\n62/R8m6gE/E0kfPhDYpCoXLnh8G9iJE9zxTpOhvbtwux87e9b+DnZj5cAQJAK5yK\r\nRGYMAYZotYeFUWu6R3i/sPb9zjIVKLuvr2bkqgi27/RMnOOQkSbvXrBHSS3y+VnU\r\nPnB6WqUBo6hMryT4QQJAIrI8QfSG41c8OX+/wJKCglCxlIEmRBeA61jn82kPmIE1\r\n5xxIB5ty4W8/hp3VbZtMr8m5oTvGjz6f7Lu98RtACg==\r\n-----END RSA PRIVATE KEY-----",
    PublicKey = "-----BEGIN PUBLIC KEY-----\r\nMIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQCQXQPk5IVL9tCiMVA8+m6Pdpau\r\n8SZo1ICiVHk31Z5RkV5J3uW4LYAKhkNHczBDBwAB5BcuqhouKr86weBr9IwndsuH\r\ns5qKmUvz8y6HCiZvMwCvg7UX6G/9lZcX4uXLdcmKSOrrjPEFtzCPJ3JFLm7pi1dU\r\nRzDACLSPDxp5lva7PQIDAQAB\r\n-----END PUBLIC KEY-----",
};
builder.Services.AddSingleton(jwksOptions);

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddAuthentication().AddJwtBearer("myscheme", options =>
{
    options.RequireHttpsMetadata = false; //do not bother with that, it is infra related not code related
    options.SaveToken = true; //to be able to send the token to another service                                          
    options.Authority = "http://localhost:5001";
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = "http://localhost:5001",
        ValidateAudience = true,
        ValidAudience = "dummyaudience",
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,
        IssuerSigningKeys = jwksOptions.GetJwks().GetSigningKeys(),
    };
    options.MapInboundClaims = false; //by default, .NET brutally maps some claims (https://learn.microsoft.com/en-us/aspnet/core/security/authentication/claims?view=aspnetcore-6.0#claims-namespaces-default-namespaces)
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapOpenApi();

app.UseAuthorization();

app.MapControllers();

app.UseHttpLogging();

app.Run();
