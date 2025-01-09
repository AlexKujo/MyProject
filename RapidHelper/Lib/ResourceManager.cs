using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RapidHelper.Lib
{
    class ResourceManager
    {
        private static string[] s_resourceFiles =
            {
            "Supplies.json",
            "Threads.json",
            "Tools.json"
            };

        public static void InitializeResources()
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string resourcesPath = Path.Combine(baseDirectory, "Resources");

            // Проверяем наличие директории ресурсов, и создаем её, если она не существует
            EnsureDirectoryExists(resourcesPath);
            //SetWritable(re);
            
            foreach (string fileName in s_resourceFiles)
            {
                string sourcePath = Path.Combine(baseDirectory, "EmbeddedResources", fileName); // Путь к файлу в проекте
                string destPath = Path.Combine(resourcesPath, fileName); // Путь назначения

                CopyFile(sourcePath, destPath);
            }
        }

        private static void EnsureDirectoryExists(string path)
        {
            if (Directory.Exists(path) == false)
                Directory.CreateDirectory(path);
        }

        //private static void SetWritable(string path) => File.SetAttributes(path, File.GetAttributes(path) & ~FileAttributes.ReadOnly);

        private static void CopyFile(string sourcePath, string destPath)
        {
            // Копируем файл, если он еще не существует
            if (File.Exists(destPath) == false)
            {
                File.Copy(sourcePath, destPath);
                //SetWritable(destPath);
            }
        }
    }
}
