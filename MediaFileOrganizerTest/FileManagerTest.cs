using System.Collections.Generic;
using System.Linq;

namespace MediaFileOrganizerTest
{
    public class FileManagerTest : IFileManager
    {
        string slash { get; } = "\\";
        public string[] GetDirectories(string directoryFrom)
        {
            var count = directoryFrom.Split(slash).Count();
            return files
                    .Where(f => f.StartsWith(directoryFrom)
                            && count + 1 < f.Split(slash).Count())
                    .Select(f =>
                    {
                        var s = f.Split(slash);
                        var c = count + 1;
                        if (c == s.Count())
                            c--;
                        var len = s[0..c];
                        return string.Join(slash, len);
                    })
                    .Distinct()
                    .ToArray();
        }

        public string[] GetFiles(string directoryFrom)
        {
            var count = directoryFrom.Split(slash).Count() + 1;
            return files
                    .Where(f => f.StartsWith(directoryFrom)
                            && count == f.Split(slash).Count())
                    .ToArray();
        }

        public void Move(string f, string folderPath, string fileName)
        {
            files.Remove(f);
            files.Add($"{folderPath}\\{fileName}");
        }

        public void DeleteDirectory(string directoryFrom)
        { }

        public void DeleteFile(string f)
            => files.Remove(f);
        public void Move(string from, string to)
        {
            files.Remove(from);
            files.Add(to);
        }

        public List<string> files = new List<string>();
    }
}