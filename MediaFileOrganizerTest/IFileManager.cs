namespace MediaFileOrganizerTest
{
    public interface IFileManager
    {
        string[] GetDirectories(string directoryFrom);
        string[] GetFiles(string directoryFrom);
        void Move(string from, string to);
        void Move(string f, string folderPath, string fileName);
        void DeleteDirectory(string directoryFrom);
        void DeleteFile(string f);
    }
}