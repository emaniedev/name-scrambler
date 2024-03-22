
using System.Data.SqlTypes;

internal class Program
{
    public class Name : INullable
    {
        public string text = "";
        public char[][] letters = Array.Empty<char[]>();
        public char usedLetter;
        public string[] used = Array.Empty<string>();
        public Direction direction;
        public Name?[] proposal = Array.Empty<Name?>();
        public char[] board;

        public bool IsNull => throw new NotImplementedException();
    }
    public struct Coord
    {
        public int x;
        public int y;
        public readonly int ToIndex() => x + y * maxComposition;
    }
    public static int counter = 0;
    public static int maxComposition = 0;
    public static Name[] Data = Array.Empty<Name>();
    // readonly static string[] names = { "EYRA", "AILEN", "MAKI", "ROCIO", "EDGAR" };
    readonly static string[] names = { "EYRA", "AILEN" };

    public static Name? SearchPermutations(ref char[] board, Name? n)
    {
        Name? existing = null;
        if (n?.used.Length == names.Length)
        {
            counter++;
        }
        for (int k = 0; k < names.Length; k++)
        {
            for (int j = 0; j < n?.letters.Length; j++)
            {
                for (int i = 0; i < n.letters[j].Length; i++)
                {

                    var letter = n.letters[j][i];
                    existing = n.proposal?.FirstOrDefault(p => p?.text == names[k] && p.usedLetter == letter && p.used.Contains(names[k]));
                    if (!n.used.Contains(names[k]))
                    {
                        // TODO: Revisar que la letra esté en los 2 arrays de nombres que se comparan.
                        // EYRA, AILEN
                        // ^        ^ 
                        var toSearch = names[k].ToCharArray();
                        if (toSearch.Contains(letter) && n.letters[j].Contains(letter))
                        {
                            existing = new()
                            {
                                text = names[k],
                                // letters = n.letters.Append(names[k].ToCharArray()).ToArray(),
                                usedLetter = letter
                            };
                            var currentNameWOLetter = DeleteWithArrayCopy(names[k].ToCharArray(), letter);
                            var currentLettersWOLetter = DeleteWithArrayCopy(n.letters[j], n.letters[j][i]);
                            var temp = DeleteWithArrayCopy(n.letters, n.letters[j]);

                            char[] tempch = new char[board.Length];
                            Array.Copy(board, tempch, board.Length);
                            //TODO: Posicionar el nombre en el tablero


                            existing.letters = temp.Append(currentNameWOLetter).Append(currentLettersWOLetter).ToArray();
                            existing.used = n.used.Append(existing.text).ToArray();
                            existing.board = board;
                        }
                    }
                    if (existing is not null)
                    {
                        existing = SearchPermutations(ref board, existing);
                        n.proposal = n.proposal?.Append(existing).ToArray();
                    }
                }
            }
        }
        return n;
    }
    public static void PrintData(Name?[] ns, int padding = 0)
    {
        string pad = "";
        for (int i = 0; i < padding; i++)
        {
            pad += "\t";
        }
        foreach (var n in ns)
        {
            Console.WriteLine($"{pad}{n?.text}");
            Console.WriteLine($"{pad} Propuestas: {n?.proposal.Length}");
            Console.WriteLine($"{pad} usedLetter: {n?.usedLetter}");
            Console.Write($"{pad} letters: ");
            foreach (var p in n?.letters)
            {
                if (p is not null)
                {
                    Console.Write($"{new string(p)},");
                }
            }
            Console.Write("\n");

            Console.WriteLine($"{pad} used: {new string(string.Join(",", n.used))}");
            PrintBoard(n.board, pad);

            PrintData(n.proposal, ++padding);
            --padding;
        }
    }
    public static T[] DeleteWithArrayCopy<T>(T[] inputArray, T elementToRemove)
    {
        var indexToRemove = Array.IndexOf(inputArray, elementToRemove);
        if (indexToRemove < 0)
        {
            return inputArray;
        }
        var tempArray = new T[inputArray.Length - 1];
        Array.Copy(inputArray, 0, tempArray, 0, indexToRemove);
        Array.Copy(inputArray, indexToRemove + 1, tempArray, indexToRemove, inputArray.Length - indexToRemove - 1);
        return tempArray;
    }
    public static T[] DeleteWithArraySegment<T>(T[] inputArray, T elementToRemove)
    {
        var indexToRemove = Array.IndexOf(inputArray, elementToRemove);
        if (indexToRemove < 0)
        {
            return inputArray;
        }
        T[] tempArray;
        var segment1 = new ArraySegment<T>(inputArray, 0, indexToRemove);
        var segment2 = new ArraySegment<T>(inputArray, indexToRemove + 1, inputArray.Length - indexToRemove - 1);
        tempArray = segment1.Concat(segment2).ToArray();

        return tempArray;
    }
    public static void ProcessNames(ref char[] board)
    {
        for (int i = 0; i < names.Length; i++)
        {
            var name = new Name
            {
                text = names[i],
            };
            name.letters = name.letters.Append(names[i].ToCharArray()).ToArray();
            name.used = name.used.Append(name.text).ToArray();
            char[] temp = new char[board.Length];
            Array.Copy(board, temp, board.Length);

            PlaceName(ref temp, name.text, Direction.LeftToRight);

            var foundName = FindNameIndexAndDirection(temp, name.text);

            Console.WriteLine($"foundName.index: {foundName.Item1}");
            Console.WriteLine($"foundName.direction: {foundName.Item2}");

            name.board = temp;

            SearchPermutations(ref name.board, name);

            Data = Data.Append(name).ToArray();

            Console.WriteLine($"===============================");
        }
    }
    public static void PlaceName(ref char[] board, string n, Direction direction, Coord? coord = null)
    {
        switch (direction)
        {
            case Direction.LeftToRight:
                coord ??= new Coord
                {
                    x = maxComposition / 2 - n.Length / 2,
                    y = maxComposition / 2 - 1
                };
                PlaceLeftToRight(ref board, n, coord.x, coord.y);
                break;
            case Direction.TopDown:
                coord ??= new Coord
                {
                    x = maxComposition / 2,
                    y = maxComposition / 2 - n.Length / 2
                };
                PlaceTopDown(ref board, n, coord.GetValueOrDefault().x, coord.GetValueOrDefault().y);
                break;
        }
    }
    public static void PlaceLeftToRight(ref char[] board, string n, int pox, int poy)
    {
        for (int y = 0; y < board.Length / maxComposition; y++)
        {
            if (y == poy)
            {
                for (int x = 0; x < maxComposition; x++)
                {
                    if (x == pox)
                    {
                        int i = x + y * maxComposition;
                        for (int j = 0; j < n.Length; j++)
                        {
                            board[i + j] = n[j];
                        }
                    }
                }
            }
        }

    }
    public static void PlaceTopDown(ref char[] board, string n, int pox, int poy)
    {
        for (int y = 0; y < board.Length / maxComposition; y++)
        {
            if (y == poy)
            {
                for (int x = 0; x < maxComposition; x++)
                {
                    if (x == pox)
                    {
                        int i = x + y * maxComposition;
                        for (int j = 0; j < n.Length; j++)
                        {
                            board[i + j * maxComposition] = n[j];
                        }
                    }
                }
            }
        }

    }
    public enum Direction
    {
        LeftToRight,
        TopDown
    }

