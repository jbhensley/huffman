using BenchmarkDotNet.Running;

namespace HuffmanTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var bench = new HuffmanBench();
            var summary = BenchmarkRunner.Run<HuffmanBench>();

            //var tests = new HuffmanTests();
            //tests.SetupPerfTest();
            //tests.ProcessHeadersWithLoop();
            //tests.ProcessHeadersWithLoopAndComputedCodeMax();
            //tests.ProcessHeadersWithDictionary();

            //Console.WriteLine("Finished");
            //Console.ReadLine();
        }
    }
}
