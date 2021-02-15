using FileManager;
using System;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var mgr = new LocalFileManager();
            string x = mgr.GetProposedPathForFile("test.txt");
            Console.WriteLine(x);
        }
    }
}
