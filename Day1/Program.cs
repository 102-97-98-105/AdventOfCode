using System.Collections.Concurrent;

namespace Day1
{
    internal class Program
    {
        static ConcurrentQueue<int> numbers = new ConcurrentQueue<int>();
        static ManualResetEventSlim addedNumber = new ManualResetEventSlim();
        static CancellationTokenSource tokenSource = new CancellationTokenSource();

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
            int result = 0;
            char[] chars = line.ToCharArray();
            for (int i = 0; i < chars.Length; i++)
            {
                if (chars[i] > 47 && chars[i] < 58)
                {
                    result = (chars[i] - 48) * 10;
                    break;
                }
            }

            for (int i = chars.Length - 1; i >= 0; i--)
            {
                if (chars[i] > 47 && chars[i] < 58)
                {
                    result += chars[i] - 48;
                    break;
                }
            }
            numbers.Enqueue(result);
            addedNumber.Set();
        }
    }
}