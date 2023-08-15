using System;
using System.IO;
using System.Linq;

namespace MediaFileOrganizerTest
{
    public class FileManager : IFileManager
    {
        public void DeleteDirectory(string directoryFrom)
            => Directory.Delete(directoryFrom);
        public void DeleteFile(string f)
            => File.Delete(f);
        public string[] GetDirectories(string directoryFrom)
            => Directory.GetDirectories(directoryFrom);
        public string[] GetFiles(string directoryFrom)
            => Directory.GetFiles(directoryFrom);
        public void Move(string from, string to)
        {
            var toFile = to.Split("\\").Last();

            Move(from, to.Replace("\\" + toFile, ""), toFile);
        }
        public void Move(string f, string folderPath, string fileName)
        {
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);
            try
            {
                File.Move(f, $"{folderPath}\\{fileName}");
            }
            catch (Exception)
            {
                Console.WriteLine(fileName);
            }
        }
    }
}
