namespace Studio
{
    using System.IO;

    public class Storage
    {
        private const string DataDirectory = "data";

        public static void SaveData(StorageItem key, string value)
        {
            Directory.CreateDirectory(DataDirectory);
            var path = GetItemPath(key);
            File.WriteAllText(path, value);
        }

        private static string GetItemPath(StorageItem key)
        {
            var path = Path.Combine(DataDirectory, key.ToString());
            return path;
        }

        public static string LoadData(StorageItem item)
        {
            var path = GetItemPath(item);
            return File.Exists(path) ? File.ReadAllText(path) : null;
        }
    }

    public enum StorageItem
    {
        LastLoadedImage,
    }
}