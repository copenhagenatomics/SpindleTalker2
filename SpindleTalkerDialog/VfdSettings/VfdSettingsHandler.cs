using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpindleTalker2.VfdSettings
{
    public class VfdSettingsHandler
    {
        /// <summary>Quate</summary>
        private string QT { get; } = "\"";
        /// <summary>
        /// Header for file.
        /// </summary>
        /// <param name="seperator">Seperator in file.</param>
        /// <returns>Header</returns>
        public string Header(char seperator) { return $"{QT}ID{QT}{seperator}{QT}Value{QT}{seperator}{QT}DefaultValue{QT}{seperator}{QT}Type{QT}{seperator}{QT}Description{QT}{seperator}{QT}Unit{QT}"; }
        /// <summary>
        /// Write List of lines to file.
        /// </summary>
        /// <param name="fileName">Name of file to save in.</param>
        /// <param name="lines">Lines to save.</param>
        public void SaveCsv(string fileName, char seperator, List<string> lines)
        {
            lines.Insert(0,(Header(seperator)));
            File.WriteAllLines(fileName, lines);
        }
        /// <summary>
        /// Read 
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public List<string> OpenCsv(string fileName, char seperator = ',')
        {
            var lines = File.ReadAllLines(fileName).ToList();
            if (lines[0] == Header(seperator) || lines[0] == Header(seperator).Replace("\"", ""))
            {
                lines.RemoveAt(0);
                return lines;
            }
            return null;
        }
    }
}
