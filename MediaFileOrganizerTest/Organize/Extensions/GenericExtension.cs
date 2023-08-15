namespace MediaFileOrganizerTest.Organize.Extensions
{
    public class GenericExtension : Extension
    {
        public GenericExtension(string filePath, IFileManager fileManager) : base(filePath, fileManager)
        { }


        protected override void ExecuteProcess(string directoryTo)
        { }
    }
}
