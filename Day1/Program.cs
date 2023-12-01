using System.Collections.Concurrent;
using System.Net.Sockets;

namespace Day1
{
    internal class Program
    {
        static ConcurrentQueue<int> numbers = new ConcurrentQueue<int>();
        static ManualResetEventSlim addedNumber = new ManualResetEventSlim();
        static CancellationTokenSource tokenSource = new CancellationTokenSource();
        static string[] numberStrs = new[] {
            "one",
            "two",
            "three",
            "four",
            "five",
            "six",
            "seven",
            "eight",
            "nine",
        };

        static void Main(string[] args)
        {
            int count = 0;
            int result = 0;
            int summedUp = 0;
            CancellationToken token = tokenSource.Token;

            IEnumerable<string> lines = File.ReadLines("Input.txt");

            Thread thread = new Thread(() =>
            {
                while (true)
                {
                    addedNumber.Wait();
                    while (numbers.TryDequeue(out int num))
                    {
                        result += num;
                        summedUp++;
                    }
                    if (token.IsCancellationRequested)
                    {
                        if (summedUp >= count)
                        {
                            break;
                        }
                    }
                }
            });
            thread.Start();

            foreach (string line in lines)
            {
                count++;
                ThreadPool.QueueUserWorkItem(GetNumberOfLine, line);
            }
            tokenSource.Cancel();
            thread.Join();
            Console.WriteLine(result);
        }

        public static void GetNumberOfLine(object? param)
        {
            string line = (string)(param ?? throw new ArgumentNullException(nameof(param)));
            int firstDigitIndex = line.Length;
            int lastDigitIndex = 0;
            int firstDigit = 0;
            int lastDigit = 0;

            for (int i = 0; i < numberStrs.Length; i++)
            {
                int index = line.IndexOf(numberStrs[i]);
                if (index != -1 && index < firstDigitIndex)
                {
                    firstDigitIndex = index;
                    firstDigit = i + 1;
                }
            }

            for (int i = 0; i < numberStrs.Length; i++)
            {
                int index = line.LastIndexOf(numberStrs[i]);
                if (index != -1 && index > lastDigitIndex)
                {
                    lastDigitIndex = index;
                    lastDigit = i + 1;
                }
            }


            char[] chars = line.ToCharArray();
            for (int i = 0; i < firstDigitIndex; i++)
            {
                if (chars[i] > 47 && chars[i] < 58)
                {
                    firstDigit = chars[i] - 48;
                    break;
                }
            }

            for (int i = chars.Length - 1; i >= lastDigitIndex; i--)
            {
                if (chars[i] > 47 && chars[i] < 58)
                {
                    lastDigit = chars[i] - 48;
                    break;
                }
            }

            numbers.Enqueue(firstDigit * 10 + lastDigit);
            addedNumber.Set();
        }
    }
}