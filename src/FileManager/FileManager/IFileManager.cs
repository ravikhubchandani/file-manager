using System.Collections.Generic;
using System.IO;

namespace FileManager
{
    public interface IFileManager
    {
        /// <summary>
        /// Get information of specified directory. Folder name is included in the path
        /// </summary>
        DirectoryInfo GetDirectoryInfo(string path);

        /// <summary>
        /// Get information of specified directory. Folder name is NOT included in the path
        /// </summary>
        DirectoryInfo GetDirectoryInfo(string path, string name);

        /// <summary>
        /// Create new directory in the specified path. Folder name is included in the path
        /// </summary>
        DirectoryInfo CreateDirectory(string path);

        /// <summary>
        /// Create new directory in the specified path. Folder name is NOT included in the path
        /// </summary>
        DirectoryInfo CreateDirectory(string path, string name);
        
        /// <summary>
        /// Creates a temporary directory
        /// </summary>
        DirectoryInfo GetTemporaryDirectory();

        /// <summary>
        /// Delete directory in the specified path. Folder name is included in the path
        /// </summary>
        void DeleteDirectory(string path);

        /// <summary>
        /// Delete directory in the specified path. Folder name is NOT included in the path
        /// </summary>
        void DeleteDirectory(DirectoryInfo dInfo);

        /// <summary>
        /// Enumerate all items in the specified path. Folder name is included in the path
        /// </summary>
        IEnumerable<FileSystemInfo> GetItemsInDirectory(string path);

        /// <summary>
        /// Enumerate all items in the specified directory
        /// </summary>
        IEnumerable<FileSystemInfo> GetItemsInDirectory(DirectoryInfo directory);

        /// <summary>
        /// Enumerate all items in the specified path. Folder name is NOT included in the path
        /// </summary>
        IEnumerable<FileSystemInfo> GetItemsInDirectory(string path, string filenamePattern);


        /// <summary>
        /// Returns True is the file specified exists
        /// </summary>
        bool GetFileExists(string path);

        /// <summary>
        /// Get information of specified file. File name is included in the path
        /// </summary>
        FileInfo GetFileInfo(string path);

        /// <summary>
        /// Returns the full path of the directory where the specified file is
        /// </summary>
        string GetFileDirectory(string path);

        /// <summary>
        /// Returns the path for a unique file that will be deleted automatically when not needed anymore
        /// </summary>
        string GetTemporaryFilePath();

        /// <summary>
        /// Returns full path of a uniquely named file. Eg. If file 'Test.txt' exists. Full path for 'Test (2).txt' will be returned.
        /// </summary>
        string GetProposedPathForFile(string path);

        /// <summary>
        /// Returns full path of a uniquely named file using argument path as base directory. Eg. If file 'Test.txt' exists in specified path. Full path for 'Test (2).txt' will be returned.
        /// </summary>
        string GetProposedPathForFile(string path, string fileName);

        /// <summary>
        /// Returns full path of a uniquely named file using argument dInfo as base directory. Eg. If file 'Test.txt' exists in specified path. Full path for 'Test (2).txt' will be returned.
        /// </summary>
        string GetProposedPathForFile(DirectoryInfo dInfo, string fileName);

        /// <summary>
        /// Returns full path of a uniquely named folder. Eg. If folder 'New folder' exists. Full path for 'New folder (2)' will be returned.
        /// </summary>
        string GetProposedPathForDirectory(string path);

        /// <summary>
        /// Returns full path of a uniquely named directory using argument name as base directory. Eg. If file 'New folder' exists in specified path. Full path for 'New folder (2)' will be returned.
        /// </summary>
        string GetProposedPathForDirectory(string path, string name);

        /// <summary>
        /// Returns full path of a uniquely named directory using argument dInfo as base directory. Eg. If file 'New folder' exists in specified path. Full path for 'New folder (2)' will be returned.
        /// </summary>
        string GetProposedPathForDirectory(DirectoryInfo dInfo, string name);

        string ReadAllTextFile(FileInfo fInfo, EncodingEnums encoding);
        string ReadAllTextFile(string path, EncodingEnums encoding);
        string ReadAllTextFile(string path, string fileName, EncodingEnums encoding);

        IEnumerable<string> ReadTextFile(string path, EncodingEnums encoding);
        IEnumerable<string> ReadTextFile(string path, string fileName, EncodingEnums encoding);
        IEnumerable<string> ReadTextFile(FileInfo fInfo, EncodingEnums encoding);

        byte[] ReadBinaryFile(FileInfo fInfo);
        byte[] ReadBinaryFile(string path);
        byte[] ReadBinaryFile(string path, string fileName);

        /// <summary>
        /// Returns reader to read file sequentially line by line
        /// </summary>
        IBufferedFileReader ReadBufferedTextFile(FileInfo fInfo);

        /// <summary>
        /// Returns reader to read file sequentially line by line. Specied path contains file name
        /// </summary>
        IBufferedFileReader ReadBufferedTextFile(string path);

        /// <summary>
        /// Returns reader to read file sequentially line by line. Specied path does NOT contain file name
        /// </summary>
        IBufferedFileReader ReadBufferedTextFile(string path, string fileName);

