using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

namespace Utilities
{
    public static class FishInfoReader
    {
        private const int COLUMNS = 14;
        private const char SPLITTER = '\t';

        public const string okFolderName = @"OK";
        public const string fileName = "Fish Info - Sheet1.tsv";
        public static string docPath => Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        public static string fullPath => docPath + @"\" + okFolderName + @"\" + fileName;
        public static bool fileExists => File.Exists(fullPath);

        public static List<RawInfoHolder> Read()
        {
            List<RawInfoHolder> infos = new List<RawInfoHolder>();
            string path = fullPath;
            using(StreamReader reader = new StreamReader(path))
            {
                for (int i = 0; !reader.EndOfStream; i++)
                {
                    string line = reader.ReadLine();
                    if (i == 0) continue;
                    string[] values = line.Split(SPLITTER);

                    RawInfoHolder newInfo = new RawInfoHolder(
                        i,
                        values[0],
                        values[1],
                        values[2],
                        values[3],
                        values[4],
                        values[5],
                        values[6],
                        values[7],
                        values[8],
                        values[9],
                        values[10],
                        values[11],
                        values[12],
                        values[13]
                    );

                    infos.Add(newInfo);
                }
            }

            return infos;
        }

        public static void OpenExplorer()
        {
            System.Diagnostics.Process.Start("explorer.exe", docPath + @"\" + okFolderName);
        }
    }
}