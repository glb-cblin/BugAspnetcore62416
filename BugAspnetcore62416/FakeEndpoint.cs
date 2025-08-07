using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BugAspnetcore62416;

[ApiController]    
public class FakeEndpoint() : ControllerBase
{
    [HttpGet("/fake")]
    [Authorize]
    public Fake Get()
    {
        var id = this.User.FindFirst("id")?.Value ?? throw new InvalidOperationException("User ID not found in claims.");
        return new Fake { Id = id };
    }
}

public class Fake
{
    public string Id { get; set; }
}