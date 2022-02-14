using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Security;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Ropufu.Homepage.TagHelpers;

[HtmlTargetElement("wiki")]
public class WikiTagHelper : TagHelper
{
    private const string MissingLink = "??";
    private const string JsonPath = "/wiki.json";

    private struct WikiLink
    {
        [JsonPropertyName("link")]
        public string Link { get; set; }

        [JsonPropertyName("fallback name")]
        public string FallbackName { get; set; }

        [JsonIgnore]
        public string Name { get; set; }
    }

    private static Dictionary<string, WikiLink>? s_dictionary;

    public WikiTagHelper(IWebHostEnvironment env)
    {
        if (WikiTagHelper.s_dictionary is null)
        {
            WikiTagHelper.s_dictionary = new Dictionary<string, WikiLink>();
            string fullPath = Path.Join(env.ContentRootPath, WikiTagHelper.JsonPath);
            try
            {
                string text = File.ReadAllText(fullPath);
                using JsonDocument json = JsonDocument.Parse(text);

                foreach (JsonProperty item in json.RootElement.EnumerateObject())
                {
                    WikiLink wikiLink = JsonSerializer.Deserialize<WikiLink>(item.Value.GetRawText());
                    WikiTagHelper.s_dictionary.Add(item.Name, wikiLink);
                } // foreach (...)
            } // try
            catch (IOException) { }
            catch (SecurityException) { }
            catch (UnauthorizedAccessException) { }
            catch (NotSupportedException) { }
            catch (JsonException) { }
            catch (ArgumentException) { }
        } // if (...)
    }

    public string Key { get; set; } = "??";

    public string Class { get; set; } = "wiki-link";

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        if (WikiTagHelper.s_dictionary is null)
            return;

        WikiTagHelper.s_dictionary.TryGetValue(this.Key, out WikiLink wikiLink);

        if (string.IsNullOrEmpty(wikiLink.Link))
        {
            output.TagName = "strong";
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Content.SetContent(WikiTagHelper.MissingLink);
            return;
        } // if (...)

        var content = await output.GetChildContentAsync().ConfigureAwait(true);
        string name = content.GetContent();
        if (string.IsNullOrEmpty(name))
            name = wikiLink.FallbackName;

        output.TagName = "a";
        output.TagMode = TagMode.StartTagAndEndTag;
        output.Attributes.SetAttribute("href", wikiLink.Link);
        output.Attributes.SetAttribute("class", this.Class);
        output.Content.SetContent(name);
    }
}
