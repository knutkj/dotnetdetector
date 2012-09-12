using System;
using DotNetDetector;

namespace DotNetDetectorApp
{
    public class Program
    {
        static void Main(string[] args)
        {
            new DotNetVersionWriter(Detector.Current).WriteAll();
            Console.ReadKey();
        }
    }

    public class DotNetVersionWriter
    {
        private IDetector _detector;

        public DotNetVersionWriter(IDetector detector)
        {
            _detector = detector;
        }

        public virtual void WriteAll()
        {
            foreach (var version in _detector.Versions)
            {
                Write(version);
            }
        }

        public virtual void Write(DotNetVersion version)
        {
            Console.WriteLine(version.ToString());
        }
    }
}