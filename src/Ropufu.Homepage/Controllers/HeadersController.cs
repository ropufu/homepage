using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System.Text;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
namespace Ropufu.Homepage.Controllers;

[Route("api/[controller]")]
[ApiController]
public class HeadersController : ControllerBase
{
    // GET api/headers
    [Produces("text/plain")]
    public IActionResult Get()
    {
        HttpContext context = this.HttpContext;
        StringBuilder builder = new();

        // Display IP address.
        string? ip = context.Connection.RemoteIpAddress?.ToString();
        if (ip is not null)
            builder.AppendLine(ip);

        // Display headers.
        foreach (KeyValuePair<string, StringValues> header in context.Request.Headers)
            builder.Append(header.Key).Append(':').Append(' ').AppendLine(header.Value);

        return this.Ok(builder.ToString());
    }
}
