using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace FileManager
{
    public sealed class LocalFileManager : IFileManager
    {
        public DirectoryInfo GetDirectoryInfo(string path)
        {
            return new DirectoryInfo(path);
        }

        public DirectoryInfo GetDirectoryInfo(string path, string name)
        {
            return GetDirectoryInfo(Path.Combine(path, name));
        }

        public DirectoryInfo CreateDirectory(string path)
        {
            return Directory.CreateDirectory(path);
        }

        public DirectoryInfo CreateDirectory(string path, string name)
        {
            return CreateDirectory(Path.Combine(path, name));
        }

        public DirectoryInfo GetTemporaryDirectory()
        {
            var temp = Path.GetTempPath();
            return CreateDirectory(temp, Guid.NewGuid().ToString());
        }

        public void DeleteDirectory(DirectoryInfo dInfo)
        {
            DeleteDirectory(dInfo.FullName);
        }

        public void DeleteDirectory(string path)
        {
            try
            {
                Directory.Delete(path, true);
            }
            catch(DirectoryNotFoundException)
            {
                Debug.WriteLine("Attempted to delete a non-existing directory");
            }
        }

        public IEnumerable<FileSystemInfo> GetItemsInDirectory(DirectoryInfo directory)
        {
            return directory.EnumerateFileSystemInfos();
        }

        public IEnumerable<FileSystemInfo> GetItemsInDirectory(string path)
        {
            return GetItemsInDirectory(path);
        }

        public IEnumerable<FileSystemInfo> GetItemsInDirectory(string path, string filenamePattern = "*")
        {
            var dInfo = GetDirectoryInfo(path);
            return dInfo.EnumerateFileSystemInfos(filenamePattern);
        }

        public void AppendTextFile(string path, string content, EncodingEnums encoding = EncodingEnums.UTF8)
        {
            File.AppendAllText(path, $"{Environment.NewLine}{content}", encoding.GetEncoding());
        }

        public void AppendTextFile(string path, IEnumerable<string> content, EncodingEnums encoding = EncodingEnums.UTF8)
        {
            File.AppendAllText(path, Environment.NewLine, encoding.GetEncoding());
            File.AppendAllLines(path, content, encoding.GetEncoding());
        }

        public void AppendTextFile(string path, string fileName, string content, EncodingEnums encoding = EncodingEnums.UTF8)
        {
            AppendTextFile(Path.Combine(path, fileName), content, encoding);
        }

        public void AppendTextFile(string path, string fileName, IEnumerable<string> content, EncodingEnums encoding = EncodingEnums.UTF8)
        {
            AppendTextFile(Path.Combine(path, fileName), content, encoding);
        }

        public void AppendTextFile(FileInfo fInfo, string content, EncodingEnums encoding = EncodingEnums.UTF8)
        {
            AppendTextFile(fInfo.FullName, content, encoding);
        }

        public void AppendTextFile(FileInfo fInfo, IEnumerable<string> content, EncodingEnums encoding = EncodingEnums.UTF8)
        {
            AppendTextFile(fInfo.FullName, content, encoding);
        }

        public FileInfo CopyFile(string currentPath, DirectoryInfo destinationInfo, bool overwrite)
        {
            return CopyFile(currentPath, destinationInfo.FullName, overwrite);
        }

        public FileInfo CopyFile(FileInfo currentFInfo, DirectoryInfo destinationInfo, bool overwrite)
        {
            return CopyFile(currentFInfo, destinationInfo.FullName, overwrite);
        }

        public FileInfo CopyFile(string currentPath, DirectoryInfo destinationInfo, string fileName, bool overwrite)
        {
            return CopyFile(currentPath, destinationInfo.FullName, fileName, overwrite);
        }

        public FileInfo CopyFile(FileInfo currentFInfo, DirectoryInfo destinationInfo, string fileName, bool overwrite)
        {
            return CopyFile(currentFInfo, destinationInfo.FullName, fileName, overwrite);
        }

        public FileInfo CopyFile(string currentPath, string newPath, bool overwrite = true)
        {
            if (Directory.Exists(newPath))
                throw new ArgumentException($"Specified path {newPath} is a directory, maybe you want to use a different override of this method");

            bool exists = GetFileExists(newPath);

            if(exists && !overwrite)
                throw new ArgumentException($"Destination path already exists and overwrite flag is set to false. File path {newPath}.");

            if(overwrite)
                DeleteFile(newPath);
            
            File.Copy(currentPath, newPath);

            return GetFileInfo(newPath);
        }

        public FileInfo CopyFile(FileInfo currentFInfo, string newPath, bool overwrite = true)
        {
            return CopyFile(currentFInfo.FullName, newPath, overwrite);
        }

        public FileInfo CopyFile(string currentPath, string newPath, string fileName, bool overwrite = true)
        {
            return CopyFile(currentPath, Path.Combine(newPath, fileName), overwrite);
        }

        public FileInfo CopyFile(FileInfo currentFInfo, string newPath, string fileName, bool overwrite = true)
        {
            return CopyFile(currentFInfo.FullName, newPath, fileName, overwrite);
        }

        public DirectoryInfo CopyDirectory(DirectoryInfo source, DirectoryInfo destination, bool overwrite)
        {
            var items = GetItemsInDirectory(source);
            foreach (var item in items)
            {
                string newItemName = overwrite ?
                    Path.Combine(destination.FullName, item.Name) :
                    GetProposedPathForFile(destination, item.Name);

                if (item.Attributes.HasFlag(FileAttributes.Directory))
                {
                    var subfolder = CreateDirectory(newItemName);
                    CopyDirectory(GetDirectoryInfo(item.FullName), subfolder, overwrite);
                }
                else CopyFile(item.FullName, newItemName, overwrite);
            }
            return destination;
        }

        public void DeleteFile(string path)
        {
            try
            {
                File.Delete(path);
            }
            catch (FileNotFoundException)
            {
                Debug.WriteLine("Attempted to delete a non-exising file");
            }
        }

        public void DeleteFile(FileInfo fInfo)
        {
            DeleteFile(fInfo.FullName);
        }

        public string GetFileDirectory(string path)
        {
            return GetFileInfo(path).Directory.FullName;
        }

        public bool GetFileExists(params string[] path)
        {
            return GetFileInfo(path).Exists;
        }

        public FileInfo GetFileInfo(params string[] path)
        {
            return new FileInfo(Path.Combine(path));
        }

        public string GetTemporaryFilePath()
        {
            return Path.GetTempFileName();
        }

        public string GetProposedPathForDirectory(string path)
        {
            var dInfo = GetDirectoryInfo(path);
            if (!dInfo.Exists)
                return path;
            else
            {
                string cleanPath = RemovePathInvalidCharacters(dInfo.Parent.FullName);
                string cleanName = RemoveFileInvalidCharacters(dInfo.Name);
                int counter = 1;

                do
                {
                    counter++;
                    dInfo = GetDirectoryInfo(Path.Combine(cleanPath, $"{cleanName} ({counter})"));
                }
                while (dInfo.Exists);
            }
            return dInfo.FullName;
        }

        public string GetProposedPathForDirectory(string path, string name)
        {
            return GetProposedPathForDirectory(Path.Combine(path, name));
        }

        public string GetProposedPathForDirectory(DirectoryInfo dInfo, string name)
        {
            return GetProposedPathForDirectory(dInfo.FullName, name);
        }

        public string GetProposedPathForFile(DirectoryInfo dInfo, string fileName)
        {
            return GetProposedPathForFile(dInfo.FullName, fileName);
        }

        public string GetProposedPathForFile(string path, string fileName)
        {
            string cleanPath = RemovePathInvalidCharacters(path);
            string cleanFilename = RemoveFileInvalidCharacters(fileName);
            return GetProposedPathForFile(Path.Combine(cleanPath, cleanFilename));
        }

        public string GetProposedPathForFile(string path)
        {
            var fInfo = GetFileInfo(path);
            if (!fInfo.Exists)
                return path;
            else
            {
                string cleanPath = RemovePathInvalidCharacters(fInfo.Directory.FullName);
                string cleanFilename = RemoveFileInvalidCharacters(Path.GetFileNameWithoutExtension(fInfo.Name));
                string extension = fInfo.Extension;
                int counter = 1;

                do
                {
                    counter++;
                    fInfo = GetFileInfo(Path.Combine(cleanPath, $"{cleanFilename} ({counter}){extension}"));
                }
                while (fInfo.Exists);
            }
            return fInfo.FullName;
        }

        public FileInfo MoveFile(string currentPath, DirectoryInfo destinationInfo, bool overwrite = true)
        {
            return MoveFile(currentPath, destinationInfo.FullName, overwrite);
        }

        public FileInfo MoveFile(FileInfo currentFInfo, DirectoryInfo destinationInfo, bool overwrite = true)
        {
            return MoveFile(currentFInfo, destinationInfo.FullName, overwrite);
        }

        public FileInfo MoveFile(string currentPath, DirectoryInfo destinationInfo, string fileName, bool overwrite = true)
        {
            return MoveFile(currentPath, destinationInfo.FullName, fileName, overwrite);
        }

        public FileInfo MoveFile(FileInfo currentFInfo, DirectoryInfo destinationInfo, string fileName, bool overwrite = true)
        {
            return MoveFile(currentFInfo, destinationInfo.FullName, fileName, overwrite);
        }

        public FileInfo MoveFile(string currentPath, string newPath, bool overwrite = true)
        {
            var fInfo = CopyFile(currentPath, newPath, overwrite);
            DeleteFile(currentPath);
            return fInfo;
        }

        public FileInfo MoveFile(FileInfo currentFInfo, string newPath, bool overwrite = true)
        {
            return MoveFile(currentFInfo.FullName, newPath, overwrite);
        }

        public FileInfo MoveFile(string currentPath, string newPath, string fileName, bool overwrite = true)
        {
            return MoveFile(currentPath, Path.Combine(newPath, fileName), overwrite);
        }

        public FileInfo MoveFile(FileInfo currentFInfo, string newPath, string fileName, bool overwrite = true)
        {
            return MoveFile(currentFInfo.FullName, newPath, overwrite);
        }

        public DirectoryInfo MoveDirectory(DirectoryInfo source, DirectoryInfo destination, bool overwrite = true)
        {
            var copy = CopyDirectory(source, destination, overwrite);            
            DeleteDirectory(source);
            return copy;
        }

        public string ReadAllTextFile(FileInfo fInfo, EncodingEnums encoding = EncodingEnums.UTF8)
        {
            return ReadAllTextFile(fInfo.FullName, encoding);
        }

        public string ReadAllTextFile(string path, EncodingEnums encoding = EncodingEnums.UTF8)
        {
            return File.ReadAllText(path, encoding.GetEncoding());
        }

        public string ReadAllTextFile(string path, string fileName, EncodingEnums encoding = EncodingEnums.UTF8)
        {
            return ReadAllTextFile(Path.Combine(path, fileName), encoding);
        }

        public byte[] ReadBinaryFile(FileInfo fInfo)
        {
            return ReadBinaryFile(fInfo.FullName);
        }

        public byte[] ReadBinaryFile(string path)
        {
            return File.ReadAllBytes(path);
        }

        public byte[] ReadBinaryFile(string path, string fileName)
        {
            return ReadBinaryFile(Path.Combine(path, fileName));
        }

        public IBufferedFileReader ReadBufferedTextFile(FileInfo fInfo)
        {
            return ReadBufferedTextFile(fInfo.FullName);
        }

        public IBufferedFileReader ReadBufferedTextFile(string path)
        {
            return new BufferedFileReader(path);
        }

        public IBufferedFileReader ReadBufferedTextFile(string path, string fileName)
        {
            return new BufferedFileReader(Path.Combine(path, fileName));
        }

        public IEnumerable<string> ReadTextFile(string path, EncodingEnums encoding = EncodingEnums.UTF8)
        {
            return File.ReadAllLines(path, encoding.GetEncoding());
        }

        public IEnumerable<string> ReadTextFile(string path, string fileName, EncodingEnums encoding = EncodingEnums.UTF8)
        {
            return ReadTextFile(Path.Combine(path, fileName), encoding);
        }

        public IEnumerable<string> ReadTextFile(FileInfo fInfo, EncodingEnums encoding = EncodingEnums.UTF8)
        {
            return ReadTextFile(fInfo.FullName, encoding);
        }

        public FileInfo WriteBinaryFile(string path, byte[] content, bool overwrite = true)
        {
            bool exists = GetFileExists(path);
            if (!exists || (exists && overwrite))
            {
                File.WriteAllBytes(path, content);
            }
            return GetFileInfo(path);
        }

        public FileInfo WriteBinaryFile(string path, string fileName, byte[] content, bool overwrite = true)
        {
            return WriteBinaryFile(Path.Combine(path, fileName), content, overwrite);
        }

        public FileInfo WriteTextFile(string path, string content, EncodingEnums encoding = EncodingEnums.UTF8, bool overwrite = true)
        {
            bool exists = GetFileExists(path);
            if (!exists || (exists && overwrite))
            {
                File.WriteAllText(path, content, encoding.GetEncoding());
            }
            return GetFileInfo(path);
        }

        public FileInfo WriteTextFile(string path, IEnumerable<string> content, EncodingEnums encoding = EncodingEnums.UTF8, bool overwrite = true)
        {
            bool exists = GetFileExists(path);
            if (!exists || (exists && overwrite))
            {
                File.WriteAllLines(path, content, encoding.GetEncoding());
            }
            return GetFileInfo(path);
        }

        public FileInfo WriteTextFile(string path, byte[] content, EncodingEnums encoding, bool overwrite)
        {
            var stringContent = encoding.GetEncoding().GetString(content);
            return WriteTextFile(path, stringContent, encoding, overwrite);
        }

        public FileInfo WriteTextFile(string path, IEnumerable<byte[]> content, EncodingEnums encoding, bool overwrite)
        {
            var stringContent = content.Select(x => encoding.GetEncoding().GetString(x));
            return WriteTextFile(path, stringContent, encoding, overwrite);
        }

        public FileInfo WriteTextFile(string path, string fileName, byte[] content, EncodingEnums encoding, bool overwrite)
        {
            var stringContent = encoding.GetEncoding().GetString(content);
            return WriteTextFile(path, fileName, stringContent, encoding, overwrite);
        }

        public FileInfo WriteTextFile(string path, string fileName, IEnumerable<byte[]> content, EncodingEnums encoding, bool overwrite)
        {
            var stringContent = content.Select(x => encoding.GetEncoding().GetString(x));
            return WriteTextFile(path, fileName, stringContent, encoding, overwrite);
        }

        public FileInfo WriteTextFile(FileInfo fInfo, byte[] content, EncodingEnums encoding, bool overwrite)
        {
            var stringContent = encoding.GetEncoding().GetString(content);
            return WriteTextFile(fInfo, stringContent, encoding, overwrite);
        }

        public FileInfo WriteTextFile(FileInfo fInfo, IEnumerable<byte[]> content, EncodingEnums encoding, bool overwrite)
        {
            var stringContent = content.Select(x => encoding.GetEncoding().GetString(x));
            return WriteTextFile(fInfo, stringContent, encoding, overwrite);
        }

        public FileInfo WriteTextFile(string path, string fileName, string content, EncodingEnums encoding = EncodingEnums.UTF8, bool overwrite = true)
        {
            return WriteTextFile(Path.Combine(path, fileName), content, encoding, overwrite);
        }

        public FileInfo WriteTextFile(string path, string fileName, IEnumerable<string> content, EncodingEnums encoding = EncodingEnums.UTF8, bool overwrite = true)
        {
            return WriteTextFile(Path.Combine(path, fileName), content, encoding, overwrite);
        }

        public FileInfo WriteTextFile(FileInfo fInfo, string content, EncodingEnums encoding = EncodingEnums.UTF8, bool overwrite = true)
        {
            return WriteTextFile(fInfo.FullName, content, encoding, overwrite);
        }

        public FileInfo WriteTextFile(FileInfo fInfo, IEnumerable<string> content, EncodingEnums encoding = EncodingEnums.UTF8, bool overwrite = true)
        {
            return WriteTextFile(fInfo.FullName, content, encoding, overwrite);
        }

        public FileInfo CreateZipFile(DirectoryInfo dInfo, string fileName, bool overwrite, params string[] zippedContent)
        {
            return CreateZipFile(dInfo.FullName, fileName, overwrite, zippedContent);
        }

        public FileInfo CreateZipFile(string path, string fileName, bool overwrite, params string[] zippedContent)
        {
            string destinationPath = Path.Combine(path, fileName);
            return CreateZipFile(destinationPath, overwrite, zippedContent);
        }

        public FileInfo CreateZipFile(FileInfo fInfo, bool overwrite, params string[] zippedContent)
        {
            return CreateZipFile(fInfo.FullName, overwrite, zippedContent);
        }

        public FileInfo CreateZipFile(string path, bool overwrite, params string[] zippedContent)
        {
            var tempDir = GetTemporaryDirectory();

            foreach(var item in zippedContent)
            {
                if (Directory.Exists(item))
                {
                    var tempSubfolderName = GetDirectoryInfo(item).Name;
                    CopyDirectory(GetDirectoryInfo(item), CreateDirectory(tempDir.FullName, tempSubfolderName), true);
                }
                else
                    CopyFile(item, tempDir, GetFileInfo(item).Name, true);
            }

            if (overwrite)
                DeleteFile(path);
            
            ZipFile.CreateFromDirectory(tempDir.FullName, path, CompressionLevel.Optimal, false);
            DeleteDirectory(tempDir);
            return GetFileInfo(path);
        }

        public DirectoryInfo ExtractZipFile(string path, string fileName, string destinationPath, bool overwrite)
        {
            return ExtractZipFile(Path.Combine(path, fileName), destinationPath, overwrite);
        }

        public DirectoryInfo ExtractZipFile(FileInfo source, string destinationPath, bool overwrite)
        {
            return ExtractZipFile(source.FullName, destinationPath, overwrite);
        }

        public DirectoryInfo ExtractZipFile(string path, string fileName, DirectoryInfo destination, bool overwrite)
        {
            return ExtractZipFile(Path.Combine(path, fileName), destination, overwrite);
        }

        public DirectoryInfo ExtractZipFile(FileInfo source, DirectoryInfo destination, bool overwrite)
        {
            return ExtractZipFile(source.FullName, destination, overwrite);
        }

        public DirectoryInfo ExtractZipFile(string path, DirectoryInfo destination, bool overwrite)
        {
            return ExtractZipFile(path, destination.FullName, overwrite);
        }

        public DirectoryInfo ExtractZipFile(string path, string destinationPath, bool overwrite)
        {   
            ZipFile.ExtractToDirectory(path, destinationPath, overwrite);
            return GetDirectoryInfo(destinationPath);
        }

        private string RemovePathInvalidCharacters(string path)
        {
            return RemoveInvalidCharacters(path, Path.GetInvalidPathChars());
        }

        private string RemoveFileInvalidCharacters(string path)
        {
            return RemoveInvalidCharacters(path, Path.GetInvalidFileNameChars());
        }

        private string RemoveInvalidCharacters(string path, char[] invalidChars)
        {
            string cleanPath = path;

            foreach (char c in invalidChars)
                cleanPath = cleanPath.Replace(c, '-');

            return cleanPath;
        }

        public string GetTextSha512Hash(string value)
        {
            var bytes = Encoding.UTF8.GetBytes(value);
            return GetSha512Hash(bytes);
        }

        public string GetFileSha512Hash(FileInfo fInfo)
        {
            return GetFileSha512Hash(fInfo.FullName);
        }

        public string GetFileSha512Hash(string path)
        {
            var bytes = ReadBinaryFile(path);
            return GetSha512Hash(bytes);
        }

        private string GetSha512Hash(byte[] value)
        {
            using (var hash512 = new SHA512Managed())
            {
                var hash = hash512.ComputeHash(value);
                return ConvertHashArrayToString(hash);
            }
        }

        public string GetTextSha256Hash(string value)
        {
            var bytes = Encoding.UTF8.GetBytes(value);
            return GetSha256Hash(bytes);
        }

        public string GetFileSha256Hash(FileInfo fInfo)
        {
            return GetFileSha256Hash(fInfo.FullName);
        }

        public string GetFileSha256Hash(string path)
        {
            var bytes = ReadBinaryFile(path);
            return GetSha256Hash(bytes);
        }

        private string GetSha256Hash(byte[] value)
        {
            using (var hash256 = new SHA256Managed())
            {
                var hash = hash256.ComputeHash(value);
                return ConvertHashArrayToString(hash);
            }
        }

        public string GetTextMd5Hash(string value)
        {
            var bytes = Encoding.UTF8.GetBytes(value);
            return GetMd5Hash(bytes);
        }

        public string GetFileMd5Hash(FileInfo fInfo)
        {
            return GetFileMd5Hash(fInfo.FullName);
        }

        public string GetFileMd5Hash(string path)
        {
            var bytes = ReadBinaryFile(path);
            return GetMd5Hash(bytes);
        }

        private string GetMd5Hash(byte[] value)
        {
            using (var hashmd5 = MD5.Create())
            {
                var hash = hashmd5.ComputeHash(value);
                return ConvertHashArrayToString(hash);
            }
        }

        public string ConvertHashArrayToString(byte[] hashedValue)
        {
            var sb = new StringBuilder();
            foreach (Byte b in hashedValue)
                sb.Append(b.ToString("x2"));
            return sb.ToString();
        }

        public string EncodeTextBase64String(string value)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(value);
            return Convert.ToBase64String(plainTextBytes);
        }

        public string EncodeFileBase64String(string path)
        {
            var fInfo = new FileInfo(path);
            return EncodeFileBase64String(fInfo);
        }

        public string EncodeFileBase64String(FileInfo fInfo)
        {
            if (fInfo.Length >= 192000000)
                throw new FileLoadException("Maximum file size allowed for Base 64 is 192 MB.");
            else
            {
                var contentBytes = ReadBinaryFile(fInfo.FullName);
                return Convert.ToBase64String(contentBytes);
            }
        }

        public string DecodeTextBase64String(string value)
        {
            var base64EncodedBytes = Convert.FromBase64String(value);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public byte[] DecodeFileBase64String(string value)
        {
            return Convert.FromBase64String(value);
        }
    }
}
