using System.Text.RegularExpressions;

namespace Day2
{
    internal class Program
    {
        static readonly Regex regex = new Regex(@"^(?'game'Game (?'id'\d+):)(?'draws'.*)$", RegexOptions.Compiled | RegexOptions.Singleline);

        static void Main(string[] args)
        {
            IEnumerable<string> lines = File.ReadLines("Input.txt");
            int result = lines.Select(ParseLine)
                .OfType<Game>()
                .Where(x => !x.Draws.Any(draw => draw.Red > 12 || draw.Blue > 14 || draw.Green > 13))
                .Where(x => !x.Draws.Any(draw => draw.Red == 12 && draw.Blue == 14 && draw.Green == 13))
                .Aggregate(0, (seed, value) => seed += value.Id);
            Console.WriteLine($"possible games: {result}");

            int result2 = lines.Select(ParseLine)
                .OfType<Game>()
                .Select(game =>
                    game.Draws.Aggregate(new { Red = 0, Green = 0, Blue = 0 },
                        (seed, draw) =>
                        {
                            if (draw.Red > seed.Red) seed = seed with { Red = draw.Red };
                            if (draw.Green > seed.Green) seed = seed with { Green = draw.Green };
                            if (draw.Blue > seed.Blue) seed = seed with { Blue = draw.Blue };
                            return seed;
                        })
                    )
                .Aggregate(0, (seed, value) => value.Red * value.Green * value.Blue + seed);
            Console.WriteLine(result2);
        }

        static Game? ParseLine(string line)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                return null;
            }
            return new Game(
                int.Parse(
                    regex.Match(line).Groups["id"].Value
                    ),
                regex.Match(line).Groups["draws"].Value
                    .Split(';')
                    .Select(Draw.Build)
            );
        }
    }

    public record Game(int Id, IEnumerable<Draw> Draws);

    public record Draw(int Red, int Green, int Blue)
    {
        public static Draw Build(string str)
        {
            var parsed = str.Split(',')
                .Select(x => x.Trim())
                .Select(x => x.Split(' '))
                .Select(x => new { Color = x[1], Amount = int.Parse(x[0]) })
                .Aggregate(new { Blue = 0, Red = 0, Green = 0 },
                    (seed, val) =>
                    {
                        switch (val.Color)
                        {
                            case "red":
                                return seed with { Red = val.Amount };
                            case "green":
                                return seed with { Green = val.Amount };
                            case "blue":
                                return seed with { Blue = val.Amount };
                            default:
                                return seed;
                        }
                    });
            return new Draw(parsed.Red, parsed.Green, parsed.Blue);
        }
    }
}