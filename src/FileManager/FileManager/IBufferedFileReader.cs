namespace FileManager
{
    public interface IBufferedFileReader
    {
        string ReadLine(out bool success);
    }
}
