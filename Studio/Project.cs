namespace Studio
{
    using System.IO;
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

        public void Import(string imageFile)
        {
            if (System.IO.File.Exists(imageFile))
            {
                File = imageFile;
                Storage.SaveData(StorageItem.LastLoadedImage, imageFile);
                return;
            }

            throw new FileNotFoundException("The selected file does not exist.");
        }

        public static Project Create(string file)
        {
            var project = new Project("", "");
            project.Import(file);
            return project;
        }
    }
}