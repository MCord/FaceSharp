namespace Test
{
    using System;
    using System.IO;
    using Studio;
    using Xunit;

    public class ProjectTests
    {
        [Fact]
        public void ShouldSaveProjectObjectToXml()
        {
            const string file = "file";
            var expected = new Project(file, "TestProject");
            

            expected.Save();
            Console.Write(File.ReadAllText(file));
            var actual = Project.Load(file);
            Assert.Equal(expected.Name,actual.Name);
        }
    }
}