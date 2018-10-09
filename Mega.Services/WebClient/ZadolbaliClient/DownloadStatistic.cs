namespace Mega.WebClient.ZadolbaliClient
{
    using System.Diagnostics;
    using System.Threading;

    public static class DownloadStatistic
    {
        private static readonly Stopwatch Watch = new Stopwatch();

        private static int count;

        public static int Count => count;

        public static int Inсrement() => Interlocked.Increment(ref count);

        public static double Speed()
        {
            return Count / Watch.Elapsed.TotalSeconds;
        }

        public static void Start()
        {
            Watch.Start();
        }
    }
}
