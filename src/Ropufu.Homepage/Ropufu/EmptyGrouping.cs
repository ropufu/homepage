using System.Collections;

namespace Ropufu.Homepage;
 
public class EmptyGrouping<TKey, TElement> : IGrouping<TKey, TElement>
{
    private static readonly List<TElement> s_elements = new();

    public TKey Key { get; private init; }

    public EmptyGrouping(TKey key) => this.Key = key;

    public IEnumerator<TElement> GetEnumerator() => s_elements.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
}
