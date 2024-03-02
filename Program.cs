using System.Net.WebSockets;
using System.Reflection.Metadata;

internal class Program
{
    public class Name
    {
        public string text;
        public char[] letters;
        public Name[] proposal;
    }

    public static Name[] Data = Array.Empty<Name>();
    private static void Main(string[] args)
    {
        string[] names = { "EYRA", "AILEN", "ROCIO", "EDGAR", "MAKI" };

        for (int i = 0; i < names.Length; i++)
        {
            var name = new Name
            {
                text = names[i],
                letters = names[i].ToCharArray(),
                proposal = Array.Empty<Name>()
            };
            Console.WriteLine($"Procesando {name.text}");
            Console.WriteLine($" Propuestas: {name.proposal.Length}");
            for (int j = 0; j < name.letters.Length; j++)
            {
                Console.WriteLine($" +++++++++++++++++++++++++++++++");
                var letter = name.letters[j];
                for (int k = 0; k < names.Length; k++)
                {
                    if (k != i)
                    {
                        var toSearch = names[k].ToCharArray();
                        Console.WriteLine($"  Procesando letra {letter} sobre {names[k]}");
                        if (toSearch.Contains(letter))
                        {
                            Name existing = name.proposal.FirstOrDefault(p => p.text == names[k]);
                            if (existing is null)
                            {
                                existing = new Name();
                                Console.WriteLine($" No hay propuesta, se crea {names[k]}");
                                existing.text = names[k];
                                existing.letters = Array.Empty<char>();
                                name.proposal = name.proposal.Append(existing).ToArray();
                            }
                            existing.letters = existing.letters.Append(letter).ToArray();
                        }

                    }
                }
            }

            Console.WriteLine($" Propuestas: {name.proposal.Length}");
            Console.WriteLine($"Procesado {name.text}");

            Data = Data.Append(name).ToArray();

            Console.WriteLine($"===============================");
        }


        foreach (var n in Data)
        {
            Console.WriteLine($"{n.text}");
            Console.WriteLine($" Propuestas: {n.proposal.Length}");

            foreach (var p in n.proposal)
            {
                Console.WriteLine($"  - {p.text} in {new string(p.letters)}");
            }

        }

    }
}