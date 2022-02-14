using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;

namespace Ropufu.Homepage.Pages;

// https://docs.microsoft.com/en-us/aspnet/core/fundamentals/error-handling
[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
[IgnoreAntiforgeryToken]
public class ErrorModel : PageModel
{
    private readonly IWebHostEnvironment _environment;

    public ErrorModel(IWebHostEnvironment environment)
    {
        this._environment = environment;
    }

    public string? RequestId { get; private set; }

    public string? ExceptionMessage { get; private set; }

    private void DisplayDetails()
    {
        if (!this._environment.IsDevelopment())
            return;

        this.RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier;

        IExceptionHandlerPathFeature? feature =
            this.HttpContext.Features.Get<IExceptionHandlerPathFeature>();

        this.ExceptionMessage = feature?.Error.Message;
    }

    public void OnGet() => this.DisplayDetails();

    public void OnPost() => this.DisplayDetails();
}
