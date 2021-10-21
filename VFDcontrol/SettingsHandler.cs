using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace VfdControl
{
    public class SettingsHandler
    {
        /// <summary>
        /// Turn lines from settings file into <see cref="RegisterValue"/>
        /// </summary>
        /// <param name="lines">Lines to convert.</param>
        /// <param name="seperator">Seperator in lines.</param>
        /// <returns>List of <see cref="RegisterValue"/></returns>
        public List<RegisterValue> Convert(List<string> lines, char seperator)
        {
            var result = new List<RegisterValue>();
            foreach (var line in lines)
            {
                var row = line.Split(new char[] { seperator }, StringSplitOptions.RemoveEmptyEntries).ToList();
                if (row.Count() != 6)
                {
                    Console.WriteLine("Invalid column count in row:");
                    return null;
                }

                var item = new RegisterValue(int.Parse(row[0]));
                item.Value = row[1];
                if (item.Type != row[3]) Console.WriteLine($"ID: {item.ID} has wrong type: {item.Type} <> {row[3]}");
                if (item.Unit != row[5]) Console.WriteLine($"ID: {item.ID} has wrong type: {item.Unit} <> {row[5]}");
                result.Add(item);
            }
            return result;
        }
    }
}
