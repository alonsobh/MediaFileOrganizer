using System;
using System.Linq;

namespace MediaFileOrganizerTest.Organize.Extensions
{
    public abstract class Extension
    {
        protected IFileManager FileManager { get; }
        protected string OriginalFilePath { get; private set;}
        protected string OriginalFileName { get; private set;}

        public Extension(string filePath, IFileManager fileManager)
        {
            FileManager = fileManager;
            SetVariables(filePath);
        }

        protected void SetVariables(string filePath)
        {
            OriginalFilePath = filePath;
            var path = OriginalFilePath.Split("\\");
            OriginalFileName = path.Last();
            var parentFolder = path[path.Length - 2];
            if (OriginalFileName[0..3].ToUpper() != parentFolder[0..3])
                throw new Exception();
        }

        public void Process(string directoryTo)
        {
            if (ShouldDelete(OriginalFileName))
                FileManager.DeleteFile(OriginalFilePath);
            else
                ExecuteProcess(directoryTo);
        }
        protected abstract void ExecuteProcess(string directoryTo);
        protected bool ShouldDelete(string f)
        {
            var path = f.Split("\\");
            var fileName = path.Last();
            return FilesToDelete.Contains(fileName.ToUpper())
                || ExtensionsToDelete.Any(f => fileName.ToUpper().EndsWith($".{f.ToUpper()}"));
        }
        string[] ExtensionsToDelete { get; } = new string[]
        {
            "nfo"
        };
        string[] FilesToDelete { get; } = new[]
        {
            "RARBG.TXT",
            "RARBG_DO_NOT_MIRROR.EXE"
        };
    }
}
