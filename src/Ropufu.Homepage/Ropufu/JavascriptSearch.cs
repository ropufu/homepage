using System.Text;

namespace Ropufu.Homepage;

public static class JavascriptSearch
{
    public static void Initialize<TKey, TElement>(IEnumerable<IGrouping<TKey, TElement>> groupings,
        Func<IGrouping<TKey, TElement>, string> groupingIdSelector,
        Func<TElement, string> itemIdSelector,
        Func<TElement, IEnumerable<string>> keywordSelector,
        out string groupingKeys,
        out string items)
    {
        StringBuilder keysBuilder = new();
        StringBuilder itemsBuilder = new();
        var groupingIndex = 0;
        var itemIndex = 0;
        foreach (IGrouping<TKey, TElement> grouping in groupings)
        {
            if (groupingIndex != 0)
                keysBuilder.Append(',');
            keysBuilder.Append('"').Append(groupingIdSelector(grouping)).Append('"');

            foreach (TElement item in grouping)
            {
                if (itemIndex != 0)
                    itemsBuilder.Append(',');
                itemsBuilder
                    .Append('{')
                    .Append("id:").Append('"').Append(itemIdSelector(item)).Append('"').Append(',')
                    .Append("cat:").Append('"').Append(groupingIndex).Append('"').Append(',')
                    .Append("keywords:[");
                foreach (string keyword in keywordSelector(item))
                    itemsBuilder.Append('"').Append(keyword).Append('"').Append(',');
                itemsBuilder
                    .Append(']')
                    .Append('}');
                ++itemIndex;
            } // foreach (...)

            ++groupingIndex;
        } // foreach (...)

        groupingKeys = keysBuilder.ToString();
        items = itemsBuilder.ToString();
    }
}
