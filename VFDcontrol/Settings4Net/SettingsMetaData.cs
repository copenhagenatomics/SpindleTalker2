using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFDcontrol.Settings4Net
{
    /// <summary>
    /// Class containing meta data of Settings item
    /// </summary>
    public class SettingsMetaData
    {
        /// <summary>
        /// Settings item description
        /// </summary>
        public string Description = String.Empty;

        /// <summary>
        /// Settings item category
        /// </summary>
        public string Category = String.Empty;

        /// <summary>
        /// Settings item tag - could be used freely to identify object
        /// </summary>
        public string Tag = String.Empty;

        /// <summary>
        /// Settings item visibility - shows/hides it in PropertyGrid control
        /// </summary>
        public bool? Visible = null;

        /// <summary>
        /// SettingsMetaData Contructor
        /// </summary>
        /// <param name="Description">Settings item description</param>
        /// <param name="Category">Settings item category</param>
        /// <param name="Tag">Settings item tag</param>
        /// <param name="Visible">Settings item visibility</param>
        public SettingsMetaData(string Description, string Category, string Tag, bool? Visible)
        {
            this.Description = Description ?? String.Empty;
            this.Category = Category ?? String.Empty;
            this.Tag = Tag ?? String.Empty;
            this.Visible = Visible;
        }

        /// <summary>
        /// SettingsMetaData Constructor (creates default empty values)
        /// </summary>
        public SettingsMetaData()
        {
        }
    }
}
