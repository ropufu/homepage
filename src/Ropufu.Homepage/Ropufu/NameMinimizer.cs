namespace Ropufu.Homepage;

public class NameMinimizer
{
    private readonly char _first;
    private readonly char _last;
    private readonly List<char> _next = new();

    public NameMinimizer(char first, char last)
    {
        if (first <= last)
        {
            this._first = first;
            this._last = last;
        } // if (...)
        else
        {
            this._first = last;
            this._last = first;
        } // else
        this._next.Add(this._first);
    }

    public string Next()
    {
        string result = new(this._next.ToArray());

        var position = 0;
        ++this._next[position];
        while (this._next[position] == this._last + 1)
        {
            this._next[position] = this._first;
            ++position;
            if (position == this._next.Count)
                this._next.Add(this._first);
            else ++this._next[position];
        } // while (...)

        return result;
    }
}
