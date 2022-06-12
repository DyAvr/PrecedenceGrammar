using System.Text;

internal class Program
{
    static Dictionary<char, List<String>> grammar;
    static Dictionary<char, int> dictionary;
    static int?[,] matrix;

    static void Main()
    {
        Console.WriteLine(
            IsDerivable(Console.ReadLine() ?? String.Empty) ? "Success" : "Failure");
    }

    private static bool IsDerivable(string line)
    {
        Initialize();
        line += "#";
        int cursor = 0;

        var stack = new Stack<char>();
        stack.Push('#');
        bool failure = false;

        if (line.Length == 1)
            return false;
        StringBuilder sb;
        while (cursor <= line.Length) {
            char c = line[cursor];
            if (!dictionary.ContainsKey(c)) {
                failure = true;
                break;
            }
            if (c == '#' && stack.Count == 2 && stack.Peek() == 'A')
                break;
            int? relation = GetRelation(stack.Peek(), line[cursor]);
            if (relation == null) {
                failure = true;
                break;
            }
            if (relation <= 0) {
                stack.Push(c);
                cursor++;
            } else {
                sb = new StringBuilder();
                char last;
                do {
                    last = stack.Pop();
                    sb.Insert(0, last);
                } while (GetRelation(stack.Peek(), last) >= 0);
                using var enumerator = grammar.GetEnumerator();
                bool found = false;
                char leftPart = default;
                while (enumerator.MoveNext() && !found) {
                    var entry = enumerator.Current;
                    if (entry.Value.Contains(sb.ToString())) {
                        found = true;
                        leftPart = entry.Key;
                    }
                }
                if (!found) {
                    failure = true;
                    break;
                }
                stack.Push(leftPart);
            }
        }
        sb = new StringBuilder();
        try {
            sb.Insert(0, stack.Pop());
            sb.Insert(0, stack.Pop());
        } catch (Exception) {
            return false;
        }
        return !failure && sb.ToString().Equals("#A");
    }

    private static void Initialize() {
        grammar = new Dictionary<char, List<string>>();
        var A = new List<string>
        {
            "A+D",
            "A-D",
            "D"
        };
        grammar.Add('A', A);
        var B = new List<string>
        {
            "B*C",
            "B/C",
            "C"
        };
        grammar.Add('B', B);
        var C = new List<string>
        {
            "a",
            "(E)"
        };
        grammar.Add('C', C);
        var D = new List<string>
        {
            "B"
        };
        grammar.Add('D', D);
        var E = new List<string>
        {
            "A"
        };
        grammar.Add('E', E);

        dictionary = new Dictionary<char, int>
        {
            { 'A', 0 },
            { 'D', 1 },
            { 'B', 2 },
            { 'C', 3 },
            { 'E', 4 },
            { '+', 5 },
            { '-', 6 },
            { '*', 7 },
            { '/', 8 },
            { 'a', 9 },
            { '(', 10 },
            { ')', 11 },
            { '#', 12 }
        };

        matrix = new int?[,]{
            {null, null, null, null, null, 0, 0, null, null, null, null, 1, 1},
            {null, null, null, null, null, 1, 1, null, null, null, null, 1, 1},
            {null, null, null, null, null, 1, 1, 0, 0, null, null, 1, 1},
            {null, null, null, null, null, 1, 1, 1, 1, null, null, 1, 1},
            {null, null, null, null, null, null, null, null, null, null, null, 0, 1},
            {null, 0, -1, -1, null, null, null, null, null, -1, -1, null, 1},
            {null, 0, -1, -1, null, null, null, null, null, -1, -1, null, 1},
            {null, null, null, 0, null, null, null, null, null, -1, -1, null, 1},
            {null, null, null, 0, null, null, null, null, null, -1, -1, null, 1},
            {null, null, null, null, null, 1, 1, 1, 1, null, null, 1, 1},
            {-1, -1, -1, -1, 0, null, null, null, null, -1, -1, null, 1},
            {null, null, null, null, null, 1, 1, 1, 1, null, null, 1, 1},
            {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, null},
        };
    }

    private static int? GetRelation(char left, char right) => matrix[dictionary[left],dictionary[right]];
}
