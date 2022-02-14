using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ropufu.Homepage.Pages.Xavier.Courses;

public class GraphModel : PageModel
{
    public string CatalogueId { get; private set; } = string.Empty;

    public IActionResult OnGet(string? catalogueId)
    {
        if (catalogueId is not null)
        {
            const string pattern = "AAAA-XXX";
            IActionResult sorry = this.RedirectToPage("/Sorry");

            if (catalogueId.Length != pattern.Length)
                return sorry;

            if (!char.IsLetter(catalogueId, 0) ||
                !char.IsLetter(catalogueId, 1) ||
                !char.IsLetter(catalogueId, 2) ||
                !char.IsLetter(catalogueId, 3))
                return sorry;

            if (catalogueId[4] != pattern[4])
                return sorry;

            if (!char.IsDigit(catalogueId, 5) ||
                !char.IsDigit(catalogueId, 6) ||
                !char.IsDigit(catalogueId, 7))
                return sorry;

            this.CatalogueId = catalogueId;
        } // if (...)
        return this.Page();
    }
}
