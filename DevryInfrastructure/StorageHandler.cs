using System;
using System.IO;

namespace DevryInfrastructure
{
    public static class StorageHandler
    {
        public static string AppDataPath => 
            Path.Join(AppDomain.CurrentDomain.BaseDirectory, "Data");
        public static string ToolProfilesPath => Path.Join(AppDataPath, "Profiles");
        public static string ScriptsPath => Path.Join(AppDataPath, "Scripts");
        public static string ConfigsPath => Path.Join(AppDataPath, "Configs");
        public static string TemporaryFileStorage => Path.Join(AppDataPath, "Reviewing");
        public static string TestData = Path.Join(AppDataPath, "Testing");

        public static void InitializeFolderStructure()
        {
            if (!Directory.Exists(ToolProfilesPath))
                Directory.CreateDirectory(ToolProfilesPath);

            if (!Directory.Exists(ScriptsPath))
                Directory.CreateDirectory(ScriptsPath);

            if (!Directory.Exists(ConfigsPath))
                Directory.CreateDirectory(ConfigsPath);

            if (!Directory.Exists(TestData))
                Directory.CreateDirectory(TestData);
            
            if (!Directory.Exists(TemporaryFileStorage))
                Directory.CreateDirectory(TemporaryFileStorage);
        }
    }
}