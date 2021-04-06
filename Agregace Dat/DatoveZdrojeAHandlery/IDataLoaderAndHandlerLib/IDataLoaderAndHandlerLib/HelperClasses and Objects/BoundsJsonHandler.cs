using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
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