    public static (int, Direction) FindNameIndexAndDirection(char[] board, string n)
    {
        var dir = Direction.LeftToRight;
        int index = new string(board).IndexOf(n);

        if (index < 0)
        {
            //try vertical
            int vIndex = new string(board).IndexOf(n[0]);
            if (vIndex > 0)
            {
                dir = Direction.TopDown;
                for (int j = 1; j < n.Length; j++)
                {
                    var v = new string(board).IndexOf(n[j]);
                    if (v < 0) break;
                    if (v % maxComposition != vIndex % maxComposition) break;
                }
                index = vIndex;
            }
        }
        return (index, dir);
    }
    public static void PrintBoard(char[] board, string pad)
    {
        for (int y = 0; y < board.Length / maxComposition; y++)
        {
            Console.Write($"{pad}");
            for (int x = 0; x < maxComposition; x++)
            {
                int i = x + y * maxComposition;
                Console.Write($"{board[i]}");
            }
            Console.Write("\n");
        }
    }
    public static void Main(string[] args)
    {
        ArgumentNullException.ThrowIfNull(args);

        for (int i = 0; i < names.Length; i++)
        {
            maxComposition += names[i].Length;
        }


        var board = new char[maxComposition * maxComposition];
        for (int i = 0; i < board.Length; i++)
        {
            board[i] = '*';
        }
        ProcessNames(ref board);
        PrintData(Data);


    }
}