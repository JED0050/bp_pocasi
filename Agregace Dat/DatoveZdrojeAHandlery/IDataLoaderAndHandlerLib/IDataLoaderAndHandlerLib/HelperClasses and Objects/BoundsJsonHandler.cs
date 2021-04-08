using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace IDataLoaderAndHandlerLib.HandlersAndObjects
{
    public static class BoundsJsonHandler
    {
        public static string fileName = "agrConfig.json";

        public static Bounds LoadBoundsFromJsonFile()
        {
            string jsonContent = LoadBoundsAsString();

            Bounds bounds = JsonConvert.DeserializeObject<Bounds>(jsonContent);

            return bounds;
        }

        public static string LoadBoundsAsString()
        {
            string filePath = GetPathToDataDirectory(fileName);

            if (!File.Exists(filePath))
            {
                string dataDir = Environment.CurrentDirectory + @"\Data\";

                if (!Directory.Exists(dataDir))
                {
                    Directory.CreateDirectory(dataDir);
                }

                Assembly assembly = Assembly.GetExecutingAssembly();
                string scaleAssembylName = "IDataLoaderAndHandlerLib.Resources.agrConfig.json";

                using (var stream = assembly.GetManifestResourceStream(scaleAssembylName))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        File.WriteAllText(filePath, reader.ReadToEnd());
                    }
                }
            }

            string jsonContent = "";

            using (StreamReader r = new StreamReader(filePath))
            {
                jsonContent = r.ReadToEnd();
            }

            return jsonContent;
        }

        private static string GetPathToDataDirectory(string fileName)
        {
            string workingDirectory = Environment.CurrentDirectory;
            return workingDirectory + @"\Data\" + fileName;
        }
    }
}
