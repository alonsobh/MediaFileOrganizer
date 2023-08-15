using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MediaFileOrganizerTest
{
    //[Ignore]
    [TestClass]
    public class Rename
    {
        //[Ignore]
        [TestMethod]
        public void DoOrganize()
        {
            var units = new[]{
                //"A",
                //"B",
                //"D",
                "M",
                "S",
                //"T",
                //"U"
                };
            //while (true)
            //{
            foreach (var unit in units)
                new MediaOrganizer(unit).Organize();
            //    Thread.Sleep(10000);
            //}
            Assert.Fail();
        }
    }
}
