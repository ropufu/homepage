using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ropufu.Homepage.Pages;

public class SorryModel : PageModel
{
    private static readonly string[] s_quotes =
    {
        "The world has changed.",
        "Much that once was is lost.",
        "None now live who remember it.",
        "The one charm about the past is that it is the past.",
        "To define is to limit.",
        "Some things are more precious because they don't last long.",
        "One should always be a little improbable."
    };

    private static readonly Random s_random = new();

    public static string GetQuote()
    {
        int which = SorryModel.s_random.Next(SorryModel.s_quotes.Length);
        return SorryModel.s_quotes[which];
    }
}
