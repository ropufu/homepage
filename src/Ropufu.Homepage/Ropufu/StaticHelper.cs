using System.Text;

namespace Ropufu.Homepage;

public static class StaticHelper
{
    public static string AddOrdinal(float num) => StaticHelper.AddOrdinal((int)Math.Ceiling(num));

    public static string AddOrdinal(double num) => StaticHelper.AddOrdinal((int)Math.Ceiling(num));

    public static string AddOrdinal(int num)
    {
        if (num < 0)
            return "-" + StaticHelper.AddOrdinal(-num);

        return (num % 100) switch
        {
            11 or 12 or 13 => num + "th",
            _ => (num % 10) switch
            {
                1 => num + "st",
                2 => num + "nd",
                3 => num + "rd",
                _ => num + "th",
            },
        };
    }

    private static readonly string[] s_short_words = {
        "a", "an", "and", "as", "at",
        "but", "by",
        "for",
        "in",
        "nor",
        "of", "on", "or", "out",
        "so",
        "the", "to",
        "via",
        "yet"
    };

    public static bool IsTooShort(this string value) =>
        StaticHelper.s_short_words.Contains(value);

    public static void ParseWordsAndNumbers(this string value, out List<string> words, out List<int> numbers)
    {
        words = new List<string>();
        numbers = new List<int>();

        // 0 for no expectations; 1 for digit; 2 for letter.
        byte expecting = 0;
        StringBuilder builder = new(value.Length);

        foreach (char c in value)
        {
            bool isDigit = char.IsDigit(c);
            bool isLetter = char.IsLetter(c);
            if (isDigit)
            {
                // Unexpected digit.
                if (expecting == 2)
                {
                    // Dump the current word.
                    words.Add(builder.ToString());
                    builder.Clear();
                } // if (...)
                builder.Append(c);
                expecting = 1;
            } // if (...)
            else if (isLetter)
            {
                // Unexpected letter.
                if (expecting == 1)
                {
                    string word = builder.ToString();
                    // Dump the current number.
                    if (int.TryParse(word, out int x))
                        numbers.Add(x);
                    else
                        words.Add(word);
                    builder.Clear();
                } // if (...)
                builder.Append(c);
                expecting = 2;
            } // else if (...)
            else
            {
                // Unexpected separator.
                switch (expecting)
                {
                    case 1:
                        string word = builder.ToString();
                        // Dump the current number.
                        if (int.TryParse(word, out int x))
                            numbers.Add(x);
                        else
                            words.Add(word);
                        builder.Clear();
                        break;
                    case 2:
                        // Dump the current word.
                        words.Add(builder.ToString());
                        builder.Clear();
                        break;
                } // switch (...)
                expecting = 0;
            } // else
        } // foreach (...)

        // Finish up.
        switch (expecting)
        {
            case 1:
                string word = builder.ToString();
                // Dump the current number.
                if (int.TryParse(word, out int x))
                    numbers.Add(x);
                else
                    words.Add(word);
                builder.Clear();
                break;
            case 2:
                // Dump the current word.
                words.Add(builder.ToString());
                builder.Clear();
                break;
        } // switch (...)
    }

    public static string Reduce(this string value, Func<char, bool>? predicate = null)
    {
        if (predicate is null)
            predicate = c => char.IsLetter(c) || char.IsDigit(c);
        
        StringBuilder builder = new(value.Length);

        foreach (char c in value)
            if (predicate(c))
                builder.Append(c);

        return builder.ToString();
    }

    public static int[] NextPermutation(this Random rnd, int size, int offset = 0)
    {
        if (size < 1)
            throw new ArgumentOutOfRangeException(nameof(size));

        var shuffled = new int[size];
        for (var i = 0; i < size; ++i)
            shuffled[i] = i + offset;

        // First index of non-shuffled element.
        var position = 0;
        for (var i = 0; i < size; ++i)
        {
            var index = rnd.Next(position, size);
            // Swap elements at <position> and <index>.
            var old = shuffled[position];
            shuffled[position] = shuffled[index];
            shuffled[index] = old;
            ++position;
        } // for (...)
        return shuffled;
    }

    public static string ToCommaSeparatedValues<T>(T[] column)
    {
        StringBuilder builder = new();

        foreach (T item in column)
            builder.Append(item).AppendLine();

        return builder.ToString();
    }

    public static string ToCommaSeparatedValues<T>(T[,] matrix, string[] header, string separator = ",")
    {
        int m = matrix.GetLength(0);
        int n = matrix.GetLength(1);

        if (header.Length != n)
            throw new ArgumentOutOfRangeException(nameof(header));
        if (m == 0 || n == 0)
            return string.Empty;

        StringBuilder builder = new();
        builder.AppendJoin(separator, header).AppendLine();
        for (var i = 0; i < m; ++i)
        {
            builder.Append(matrix[i, 0]);
            for (var j = 1; j < n; ++j)
                builder.Append(separator).Append(matrix[i, j]);
            builder.AppendLine();
        } // for (...)

        return builder.ToString();
    }
}
