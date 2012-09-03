using System;
using DotNetDetector;

namespace DotNetDetectorApp
{
    class Program
    {
        static void Main(string[] args)
        {
            foreach (var version in Detector.Versions)
            {
                Console.WriteLine(version.ToString());
            }
            Console.ReadKey();
        }
    }
}