        FileInfo WriteTextFile(string path, string content, EncodingEnums encoding, bool overwrite);
        FileInfo WriteTextFile(string path, IEnumerable<string> content, EncodingEnums encoding, bool overwrite);
        FileInfo WriteTextFile(string path, string fileName, string content, EncodingEnums encoding, bool overwrite);
        FileInfo WriteTextFile(string path, string fileName, IEnumerable<string> content, EncodingEnums encoding, bool overwrite);
        FileInfo WriteTextFile(FileInfo fInfo, string content, EncodingEnums encoding, bool overwrite);
        FileInfo WriteTextFile(FileInfo fInfo, IEnumerable<string> content, EncodingEnums encoding, bool overwrite);
        FileInfo WriteTextFile(string path, byte[] content, EncodingEnums encoding, bool overwrite);
        FileInfo WriteTextFile(string path, IEnumerable<byte[]> content, EncodingEnums encoding, bool overwrite);
        FileInfo WriteTextFile(string path, string fileName, byte[] content, EncodingEnums encoding, bool overwrite);
        FileInfo WriteTextFile(string path, string fileName, IEnumerable<byte[]> content, EncodingEnums encoding, bool overwrite);
        FileInfo WriteTextFile(FileInfo fInfo, byte[] content, EncodingEnums encoding, bool overwrite);
        FileInfo WriteTextFile(FileInfo fInfo, IEnumerable<byte[]> content, EncodingEnums encoding, bool overwrite);

        FileInfo WriteBinaryFile(string path, byte[] content, bool overwrite);
        FileInfo WriteBinaryFile(string path, string fileName, byte[] content, bool overwrite);
        FileInfo CreateZipFile(string path, bool overwrite, params string[] zippedContent);
        FileInfo CreateZipFile(string path, string fileName, bool overwrite, params string[] zippedContent);
        FileInfo CreateZipFile(FileInfo fInfo, bool overwrite, params string[] zippedContent);
        FileInfo CreateZipFile(DirectoryInfo dInfo, string fileName, bool overwrite, params string[] zippedContent);
        DirectoryInfo ExtractZipFile(string path, string destinationPath, bool overwrite);
        DirectoryInfo ExtractZipFile(string path, string fileName, string destinationPath, bool overwrite);
        DirectoryInfo ExtractZipFile(FileInfo source, string destinationPath, bool overwrite);
        DirectoryInfo ExtractZipFile(string path, DirectoryInfo destination, bool overwrite);
        DirectoryInfo ExtractZipFile(string path, string fileName, DirectoryInfo destination, bool overwrite);
        DirectoryInfo ExtractZipFile(FileInfo source, DirectoryInfo destination, bool overwrite);

        void AppendTextFile(string path, string content, EncodingEnums encoding);
        void AppendTextFile(string path, IEnumerable<string> content, EncodingEnums encoding);
        void AppendTextFile(string path, string fileName, string content, EncodingEnums encoding);
        void AppendTextFile(string path, string fileName, IEnumerable<string> content, EncodingEnums encoding);
        void AppendTextFile(FileInfo fInfo, string content, EncodingEnums encoding);
        void AppendTextFile(FileInfo fInfo, IEnumerable<string> content, EncodingEnums encoding);

        FileInfo MoveFile(string currentPath, string newPath, bool overwrite);
        FileInfo MoveFile(FileInfo currentFInfo, string newPath, bool overwrite);
        FileInfo MoveFile(string currentPath, string newPath, string fileName, bool overwrite);
        FileInfo MoveFile(FileInfo currentFInfo, string newPath, string fileName, bool overwrite);
        FileInfo MoveFile(string currentPath, DirectoryInfo destinationInfo, bool overwrite);
        FileInfo MoveFile(FileInfo currentFInfo, DirectoryInfo destinationInfo, bool overwrite);
        FileInfo MoveFile(string currentPath, DirectoryInfo destinationInfo, string fileName, bool overwrite);
        FileInfo MoveFile(FileInfo currentFInfo, DirectoryInfo destinationInfo, string fileName, bool overwrite);
        DirectoryInfo MoveDirectory(DirectoryInfo source, DirectoryInfo destination, bool overwrite);

        FileInfo CopyFile(string currentPath, DirectoryInfo destinationInfo, bool overwrite);
        FileInfo CopyFile(FileInfo currentFInfo, DirectoryInfo destinationInfo, bool overwrite);
        FileInfo CopyFile(string currentPath, DirectoryInfo destinationInfo, string fileName, bool overwrite);
        FileInfo CopyFile(FileInfo currentFInfo, DirectoryInfo destinationInfo, string fileName, bool overwrite);
        FileInfo CopyFile(string currentPath, string newPath, bool overwrite);
        FileInfo CopyFile(FileInfo currentFInfo, string newPath, bool overwrite);
        FileInfo CopyFile(string currentPath, string newPath, string fileName, bool overwrite);
        FileInfo CopyFile(FileInfo currentFInfo, string newPath, string fileName, bool overwrite);

        /// <summary>
        /// Recursively copies source directory to destination directory
        /// </summary>
        DirectoryInfo CopyDirectory(DirectoryInfo source, DirectoryInfo destination, bool overwrite);

        void DeleteFile(string path);
        void DeleteFile(FileInfo fInfo);

        string GetTextSha256Hash(string value);
        string GetFileSha256Hash(string path);
        string GetFileSha256Hash(FileInfo fInfo);
        string GetTextSha512Hash(string value);
        string GetFileSha512Hash(string path);
        string GetFileSha512Hash(FileInfo fInfo);
        string GetTextMd5Hash(string value);
        string GetFileMd5Hash(string path);
        string GetFileMd5Hash(FileInfo fInfo);
        string EncodeTextBase64String(string value);
        string EncodeFileBase64String(string path);
        string EncodeFileBase64String(FileInfo fInfo);
        string DecodeTextBase64String(string value);
        byte[] DecodeFileBase64String(string value);        
    }
}
