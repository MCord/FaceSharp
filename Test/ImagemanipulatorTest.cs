using NUnit.Framework;
using Studio;
using Studio.Normalization;

namespace Test
{
    public class ImagemanipulatorTest
    {
        [Test]
        public void Test()
        {
            var settings = new NormalizationSetting
            {
                SourceFolder = @"H:\Source",
                TargetFolder = @"H:\PI"
            };


            settings.SerializeToFile(@"H:\Norm.xml");

            var im = new ImageManipulator(settings);
            im.Normalize();
        }
    }
}