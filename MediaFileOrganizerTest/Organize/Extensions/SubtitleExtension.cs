namespace MediaFileOrganizerTest.Organize.Extensions
{
    public class SubtitleExtension : Extension
    {
        public SubtitleExtension(string filePath, IFileManager fileManager) : base(filePath, fileManager)
        { }


        protected override void ExecuteProcess(string directoryTo)
        {
            if (OriginalFileName.EndsWith(".Eng.srt"))
            {
                var newPathFile = OriginalFilePath.Replace(".Eng.srt", ".srt");
                FileManager.Move(OriginalFilePath, newPathFile);
                SetVariables(newPathFile);
            }
        }
    }
}
