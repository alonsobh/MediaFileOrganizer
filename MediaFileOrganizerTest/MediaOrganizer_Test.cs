using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace MediaFileOrganizerTest
{
    [TestClass]
    public class MediaOrganizer_Test
    {
        [DataTestMethod]
        [DataRow("S")]
        [DataRow("M")]
        public void OrganizeFiles(string disc)
        {
            var organizer = new MediaOrganizer(disc);
            var fileManager = new FileManagerTest();
            organizer.SetFileManager(fileManager);
            organizer.Organize();
        }

        [TestMethod]
        [DataRow("Marvels.The.Punisher.S01.Webrip.X264-Ion10\\Marvels.The.Punisher.S01e01.Webrip.X264-Ion10.mp4"
               , "Marvels.The.Punisher.S01.Webrip.X264-Ion10\\Marvels.The.Punisher.S01e01.Webrip.X264-Ion10.mp4")]
        [DataRow("folder.name.S01.rip\\folder.name.S01e01.rip.mp4"
               , "Folder.Name.S01.Rip\\Folder.Name.S01e01.Rip.mp4")]
        [DataRow("B.Positive.S01.WEBRip.x264-ION10\\B.Positive.S01E01.WEBRip.x264-ION10.mp4"
               , "B.Positive.S01.Webrip.X264-Ion10\\B.Positive.S01e01.Webrip.X264-Ion10.mp4")]
        [DataRow("B.Positive.S01.WEBRip.x264-ION10\\Subs\\B.Positive.S01E01.WEBRip.x264-ION10\\2_English.srt"
               , "B.Positive.S01.Webrip.X264-Ion10\\B.Positive.S01e01.Webrip.X264-Ion10.2_English.srt")]
        [DataRow("Marvels.The.Punisher.S01.Webrip.X264-Ion10\\Marvels.The.Punisher.S01e02.Webrip.X264-Ion10.4_French.srt"
               , "4_French\\Marvels.The.Punisher.S01.Webrip.X264-Ion10\\Marvels.The.Punisher.S01e02.Webrip.X264-Ion10.4_French.srt")]
        public void OrganizeFiles_Move(string from, string to)
        {
            var organizer = new MediaOrganizer("S");
            var fileManager = new FileManagerTest();
            organizer.SetFileManager(fileManager);
            fileManager.files.Add($"S:\\#Pending\\{from}");


            organizer.Organize();

            Assert.AreEqual($"S:\\#Ordenado\\{to}", fileManager.files.Single());
        }

        [TestMethod]
        [DataRow("B.Positive.S01.WEBRip.x264-ION10\\RARBG.txt")]
        public void OrganizeFiles_Delete(string from)
        {
            var organizer = new MediaOrganizer("S");
            var fileManager = new FileManagerTest();
            organizer.SetFileManager(fileManager);
            fileManager.files.Add($"S:\\#Pending\\{from}");


            organizer.Organize();


            Assert.IsFalse(fileManager.files.Any());
        }
    }
}
