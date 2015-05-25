using System.Collections.Generic;
using System.Drawing;
using NUnit.Framework;
using Studio;
using Studio.Common;
using Xunit;

namespace Test
{
    public class ProjectTests
    {
        [Fact]
        public void ShouldSaveProjectObjectToXml()
        {
            //const string file = "file";
            //var expected = new Project(file, "TestProject");


            //expected.Save();
            //Console.Write(File.ReadAllText(file));
            //var actual = Project.Load(file);
            //Assert.Equal(expected.Name,actual.Name);
        }

        [Test]
        public void WarpBug()
        {
            var image = Image.FromFile("H:\\case.jpeg");
            var parma = SerializationExtensions.XmlDeserialize<TestCase>("H:\\case.xml");

            var result = WarpImageProcessor.Warp(image, new List<FacialFeature>(), parma.First, parma.Second, 10);
            result.Save("H:\\@CAse.jpeg");

        }
    }
}