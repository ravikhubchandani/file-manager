using System;
using System.IO;

namespace FileManager
{
    public class BufferedFileReader : IBufferedFileReader, IDisposable
    {
        public string CurrentLine { get; set; }
        public int CurrentLineNumber { get; set; }
        
        private readonly StreamReader _reader;

        public BufferedFileReader(string path)
        {
            _reader = new StreamReader(path);
        }

        public string ReadLine(out bool success)
        {
            CurrentLine = _reader.ReadLine();
            success = CurrentLine != null;
            if (success)
                CurrentLineNumber++;
            return CurrentLine;
        }

        public void Dispose()
        {
            _reader.Close();
        }
        
    }
}
