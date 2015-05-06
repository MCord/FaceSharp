using NUnit.Framework;
using Studio;

namespace Test
{
    public class ImagemanipulatorTest
    {
        [Test]
        public void Test()
        {
            var im = new ImageManipulator();

            im.NormalizeFolder(@"H:\Source");
        }
    }
}