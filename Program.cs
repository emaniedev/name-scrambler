
using System.Data.SqlTypes;

internal class Program
{
    public class Name : INullable
    {
        public string text;
        public char[] letters = Array.Empty<char>();
        public string[] used = Array.Empty<string>();
        public Name?[] proposal = Array.Empty<Name?>();

        public bool IsNull => throw new NotImplementedException();
    }

    public static Name[] Data = Array.Empty<Name>();
    readonly static string[] names = { "EYRA", "AILEN", "ROCIO", "EDGAR", "MAKI" };
    public static Name? SearchPermutations(Name? n)
    {
        Name? existing = null;
        for (int k = 0; k < names.Length; k++)
        {
            var uniqueLetters = n.letters.Distinct().ToArray();
            Console.WriteLine(new string(uniqueLetters));
            existing = n.proposal?.FirstOrDefault(p => p.text == names[k]);
            for (int j = 0; j < uniqueLetters.Length; j++)
            {
                var letter = uniqueLetters[j];
                if (!n.used.Contains(names[k]))
                {
                    var toSearch = names[k].ToCharArray();
                    if (toSearch.Contains(letter))
                    {
                        existing ??= new()
                        {
                            text = names[k],
                            letters = names[k].ToCharArray().Concat(n.letters).ToArray(),
                        };
                        existing.used = n.used.Append(existing.text).ToArray();
                    }
                }
            }
            if (existing is not null)
            {
                existing = SearchPermutations(existing);
                n.proposal = n.proposal?.Append(existing).ToArray();
            }
        }
        return n;
    }
    public static void PrintData(Name[] ns, int padding = 0)
    {
        string pad = "";
        for (int i = 0; i < padding; i++)
        {
            pad += "\t";
        }
        foreach (var n in ns)
        {
            Console.WriteLine($"{pad}{n.text}");
            Console.WriteLine($"{pad} Propuestas: {n.proposal.Length}");
            Console.WriteLine($"{pad} letters: {new string(n.letters)}");
            Console.WriteLine($"{pad} used: {new string(string.Join(",", n.used))}");

            PrintData(n.proposal, ++padding);
            --padding;

            // foreach (var p in n.proposal)
            // {
            //     Console.WriteLine($"{pad}  - {p.text} with {new string(p.letters)}");
            // }

        }
    }
    public static void Main(string[] args)
    {
        ArgumentNullException.ThrowIfNull(args);


        for (int i = 0; i < names.Length; i++)
        {
            var name = new Name
            {
                text = names[i],
                letters = names[i].ToCharArray(),
            };
            name.used = name.used.Append(name.text).ToArray();

            SearchPermutations(name);

            Data = Data.Append(name).ToArray();

            Console.WriteLine($"===============================");
        }

        PrintData(Data);
    }
}