using BenchmarkDotNet.Running;

namespace HuffmanTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var bench = new HuffmanBench();

            var summary = BenchmarkRunner.Run<HuffmanBench>();

            //bench.Setup();
            //for (var i = 0; i < 20_000; i++)
            //{
            //    bench.Array();
            //}

            //Console.WriteLine("Finished");
            //Console.ReadLine();
        }
    }
}
