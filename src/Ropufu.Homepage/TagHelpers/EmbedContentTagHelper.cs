using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Security;

namespace Ropufu.Homepage.TagHelpers;
 
[HtmlTargetElement("embed-content")]
public class EmbedContentTagHelper : TagHelper
{
    private static string? s_webRootPath;

    public EmbedContentTagHelper(IWebHostEnvironment env)
    {
        if (EmbedContentTagHelper.s_webRootPath is null)
            EmbedContentTagHelper.s_webRootPath = env.WebRootPath;
    }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        if (EmbedContentTagHelper.s_webRootPath is null)
            return;

        string fullPath = this.Source.StartsWith("~/") ?
            Path.Join(EmbedContentTagHelper.s_webRootPath, this.Source.Substring(2)) :
            Path.Join(EmbedContentTagHelper.s_webRootPath, this.Source);

        output.TagName = null;
        try
        {
            string text = File.ReadAllText(fullPath);
            output.Content.SetHtmlContent(text);
        } // try
        catch (IOException) { }
        catch (SecurityException) { }
        catch (UnauthorizedAccessException) { }
        catch (NotSupportedException) { }
        catch (ArgumentException) { }
    }

    [HtmlAttributeName("src")]
    public string Source { get; set; } = "??";
}
