namespace Studio
{
    using System.Runtime.Serialization;

    [DataContract]
    public class Project
    {
        public string File { get; private set; }

        [DataMember]
        public string Name { get; private set; }

        public Project(string file, string name)
        {
            File = file;
            Name = name;
        }

        public void Save()
        {
            this.SerializeToFile(File);
        }

        public static Project Load(string file)
        {
            var instance = SerializationExtensions.Deserialize<Project>(file);
            instance.File = file;
            return instance;
        }
    }
}