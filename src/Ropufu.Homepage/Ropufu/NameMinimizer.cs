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
            _first = first;
            _last = last;
        } // if (...)
        else
        {
            _first = last;
            _last = first;
        } // else
        _next.Add(_first);
    }

    public string Next()
    {
        string result = new(_next.ToArray());

        var position = 0;
        ++_next[position];
        while (_next[position] == _last + 1)
        {
            _next[position] = _first;
            ++position;
            if (position == _next.Count)
                _next.Add(_first);
            else ++_next[position];
        } // while (...)

        return result;
    }
}
