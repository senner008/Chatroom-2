using System;
using System.Diagnostics;
using System.Numerics;

namespace app {
    public static class Helper 
    {
        public static double ToMiliseconds (DateTime date) 
        {
            var mili =  date.ToUniversalTime ().Subtract (new DateTime (1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
            return Math.Round(mili,2);
        }

        public static string GuidToBigInt (Guid guid) 
        {
           return (new BigInteger(guid.ToByteArray())).ToString();   
        }
    }

    public class Benchmark : IDisposable 
    {
        private readonly Stopwatch timer = new Stopwatch();
        private readonly string benchmarkName;

        public Benchmark(string benchmarkName)
        {
            this.benchmarkName = benchmarkName;
            timer.Start();
        }

        public void Dispose() 
        {
            timer.Stop();
            Console.WriteLine($"{benchmarkName} {timer.ElapsedMilliseconds}");
        }
    }

    public class MyChatHubException : Exception
    {
        public MyChatHubException()
        {
        }

        public MyChatHubException(string message)
            : base(message)
        {
        }

        public MyChatHubException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}