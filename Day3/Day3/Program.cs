using System.Diagnostics.CodeAnalysis;

namespace Day3
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string[] lines = File.ReadAllLines("input.txt");
            List<Part> parts = new List<Part>();
            List<Symbol> symbols = new List<Symbol>();

            int partStart = -1;

            for (int line = 0; line < lines.Length; line++)
            {
                char[] chars = lines[line].ToCharArray();
                for (int col = 0; col < chars.Length; col++)
                {
                    if (!char.IsNumber(chars[col]) && chars[col] != '.')
                    {
                        symbols.Add(new Symbol(col, line, chars[col]));
                    }


                    if (char.IsNumber(chars[col]) && partStart == -1)
                    {
                        partStart = col;
                    }

                    if (partStart != -1 && !char.IsNumber(chars[col]))
                    {
                        parts.Add(new Part(partStart, line, col - partStart, int.Parse(new Span<char>(chars, partStart, col - partStart))));
                        partStart = -1;
                    }
                }
                if (partStart != -1)
                {
                    parts.Add(new Part(partStart, line, chars.Length - partStart, int.Parse(new Span<char>(chars, partStart, chars.Length - partStart))));
                    partStart = -1;
                }

            }

            int sum = parts.Where(x => x.GetSurroundingPoints().Any(x => symbols.Any(y => y?.X == x.X && y?.Y == x.Y)))
                .Aggregate(0, (seed, part) => seed + part.Value);
            Console.WriteLine(sum);

            int sum2 = symbols.Where(x => x.Char == '*')
                .Select(symbol =>
                    parts
                        .Where(part =>
                            part.GetSurroundingPoints()
                            .Any(point => point.X == symbol.X && point.Y == symbol.Y)
                            )
                        )
                .Where(x => x.Count() > 1)
                .Select(x => x.Aggregate(1, (seed, value) => value.Value * seed))
                .Aggregate(0, (seed, x) => seed + x);


            Console.WriteLine(sum2);

        }
    }

    public record Point(int X, int Y);

    public record Symbol(int X, int Y, char Char) : Point(X, Y);

    public record Part(int X, int Y, int Width, int Value)
    {
        public IEnumerable<Point> GetSurroundingPoints()
        {
            List<Point> points = new List<Point>
            {
                // Left border
                new(X - 1, Y - 1),
                new(X - 1, Y),
                new(X - 1, Y + 1),
                // Right border
                new(X + Width, Y - 1),
                new(X + Width, Y),
                new(X + Width, Y + 1),
            };

            for (int i = 0; i < Width; i++)
            {
                points.Add(new(X + i, Y - 1));
                points.Add(new(X + i, Y + 1));
            }
            return points;
        }
    }
}