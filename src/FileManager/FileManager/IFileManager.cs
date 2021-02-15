using System.Collections.Generic;
using System.IO;

namespace FileManager
{
    public interface IFileManager
    {
        DirectoryInfo GetDirectoryInfo(string path);
        DirectoryInfo GetDirectoryInfo(string path, string name);
        DirectoryInfo CreateDirectory(string path);
        DirectoryInfo CreateDirectory(string path, string name);
        DirectoryInfo GetTemporaryDirectory();
        void DeleteDirectory(string path);
        void DeleteDirectory(DirectoryInfo dInfo);
        IEnumerable<FileInfo> GetFilesInDirectory(string path);
        IEnumerable<FileInfo> GetFilesInDirectory(string path, string filenamePattern, SearchOption options);

        bool GetFileExists(string path);
        FileInfo GetFileInfo(string path);
        string GetFileDirectory(string path);
        string GetTemporaryFilePath();
        string GetProposedPathForFile(string path);
        string GetProposedPathForFile(string path, string fileName);

        string ReadAllTextFile(FileInfo fInfo, EncodingEnums encoding);
        string ReadAllTextFile(string path, EncodingEnums encoding);
        string ReadAllTextFile(string path, string fileName, EncodingEnums encoding);

        IEnumerable<string> ReadTextFile(string path, EncodingEnums encoding);
        IEnumerable<string> ReadTextFile(string path, string fileName, EncodingEnums encoding);
        IEnumerable<string> ReadTextFile(FileInfo fInfo, EncodingEnums encoding);

        byte[] ReadBinaryFile(FileInfo fInfo);
        byte[] ReadBinaryFile(string path);
        byte[] ReadBinaryFile(string path, string fileName);

        IBufferedFileReader ReadBufferedTextFile(FileInfo fInfo);
        IBufferedFileReader ReadBufferedTextFile(string path);
        IBufferedFileReader ReadBufferedTextFile(string path, string fileName);

        FileInfo WriteTextFile(string path, string content, EncodingEnums encoding, bool overwrite);
        FileInfo WriteTextFile(string path, IEnumerable<string> content, EncodingEnums encoding, bool overwrite);
        FileInfo WriteTextFile(string path, string fileName, string content, EncodingEnums encoding, bool overwrite);
        FileInfo WriteTextFile(string path, string fileName, IEnumerable<string> content, EncodingEnums encoding, bool overwrite);
        FileInfo WriteTextFile(FileInfo fInfo, string content, EncodingEnums encoding, bool overwrite);
        FileInfo WriteTextFile(FileInfo fInfo, IEnumerable<string> content, EncodingEnums encoding, bool overwrite);

        FileInfo WriteBinaryFile(string path, byte[] content, bool overwrite);
        FileInfo WriteBinaryFile(string path, string fileName, byte[] content, bool overwrite);

        void AppendTextFile(string path, string content, EncodingEnums encoding);
        void AppendTextFile(string path, IEnumerable<string> content, EncodingEnums encoding);
        void AppendTextFile(string path, string fileName, string content, EncodingEnums encoding);
        void AppendTextFile(string path, string fileName, IEnumerable<string> content, EncodingEnums encoding);
        void AppendTextFile(FileInfo fInfo, string content, EncodingEnums encoding);
        void AppendTextFile(FileInfo fInfo, IEnumerable<string> content, EncodingEnums encoding);

        FileInfo Move(string currentPath, string newPath, bool overwrite);
        FileInfo Move(FileInfo currentFInfo, string newPath, bool overwrite);
        FileInfo Move(string currentPath, string newPath, string fileName, bool overwrite);
        FileInfo Move(FileInfo currentFInfo, string newPath, string fileName, bool overwrite);

        FileInfo Copy(string currentPath, string newPath, bool overwrite);
        FileInfo Copy(FileInfo currentFInfo, string newPath, bool overwrite);
        FileInfo Copy(string currentPath, string newPath, string fileName, bool overwrite);
        FileInfo Copy(FileInfo currentFInfo, string newPath, string fileName, bool overwrite);

        void DeleteFile(string path);
        void DeleteFile(FileInfo fInfo);
    }
}
