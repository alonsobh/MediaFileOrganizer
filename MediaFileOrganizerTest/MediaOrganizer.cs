using MediaFileOrganizerTest.Organize;
using System;
using System.Linq;

namespace MediaFileOrganizerTest
{
    public class MediaOrganizer
    {
        string Unit { get; }

        public MediaOrganizer(string unit)
            => Unit = unit;
        IFileManager FileManager { get; set; } = new FileManager();
        internal void SetFileManager(IFileManager fileManager)
            => FileManager = fileManager;

        internal void Organize()
        {
            var directoryFrom = $"{Unit}:\\#Pending";

            var subDirectories = FileManager.GetDirectories(directoryFrom).OrderBy(s => s);
            foreach (var d in subDirectories)
                MoveFiles(d);
            var files = FileManager.GetFiles(directoryFrom).OrderBy(s => s);
            foreach (var f in files)
                MoveFile(f);
        }

        private void MoveFiles(string directoryFrom)
        {
            var subDirectories = FileManager.GetDirectories(directoryFrom).OrderBy(s => s);
            foreach (var d in subDirectories)
                MoveFiles(d);
            var files = FileManager.GetFiles(directoryFrom).OrderBy(s => s);
            foreach (var f in files)
                try
                {
                    MoveFile(f);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            files = FileManager.GetFiles(directoryFrom).OrderBy(s => s);
            subDirectories = FileManager.GetDirectories(directoryFrom).OrderBy(s => s);
            if (!files.Any() && !subDirectories.Any() && !directoryFrom.EndsWith("#TMP"))
                FileManager.DeleteDirectory(directoryFrom);
        }


        private void MoveFile(string f)
            => new FileManagement(FileManager).MoveFile(f, Unit);
    }
}
