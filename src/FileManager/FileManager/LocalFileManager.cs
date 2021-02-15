using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace FileManager
{
    public class LocalFileManager : IFileManager
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
            return CreateDirectory(temp, DateTime.Now.ToString("yyyyHHmmss"));
        }

        public void DeleteDirectory(DirectoryInfo dInfo)
        {
            DeleteDirectory(dInfo.FullName);
        }

        public void DeleteDirectory(string path)
        {
            try
            {
                Directory.Delete(path);
            }
            catch(DirectoryNotFoundException)
            {
                Debug.WriteLine("Attempted to delete a non-existing directory");
            }
        }

        public IEnumerable<FileInfo> GetFilesInDirectory(string path)
        {
            var dInfo = GetDirectoryInfo(path);
            return dInfo.GetFiles();
        }

        public IEnumerable<FileInfo> GetFilesInDirectory(string path, string filenamePattern = "*", SearchOption option = SearchOption.TopDirectoryOnly)
        {
            var dInfo = GetDirectoryInfo(path);
            return dInfo.GetFiles(filenamePattern, option);
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

        public FileInfo Copy(string currentPath, string newPath, bool overwrite = true)
        {
            bool exists = GetFileExists(newPath);
            if(exists && !overwrite)
                throw new ArgumentException($"Destination path already exists and overwrite flag is set to false. File path {newPath}.");

            if(overwrite)
                DeleteFile(newPath);
            
            File.Copy(currentPath, newPath);

            return GetFileInfo(newPath);
        }

        public FileInfo Copy(FileInfo currentFInfo, string newPath, bool overwrite = true)
        {
            return Copy(currentFInfo.FullName, newPath, overwrite);
        }

        public FileInfo Copy(string currentPath, string newPath, string fileName, bool overwrite = true)
        {
            return Copy(currentPath, Path.Combine(newPath, fileName), overwrite);
        }

        public FileInfo Copy(FileInfo currentFInfo, string newPath, string fileName, bool overwrite = true)
        {
            return Copy(currentFInfo.FullName, newPath, fileName, overwrite);
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

        public bool GetFileExists(string path)
        {
            return GetFileInfo(path).Exists;
        }

        public FileInfo GetFileInfo(string path)
        {
            return new FileInfo(path);
        }

        public string GetTemporaryFilePath()
        {
            return Path.GetTempFileName();
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

        public FileInfo Move(string currentPath, string newPath, bool overwrite = true)
        {
            var fInfo = Copy(currentPath, newPath, overwrite);
            DeleteFile(currentPath);
            return fInfo;
        }

        public FileInfo Move(FileInfo currentFInfo, string newPath, bool overwrite = true)
        {
            return Move(currentFInfo.FullName, newPath, overwrite);
        }

        public FileInfo Move(string currentPath, string newPath, string fileName, bool overwrite = true)
        {
            return Move(currentPath, Path.Combine(newPath, fileName), overwrite);
        }

        public FileInfo Move(FileInfo currentFInfo, string newPath, string fileName, bool overwrite = true)
        {
            return Move(currentFInfo.FullName, newPath, overwrite);
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
    }
}
