using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace VFDcontrol.Settings4Net
{
    /// <summary>
    /// Class for handling and manipulating Settings items
    /// </summary>
    public class Setting4Net
    {
        /// <summary>
        /// Collection of Settings Items
        /// </summary>
        public SettingsHashtable Settings = new SettingsHashtable();

        /// <summary>
        /// Gets or sets an option for tracking date/time of last settings item modification
        /// </summary>
        public bool TrackModificationTimes { get; set; }

        /// <summary>
        /// Gets or sets  an option to perform checksums on settings file to prevent manual editing of the file
        /// </summary>
        public bool PreventManualModifications { get; set; }

        /// <summary>
        /// Indicates if Settings were loaded from the file
        /// </summary>
        public bool IsLoaded
        {
            get;
            private set;
        }

        /// <summary>
        /// Indicates if Settings file was modified manually
        /// </summary>
        public bool WasModifiedManually
        {
            get;
            private set;
        }

        /// <summary>
        /// Checksum of the loaded file
        /// </summary>
        private string FileChecksum = "";


        #region InternalClassProperties
        private string _xml_version = "1.0";
        private string _xml_encoding = "utf-8";
        private string _magic_salt = "RG_Setting4Net";
        #endregion


        /// <summary>
        /// Opens and reads settings from specified file
        /// </summary>
        /// <param name="filename">Settings file</param>
        /// <returns>Status of the operation</returns>
        public bool Open(string filename)
        {
            return Open(filename, PreventManualModifications, false); ;
        }


        /// <summary>
        /// Opens and reads settings from specified file
        /// </summary>
        /// <param name="filename">Settings file</param>
        /// <param name="PreventManualModifications">If set to true, prevents loading settings if the file was manually modified</param>
        /// <returns>Status of the operation</returns>
        public bool Open(string filename, bool PreventManualModifications)
        {
            return Open(filename, PreventManualModifications, false);
        }

        /// <summary>
        /// Opens and reads settings from specified file
        /// </summary>
        /// <param name="filename">Settings file</param>
        /// <param name="PreventManualModifications">If true, prevents loading settings if the file was manually modified</param>
        /// <param name="AppendSettings">If true, appends settings from specified file to already existing ones instead of overwriting</param>
        /// <returns></returns>
        public bool Open(string filename, bool PreventManualModifications, bool AppendSettings)
        {
            IsLoaded = false;

            bool res = false;

            if (!AppendSettings)
                Settings.Clear();

            try
            {
                res = ParseXmlFile(filename);
            }
            catch (Exception)
            {
                return false;
            }

            if (res && PreventManualModifications && (CalculateChecksumValue() != FileChecksum))
            {
                Settings.Clear();
                return false;
            }

            this.PreventManualModifications = PreventManualModifications;
            IsLoaded = res;
            return res;
        }

        /// <summary>
        /// Saves all Settings to specified file
        /// </summary>
        /// <param name="filename">Output file</param>
        public void Save(string filename)
        {
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.AppendChild(xmldoc.CreateXmlDeclaration(_xml_version, _xml_encoding, null));

            XmlElement xmlRoot = xmldoc.CreateElement("settings");
            xmldoc.AppendChild(xmlRoot);

            string iname, ivalue, itype;
            bool? ireadonly;
            string imin = null, imax = null, idefault = null;

            foreach (string key in Settings.Keys)
            {
                object setting = Settings[key, true];
                SettingsMetaData imeta;
                if (setting is SettingsItem<int?>)
                {
                    itype = "int";
                    iname = (setting as SettingsItem<int?>).Name;
                    ireadonly = (setting as SettingsItem<int?>).Readonly;
                    ivalue = (setting as SettingsItem<int?>).Value.ToString();
                    imin = (setting as SettingsItem<int?>).MinValue == null ? null : (setting as SettingsItem<int?>).MinValue.Value.ToString();
                    imax = (setting as SettingsItem<int?>).MaxValue == null ? null : (setting as SettingsItem<int?>).MaxValue.Value.ToString();
                    idefault = (setting as SettingsItem<int?>).Default == null ? null : (setting as SettingsItem<int?>).Default.Value.ToString();
                    imeta = (setting as SettingsItem<int?>).MetaData;
                }
                else if (setting is SettingsItem<bool?>)
                {
                    itype = "bool";
                    iname = (setting as SettingsItem<bool?>).Name;
                    ireadonly = (setting as SettingsItem<bool?>).Readonly;
                    ivalue = (setting as SettingsItem<bool?>).Value.ToString();
                    idefault = (setting as SettingsItem<bool?>).Default == null ? null : (setting as SettingsItem<bool?>).Default.ToString();
                    imeta = (setting as SettingsItem<bool?>).MetaData;
                }
                else if (setting is SettingsItem<double?>)
                {
                    itype = "double";
                    iname = (setting as SettingsItem<double?>).Name;
                    ireadonly = (setting as SettingsItem<double?>).Readonly;
                    ivalue = (setting as SettingsItem<double?>).Value.ToString();
                    imin = (setting as SettingsItem<double?>).MinValue == null ? null : (setting as SettingsItem<double?>).MinValue.Value.ToString();
                    imax = (setting as SettingsItem<double?>).MaxValue == null ? null : (setting as SettingsItem<double?>).MaxValue.Value.ToString();
                    idefault = (setting as SettingsItem<double?>).Default == null ? null : (setting as SettingsItem<double?>).Default.Value.ToString();
                    imeta = (setting as SettingsItem<double?>).MetaData;
                }
                else if (setting is SettingsItem<Color?>)
                {
                    itype = "color";
                    iname = (setting as SettingsItem<Color?>).Name;
                    ireadonly = (setting as SettingsItem<Color?>).Readonly;
                    ivalue = TypeDescriptor.GetConverter(typeof(Color)).ConvertToString((setting as SettingsItem<Color?>).Value);
                    idefault = TypeDescriptor.GetConverter(typeof(Color)).ConvertToString((setting as SettingsItem<Color?>).Default);
                    imeta = (setting as SettingsItem<Color?>).MetaData;
                }
                else if (setting is SettingsItem<DateTime?>)
                {
                    itype = "datetime";
                    iname = (setting as SettingsItem<DateTime?>).Name;
                    ireadonly = (setting as SettingsItem<DateTime?>).Readonly;
                    ivalue = (setting as SettingsItem<DateTime?>).Value.ToString();
                    imin = (setting as SettingsItem<DateTime?>).MinValue == null ? null : (setting as SettingsItem<DateTime?>).MinValue.Value.ToString();
                    imax = (setting as SettingsItem<DateTime?>).MaxValue == null ? null : (setting as SettingsItem<DateTime?>).MaxValue.Value.ToString();
                    idefault = (setting as SettingsItem<DateTime?>).Default == null ? null : (setting as SettingsItem<DateTime?>).Default.Value.ToString();
                    imeta = (setting as SettingsItem<DateTime?>).MetaData;
                }
                else if (setting is SettingsItem<Font>)
                {
                    itype = "font";
                    iname = (setting as SettingsItem<Font>).Name;
                    ireadonly = (setting as SettingsItem<Font>).Readonly;
                    ivalue = TypeDescriptor.GetConverter(typeof(Font)).ConvertToString((setting as SettingsItem<Font>).Value);
                    idefault = TypeDescriptor.GetConverter(typeof(Font)).ConvertToString((setting as SettingsItem<Font>).Default);
                    imeta = (setting as SettingsItem<Font>).MetaData;
                }
                else // string
                {
                    itype = "string";
                    iname = (setting as SettingsItem<string>).Name;
                    ireadonly = (setting as SettingsItem<string>).Readonly;
                    ivalue = (setting as SettingsItem<string>).Value;
                    idefault = (setting as SettingsItem<string>).Default == null ? null : (setting as SettingsItem<string>).Default;
                    imeta = (setting as SettingsItem<string>).MetaData;
                }

                XmlElement element = xmldoc.CreateElement("item");
                element.InnerText = ivalue;
                element.SetAttribute("name", iname);
                element.SetAttribute("type", itype);
                if (ireadonly != null)
                    element.SetAttribute("readonly", ireadonly.Value.ToString());
                if (imin != null)
                    element.SetAttribute("min", imin);
                if (imax != null)
                    element.SetAttribute("max", imax);
                if (idefault != null)
                    element.SetAttribute("default", idefault);
                if (TrackModificationTimes)
                    element.SetAttribute("modified", DateTime.Now.ToString());
                if (imeta != null)
                {
                    if (imeta.Description != null && imeta.Description != String.Empty)
                        element.SetAttribute("description", imeta.Description);
                    if (imeta.Category != null && imeta.Category != String.Empty)
                        element.SetAttribute("category", imeta.Category);
                    if (imeta.Tag != null && imeta.Tag != String.Empty)
                        element.SetAttribute("tag", imeta.Tag);
                    if (imeta.Visible != null && imeta.Tag != String.Empty)
                        element.SetAttribute("tag", imeta.Tag);
                }
                xmlRoot.AppendChild(element);
            }

            // add checksum element
            XmlElement checksumElement = xmldoc.CreateElement("checksum");
            checksumElement.InnerText = CalculateChecksumValue();
            xmlRoot.AppendChild(checksumElement);

            try
            {
                xmldoc.Save(filename);
            }
            catch (Exception)
            {
                throw new Exception("Couldn't save settings to the specified file");
            }
        }


        /// <summary>
        /// Reads XML-Settings file contents
        /// </summary>
        /// <param name="filename">Input file</param>
        /// <returns>Operation status</returns>
        private bool ParseXmlFile(string filename)
        {
            XmlDocument xmldoc = new XmlDocument();

            try
            {
                xmldoc.Load(filename);
            }
            catch (Exception)
            {
                return false;
            }

            foreach (XmlNode node in xmldoc.GetElementsByTagName("item"))
            {
                ParseANode(node);
            }

            XmlNodeList checksumNode = xmldoc.GetElementsByTagName("checksum");
            if (checksumNode.Count != 1)
                return false;

            FileChecksum = checksumNode[0].InnerText;

            return true;
        }


        /// <summary>
        /// Parses and stores a single Settings item from XmlNode
        /// </summary>
        /// <param name="node">Input XML node</param>
        private void ParseANode(XmlNode node)
        {
            string item_value = node.InnerText;
            string item_name = node.Attributes["name"] == null ? null : node.Attributes["name"].Value;
            if (item_name == null || item_name.Length <= 0)
                throw new Exception("Corrupt XML file");

            string item_type = node.Attributes["type"] == null ? String.Empty : node.Attributes["type"].Value;
            string item_default = node.Attributes["default"] == null ? String.Empty : node.Attributes["default"].Value;
            string item_min = node.Attributes["min"] == null ? String.Empty : node.Attributes["min"].Value;
            string item_max = node.Attributes["max"] == null ? String.Empty : node.Attributes["max"].Value;
            bool? item_readonly = null; bool tmp_b;
            if (node.Attributes["readonly"] != null)
                item_readonly = Boolean.TryParse(node.Attributes["readonly"].Value, out tmp_b) ? tmp_b : (bool?)null;
            DateTime? item_modified = null; DateTime tmp_t;
            if (node.Attributes["modified"] != null)
                item_modified = DateTime.TryParse(node.Attributes["modified"].Value, out tmp_t) ? tmp_t : (DateTime?)null;


            string idesc = node.Attributes["description"] == null ? String.Empty : node.Attributes["description"].Value;
            string icat = node.Attributes["category"] == null ? String.Empty : node.Attributes["category"].Value;
            string itag = node.Attributes["tag"] == null ? String.Empty : node.Attributes["tag"].Value;
            bool? item_visible = null; bool tmp_visible;
            if (node.Attributes["visible"] != null)
                item_visible = Boolean.TryParse(node.Attributes["visible"].Value, out tmp_visible) ? tmp_visible : (bool?)null;

            AddSettingsItem(item_type, item_name, item_value, item_modified, item_readonly, item_min, item_max, item_default, new SettingsMetaData(idesc, icat, itag, item_visible));
        }


        /// <summary>
        /// Adds a new Settings item
        /// </summary>
        /// <param name="itype">Type</param>
        /// <param name="iname">Name</param>
        /// <param name="ivalue">Value</param>
        /// <param name="imodified">Modified datetiem</param>
        /// <param name="ireadonly">Readonly</param>
        /// <param name="imax">MaxValue</param>
        /// <param name="imin">MinValue</param>
        /// <param name="idefault">Default</param>
        /// <param name="imeta">Meta data</param>
        private void AddSettingsItem(string itype, string iname, string ivalue, DateTime? imodified, bool? ireadonly, string imax, string imin, string idefault, SettingsMetaData imeta)
        {
            if (Settings.ContainsKey(iname))
                return;

            switch (itype)
            {
                case "int":
                case "integer":
                case "number":
                    int t_int;
                    if (!int.TryParse(ivalue, out t_int))
                        throw new Exception("Corrupt XML file");

                    Settings.Add(iname, new SettingsItem<int?>(
                        iname, int.Parse(ivalue), imodified, ireadonly,
                        (int.TryParse(imin, out t_int) == true) ? t_int : (int?)null,
                        (int.TryParse(imax, out t_int) == true) ? t_int : (int?)null,
                        (int.TryParse(idefault, out t_int) == true) ? t_int : (int?)null, imeta));
                    break;

                case "bool":
                case "boolean":
                case "logic":
                    bool t_bool;
                    if (!bool.TryParse(ivalue, out t_bool))
                        throw new Exception("Corrupt XML file");

                    Settings.Add(iname, new SettingsItem<bool?>(
                        iname, bool.Parse(ivalue), imodified, ireadonly,
                        (bool.TryParse(imin, out t_bool) == true) ? t_bool : (bool?)null,
                        (bool.TryParse(imax, out t_bool) == true) ? t_bool : (bool?)null,
                        (bool.TryParse(idefault, out t_bool) == true) ? t_bool : (bool?)null, imeta));
                    break;

                case "double":
                case "float":
                case "real":
                    double t_double;
                    if (!double.TryParse(ivalue, out t_double))
                        throw new Exception("Corrupt XML file");

                    Settings.Add(iname, new SettingsItem<double?>(
                        iname, double.Parse(ivalue), imodified, ireadonly,
                        (double.TryParse(imin, out t_double) == true) ? t_double : (double?)null,
                        (double.TryParse(imax, out t_double) == true) ? t_double : (double?)null,
                        (double.TryParse(idefault, out t_double) == true) ? t_double : (double?)null, imeta));
                    break;

                case "color":
                case "rgb":
                case "brush":
                    Color color;
                    try
                    {
                        color = (Color)TypeDescriptor.GetConverter(typeof(Color)).ConvertFromString(ivalue);
                    }
                    catch (Exception)
                    {
                        throw new Exception("Corrupt XML file");
                    }

                    Color? color_default = null;
                    try
                    {
                        color_default = (Color)TypeDescriptor.GetConverter(typeof(Color)).ConvertFromString(idefault);
                    }
                    catch (Exception)
                    {
                    }

                    Settings.Add(iname, new SettingsItem<Color?>(
                        iname, color, imodified, ireadonly,
                        null,
                        null,
                        color_default == null ? (Color?)null : color_default.Value, imeta));
                    break;

                case "datetime":
                case "date":
                case "time":
                    DateTime t_datetime;
                    if (!DateTime.TryParse(ivalue, out t_datetime))
                        throw new Exception("Corrupt XML file");

                    Settings.Add(iname, new SettingsItem<DateTime?>(
                        iname, DateTime.Parse(ivalue), imodified, ireadonly,
                        (DateTime.TryParse(imin, out t_datetime) == true) ? t_datetime : (DateTime?)null,
                        (DateTime.TryParse(imax, out t_datetime) == true) ? t_datetime : (DateTime?)null,
                        (DateTime.TryParse(idefault, out t_datetime) == true) ? t_datetime : (DateTime?)null, imeta));
                    break;

                case "font":
                case "style":
                case "fontstyle":
                    Font font;
                    try
                    {
                        font = (Font)TypeDescriptor.GetConverter(typeof(Font)).ConvertFromString(ivalue);
                    }
                    catch (Exception)
                    {
                        throw new Exception("Corrupt XML file");
                    }

                    Font font_default = null;
                    try
                    {
                        font_default = (Font)TypeDescriptor.GetConverter(typeof(Font)).ConvertFromString(idefault);
                    }
                    catch (Exception)
                    {
                    }

                    Settings.Add(iname, new SettingsItem<Font>(
                        iname, font, imodified, ireadonly,
                        null, null, font_default, imeta));
                    break;

                default: // string
                    Settings.Add(iname, new SettingsItem<string>(
                        iname, ivalue, imodified, ireadonly,
                        imin, imax, idefault, imeta));
                    break;
            }
        }

        /// <summary>
        /// Adds new Settings item
        /// </summary>
        /// <typeparam name="T">Settings item type</typeparam>
        /// <param name="Name">Settings item name</param>
        /// <param name="Value">Settings item value</param>
        /// <param name="Description">Settings item description</param>
        /// <param name="Category">Settings item category</param>
        /// <param name="Tag">Settings item tag</param>
        /// <param name="Readonly">Settings item visibility</param>
        public void Add<T>(string Name, T Value, string Description, string Category, string Tag, bool Readonly)
        {
            if (Name == null || Value == null)
                throw new NullReferenceException();

            Settings[Name] = Value;
            SetReadonlyProperty(Name, Readonly);
            SetMetaData(Name, Description, Category, Tag, null);
        }

        /// <summary>
        /// Adds new Settings item
        /// </summary>
        /// <typeparam name="T">Settings item type</typeparam>
        /// <param name="Name">Settings item name</param>
        /// <param name="Value">Settings item value</param>
        /// <param name="Description">Settings item description</param>
        /// <param name="Category">Settings item category</param>
        /// <param name="Tag">Settings item tag</param>
        public void Add<T>(string Name, T Value, string Description, string Category, string Tag)
        {
            if (Name == null || Value == null)
                throw new NullReferenceException();

            Settings[Name] = Value;
            SetMetaData(Name, Description, Category, Tag, null);
        }

        /// <summary>
        /// Adds new Settings item
        /// </summary>
        /// <typeparam name="T">Settings item type</typeparam>
        /// <param name="Name">Settings item name</param>
        /// <param name="Value">Settings item value</param>
        /// <param name="Description">Settings item description</param>
        public void Add<T>(string Name, T Value, string Description)
        {
            if (Name == null || Value == null)
                throw new NullReferenceException();

            Settings[Name] = Value;
            SetMetaData(Name, Description, null, null, null);
        }


        /// <summary>
        /// Sets default value of a Settings item
        /// </summary>
        /// <param name="Name">Settings item name</param>
        /// <param name="DefaultValue">Default value</param>
        public void SetDefaultItemValue(string Name, object DefaultValue)
        {
            if (Settings[Name, true] == null)
                throw new NullReferenceException();

            if (Settings[Name, true] is SettingsItem<int?>)
                (Settings[Name, true] as SettingsItem<int?>).Default = (int?)DefaultValue;
            else if (Settings[Name, true] is SettingsItem<bool?>)
                (Settings[Name, true] as SettingsItem<bool?>).Default = (bool?)DefaultValue;
            else if (Settings[Name, true] is SettingsItem<double?>)
                (Settings[Name, true] as SettingsItem<double?>).Default = (double?)DefaultValue;
            else if (Settings[Name, true] is SettingsItem<Color?>)
                (Settings[Name, true] as SettingsItem<Color?>).Default = (Color?)DefaultValue;
            else if (Settings[Name, true] is SettingsItem<DateTime?>)
                (Settings[Name, true] as SettingsItem<DateTime?>).Default = (DateTime?)DefaultValue;
            else if (Settings[Name, true] is SettingsItem<Font>)
                (Settings[Name, true] as SettingsItem<Font>).Default = (Font)DefaultValue;
            else // string
                (Settings[Name, true] as SettingsItem<string>).Default = (string)DefaultValue;
        }

        /// <summary>
        /// Sets minimum allowed value of a Settings item
        /// </summary>
        /// <param name="Name">Settings item name</param>
        /// <param name="MinValue">Minimum value</param>
        public void SetMinItemValue(string Name, object MinValue)
        {
            if (Settings[Name, true] == null)
                throw new NullReferenceException();

            if (Settings[Name, true] is SettingsItem<int?>)
                (Settings[Name, true] as SettingsItem<int?>).MinValue = (int?)MinValue;
            else if (Settings[Name, true] is SettingsItem<double?>)
                (Settings[Name, true] as SettingsItem<double?>).MinValue = (double?)MinValue;
            else if (Settings[Name, true] is SettingsItem<DateTime?>)
                (Settings[Name, true] as SettingsItem<DateTime?>).MinValue = (DateTime?)MinValue;
        }


        /// <summary>
        /// Sets maximum allowed value of a Settings item
        /// </summary>
        /// <param name="Name">Settings item name</param>
        /// <param name="MaxValue">Maximum value</param>
        public void SetMaxItemValue(string Name, object MaxValue)
        {
            if (Settings[Name, true] == null)
                throw new NullReferenceException();

            if (Settings[Name, true] is SettingsItem<int?>)
                (Settings[Name, true] as SettingsItem<int?>).MaxValue = (int?)MaxValue;
            else if (Settings[Name, true] is SettingsItem<double?>)
                (Settings[Name, true] as SettingsItem<double?>).MaxValue = (double?)MaxValue;
            else if (Settings[Name, true] is SettingsItem<DateTime?>)
                (Settings[Name, true] as SettingsItem<DateTime?>).MaxValue = (DateTime?)MaxValue;
        }


        /// <summary>
        /// Sets readonly propery of Settings item, enabling/disabling its modifications
        /// </summary>
        /// <param name="Name">Settings item name</param>
        /// <param name="Readonly">true - allows changes; false - prevents changes untill readonly value is changed</param>
        public void SetReadonlyProperty(string Name, bool? Readonly)
        {
            if (Settings[Name, true] == null)
                throw new NullReferenceException();

            if (Settings[Name, true] is SettingsItem<int?>)
                (Settings[Name, true] as SettingsItem<int?>).Readonly = Readonly;
            else if (Settings[Name, true] is SettingsItem<bool?>)
                (Settings[Name, true] as SettingsItem<bool?>).Readonly = Readonly;
            else if (Settings[Name, true] is SettingsItem<double?>)
                (Settings[Name, true] as SettingsItem<double?>).Readonly = Readonly;
            else if (Settings[Name, true] is SettingsItem<Color?>)
                (Settings[Name, true] as SettingsItem<Color?>).Readonly = Readonly;
            else if (Settings[Name, true] is SettingsItem<DateTime?>)
                (Settings[Name, true] as SettingsItem<DateTime?>).Readonly = Readonly;
            else if (Settings[Name, true] is SettingsItem<Font>)
                (Settings[Name, true] as SettingsItem<Font>).Readonly = Readonly;
            else // string
                (Settings[Name, true] as SettingsItem<string>).Readonly = Readonly;
        }

        /// <summary>
        /// Sets metadata of specified settings item
        /// </summary>
        /// <param name="Name">Settings item name</param>
        /// <param name="Description">Description (if null = do not set)</param>
        /// <param name="Category">Category (if null = do not set)</param>
        /// <param name="Tag">Tag (if null = do not set)</param>
        /// <param name="Visible">Visibility</param>
        private void SetMetaData(string Name, string Description, string Category, string Tag, bool? Visible)
        {
            if (Settings[Name, true] == null)
                throw new NullReferenceException();

            if (Settings[Name, true] is SettingsItem<int?>)
            {
                if (Description != null)
                    (Settings[Name, true] as SettingsItem<int?>).MetaData.Description = Description;
                if (Category != null)
                    (Settings[Name, true] as SettingsItem<int?>).MetaData.Category = Category;
                if (Tag != null)
                    (Settings[Name, true] as SettingsItem<int?>).MetaData.Tag = Tag;
                if (Visible != null)
                    (Settings[Name, true] as SettingsItem<int?>).MetaData.Visible = Visible.Value;
            }
            else if (Settings[Name, true] is SettingsItem<bool?>)
            {
                if (Description != null)
                    (Settings[Name, true] as SettingsItem<bool?>).MetaData.Description = Description;
                if (Category != null)
                    (Settings[Name, true] as SettingsItem<bool?>).MetaData.Category = Category;
                if (Tag != null)
                    (Settings[Name, true] as SettingsItem<bool?>).MetaData.Tag = Tag;
                if (Visible != null)
                    (Settings[Name, true] as SettingsItem<bool?>).MetaData.Visible = Visible.Value;
            }
            else if (Settings[Name, true] is SettingsItem<double?>)
            {
                if (Description != null)
                    (Settings[Name, true] as SettingsItem<double?>).MetaData.Description = Description;
                if (Category != null)
                    (Settings[Name, true] as SettingsItem<double?>).MetaData.Category = Category;
                if (Tag != null)
                    (Settings[Name, true] as SettingsItem<double?>).MetaData.Tag = Tag;
                if (Visible != null)
                    (Settings[Name, true] as SettingsItem<double?>).MetaData.Visible = Visible.Value;
            }
            else if (Settings[Name, true] is SettingsItem<Color?>)
            {
                if (Description != null)
                    (Settings[Name, true] as SettingsItem<Color?>).MetaData.Description = Description;
                if (Category != null)
                    (Settings[Name, true] as SettingsItem<Color?>).MetaData.Category = Category;
                if (Tag != null)
                    (Settings[Name, true] as SettingsItem<Color?>).MetaData.Tag = Tag;
                if (Visible != null)
                    (Settings[Name, true] as SettingsItem<Color?>).MetaData.Visible = Visible.Value;
            }
            else if (Settings[Name, true] is SettingsItem<DateTime?>)
            {
                if (Description != null)
                    (Settings[Name, true] as SettingsItem<DateTime?>).MetaData.Description = Description;
                if (Category != null)
                    (Settings[Name, true] as SettingsItem<DateTime?>).MetaData.Category = Category;
                if (Tag != null)
                    (Settings[Name, true] as SettingsItem<DateTime?>).MetaData.Tag = Tag;
                if (Visible != null)
                    (Settings[Name, true] as SettingsItem<DateTime?>).MetaData.Visible = Visible.Value;
            }
            else if (Settings[Name, true] is SettingsItem<Font>)
            {
                if (Description != null)
                    (Settings[Name, true] as SettingsItem<Font>).MetaData.Description = Description;
                if (Category != null)
                    (Settings[Name, true] as SettingsItem<Font>).MetaData.Category = Category;
                if (Tag != null)
                    (Settings[Name, true] as SettingsItem<Font>).MetaData.Tag = Tag;
                if (Visible != null)
                    (Settings[Name, true] as SettingsItem<Font>).MetaData.Visible = Visible.Value;
            }
            else // string
            {
                if (Description != null)
                    (Settings[Name, true] as SettingsItem<string>).MetaData.Description = Description;
                if (Category != null)
                    (Settings[Name, true] as SettingsItem<string>).MetaData.Category = Category;
                if (Tag != null)
                    (Settings[Name, true] as SettingsItem<string>).MetaData.Tag = Tag;
                if (Visible != null)
                    (Settings[Name, true] as SettingsItem<string>).MetaData.Visible = Visible.Value;
            }
        }

        /// <summary>
        /// Sets Settings item description
        /// </summary>
        /// <param name="Name">Settings item name</param>
        /// <param name="Description">Description</param>
        public void SetItemDescription(string Name, string Description)
        {
            SetMetaData(Name, Description, null, null, null);
        }

        /// <summary>
        /// Sets Settings item category
        /// </summary>
        /// <param name="Name">Settings item name</param>
        /// <param name="Category">Category</param>
        public void SetItemCategory(string Name, string Category)
        {
            SetMetaData(Name, null, Category, null, null);
        }

        /// <summary>
        /// Sets Settings item freely used tag
        /// </summary>
        /// <param name="Name">Settings item name</param>
        /// <param name="Tag">Tag value</param>
        public void SetItemTag(string Name, string Tag)
        {
            SetMetaData(Name, null, null, Tag, null);
        }

        /// <summary>
        /// Sets Settings item visibility
        /// </summary>
        /// <param name="Name">Settings item name</param>
        /// <param name="Visible">Visibility</param>
        public void SetItemVisibility(string Name, bool Visible)
        {
            SetMetaData(Name, null, null, null, Visible);
        }

        /// <summary>
        /// Gets Settings item meta data object
        /// </summary>
        /// <param name="Name">Settings item name</param>
        /// <returns>SettingsMetaData object containing all meta data information</returns>
        public SettingsMetaData GetMetaData(string Name)
        {
            if (Settings[Name, true] == null)
                throw new NullReferenceException();

            if (Settings[Name, true] is SettingsItem<int?>)
                return (Settings[Name, true] as SettingsItem<int?>).MetaData;
            else if (Settings[Name, true] is SettingsItem<bool?>)
                return (Settings[Name, true] as SettingsItem<bool?>).MetaData;
            else if (Settings[Name, true] is SettingsItem<double?>)
                return (Settings[Name, true] as SettingsItem<double?>).MetaData;
            else if (Settings[Name, true] is SettingsItem<Color?>)
                return (Settings[Name, true] as SettingsItem<Color?>).MetaData;
            else if (Settings[Name, true] is SettingsItem<DateTime?>)
                return (Settings[Name, true] as SettingsItem<DateTime?>).MetaData;
            else if (Settings[Name, true] is SettingsItem<Font>)
                return (Settings[Name, true] as SettingsItem<Font>).MetaData;
            // string
            return (Settings[Name, true] as SettingsItem<string>).MetaData;
        }

        /// <summary>
        /// Gets Settings item description
        /// </summary>
        /// <param name="Name">Settings item name</param>
        /// <returns>Description</returns>
        public string GetItemDescription(string Name)
        {
            return GetMetaData(Name).Description;
        }

        /// <summary>
        /// Gets Settings item category
        /// </summary>
        /// <param name="Name">Settings item name</param>
        /// <returns>Category</returns>
        public string GetItemCategory(string Name)
        {
            return GetMetaData(Name).Category;
        }

        /// <summary>
        /// Gets Settings item tag
        /// </summary>
        /// <param name="Name">Settings item name</param>
        /// <returns>Tag value</returns>
        public string GetItemTag(string Name)
        {
            return GetMetaData(Name).Tag;
        }

        /// <summary>
        /// Gets Settings item visibility
        /// </summary>
        /// <param name="Name">Settings item name</param>
        /// <returns>Visible value</returns>
        public bool GetItemVisibility(string Name)
        {
            return (GetMetaData(Name).Visible == null) ? true : GetMetaData(Name).Visible.Value;
        }

        /// <summary>
        /// Gets default value of a Settings Item
        /// </summary>
        /// <param name="Name">Settings item name</param>
        /// <returns>Default value</returns>
        public object GetDefaultItemValue(string Name)
        {
            if (Settings[Name, true] == null)
                throw new NullReferenceException();

            if (Settings[Name, true] is SettingsItem<int?>)
                return (Settings[Name, true] as SettingsItem<int?>).Default;
            else if (Settings[Name, true] is SettingsItem<bool?>)
                return (Settings[Name, true] as SettingsItem<bool?>).Default;
            else if (Settings[Name, true] is SettingsItem<double?>)
                return (Settings[Name, true] as SettingsItem<double?>).Default;
            else if (Settings[Name, true] is SettingsItem<Color?>)
                return (Settings[Name, true] as SettingsItem<Color?>).Default;
            else if (Settings[Name, true] is SettingsItem<DateTime?>)
                return (Settings[Name, true] as SettingsItem<DateTime?>).Default;
            else if (Settings[Name, true] is SettingsItem<Font>)
                return (Settings[Name, true] as SettingsItem<Font>).Default;
            // string
            return (Settings[Name, true] as SettingsItem<string>).Default;
        }


        /// <summary>
        /// Gets Settings item value if it exists, or Default one
        /// </summary>
        /// <param name="Name">Settings item name</param>
        /// <param name="Default">If settings item doesn't exist - return this value</param>
        /// <returns></returns>
        public object GetItemOrDefaultValue(string Name, object Default)
        {
            if (Settings[Name, true] == null)
                return Default;

            return Settings[Name];
        }

        /// <summary>
        /// Gets minimum allowed value of a Settings Item
        /// </summary>
        /// <param name="Name">Settings item name</param>
        /// <returns>Minimum value</returns>
        public object GetMinItemValue(string Name)
        {
            if (Settings[Name, true] == null)
                throw new NullReferenceException();

            if (Settings[Name, true] is SettingsItem<int?>)
                return (Settings[Name, true] as SettingsItem<int?>).MinValue;
            else if (Settings[Name, true] is SettingsItem<double?>)
                return (Settings[Name, true] as SettingsItem<double?>).MinValue;
            else if (Settings[Name, true] is SettingsItem<DateTime?>)
                return (Settings[Name, true] as SettingsItem<DateTime?>).MinValue;

            return null;
        }

        /// <summary>
        /// Gets maximum allowed value of a Settings Item
        /// </summary>
        /// <param name="Name">Settings item name</param>
        /// <returns>Maximum value</returns>
        public object GetMaxItemValue(string Name)
        {
            if (Settings[Name, true] == null)
                throw new NullReferenceException();

            if (Settings[Name, true] is SettingsItem<int?>)
                return (Settings[Name, true] as SettingsItem<int?>).MaxValue;
            else if (Settings[Name, true] is SettingsItem<double?>)
                return (Settings[Name, true] as SettingsItem<double?>).MaxValue;
            else if (Settings[Name, true] is SettingsItem<DateTime?>)
                return (Settings[Name, true] as SettingsItem<DateTime?>).MaxValue;

            return null;
        }

        /// <summary>
        /// Gets readonly property of a Settings item
        /// </summary>
        /// <param name="Name">Settings item name</param>
        /// <returns>Readonly value</returns>
        public bool GetReadonlyProperty(string Name)
        {
            if (Settings[Name, true] == null)
                throw new NullReferenceException();

            bool? result = null;

            if (Settings[Name, true] is SettingsItem<int?>)
                result = (Settings[Name, true] as SettingsItem<int?>).Readonly;
            else if (Settings[Name, true] is SettingsItem<bool?>)
                result = (Settings[Name, true] as SettingsItem<bool?>).Readonly;
            else if (Settings[Name, true] is SettingsItem<double?>)
                result = (Settings[Name, true] as SettingsItem<double?>).Readonly;
            else if (Settings[Name, true] is SettingsItem<Color?>)
                result = (Settings[Name, true] as SettingsItem<Color?>).Readonly;
            else if (Settings[Name, true] is SettingsItem<DateTime?>)
                result = (Settings[Name, true] as SettingsItem<DateTime?>).Readonly;
            else if (Settings[Name, true] is Font)
                result = (Settings[Name, true] as SettingsItem<Font>).Readonly;
            else // string
                result = (Settings[Name, true] as SettingsItem<string>).Readonly;

            if (result != null && result.Value == true)
                return true;
            return false;
        }


        /// <summary>
        /// Resets Settings item value to default or specified one 
        /// </summary>
        /// <param name="Name">Settings item name</param>
        /// <param name="DefaultValue">Default value (used if Settings item doesn't have a specified one)</param>
        public void ResetToDefaultValue(string Name, object DefaultValue)
        {
            if (Settings[Name, true] == null)
                throw new NullReferenceException();

            if (Settings[Name, true] is SettingsItem<int?>)
                (Settings[Name, true] as SettingsItem<int?>).Value = (Settings[Name, true] as SettingsItem<int?>).Default ?? (int?)DefaultValue ?? (Settings[Name, true] as SettingsItem<int?>).Value;
            else if (Settings[Name, true] is SettingsItem<bool?>)
                (Settings[Name, true] as SettingsItem<bool?>).Value = (Settings[Name, true] as SettingsItem<bool?>).Default ?? (bool?)DefaultValue ?? (Settings[Name, true] as SettingsItem<bool?>).Value;
            else if (Settings[Name, true] is SettingsItem<double?>)
                (Settings[Name, true] as SettingsItem<double?>).Value = (Settings[Name, true] as SettingsItem<double?>).Default ?? (double?)DefaultValue ?? (Settings[Name, true] as SettingsItem<double?>).Value;
            else if (Settings[Name, true] is SettingsItem<Color?>)
                (Settings[Name, true] as SettingsItem<Color?>).Value = (Settings[Name, true] as SettingsItem<Color?>).Default ?? (Color?)DefaultValue ?? (Settings[Name, true] as SettingsItem<Color?>).Value;
            else if (Settings[Name, true] is SettingsItem<DateTime?>)
                (Settings[Name, true] as SettingsItem<DateTime?>).Value = (Settings[Name, true] as SettingsItem<DateTime?>).Default ?? (DateTime?)DefaultValue ?? (Settings[Name, true] as SettingsItem<DateTime?>).Value;
            else if (Settings[Name, true] is SettingsItem<Font>)
                (Settings[Name, true] as SettingsItem<Font>).Value = (Settings[Name, true] as SettingsItem<Font>).Default ?? (Font)DefaultValue ?? (Settings[Name, true] as SettingsItem<Font>).Value;
            else // string
                (Settings[Name, true] as SettingsItem<string>).Value = (Settings[Name, true] as SettingsItem<string>).Default ?? (string)DefaultValue ?? (Settings[Name, true] as SettingsItem<string>).Value;
        }


        /// <summary>
        /// If setting file is keeping track of modification dates/times, returns the last DateTime of such event
        /// </summary>
        /// <param name="Name">Settings item name</param>
        /// <returns>DateTime object of the last modification event or an empty one if no such event was recorded</returns>
        public DateTime GetModificationDateTime(string Name)
        {
            if (Settings[Name, true] == null)
                throw new NullReferenceException();

            if (Settings[Name, true] is SettingsItem<int?>)
                return (Settings[Name, true] as SettingsItem<int?>).ModifiedDate ?? new DateTime();
            else if (Settings[Name, true] is SettingsItem<bool?>)
                return (Settings[Name, true] as SettingsItem<bool?>).ModifiedDate ?? new DateTime();
            else if (Settings[Name, true] is SettingsItem<double?>)
                return (Settings[Name, true] as SettingsItem<double?>).ModifiedDate ?? new DateTime();
            else if (Settings[Name, true] is SettingsItem<Color?>)
                return (Settings[Name, true] as SettingsItem<Color?>).ModifiedDate ?? new DateTime();
            else if (Settings[Name, true] is SettingsItem<DateTime?>)
                return (Settings[Name, true] as SettingsItem<DateTime?>).ModifiedDate ?? new DateTime();
            else if (Settings[Name, true] is SettingsItem<Font>)
                return (Settings[Name, true] as SettingsItem<Font>).ModifiedDate ?? new DateTime();
            else // string
                return (Settings[Name, true] as SettingsItem<string>).ModifiedDate ?? new DateTime();
        }

        /// <summary>
        /// Resets Settings item value to default one
        /// </summary>
        /// <param name="Name">Settings item name</param>
        public void ResetToDefaultValue(string Name)
        {
            ResetToDefaultValue(Name, null);
        }


        public void ResetAllToDefault()
        {
            foreach (string key in Settings.Keys)
                ResetToDefaultValue(key, null);
        }


        /// <summary>
        /// Returns a partial hash token of a Settings item
        /// </summary>
        /// <typeparam name="T">Generic type</typeparam>
        /// <param name="item">Settings SettingsItem</param>
        /// <returns>Partial hash</returns>
        private string ComputePartialHash<T>(SettingsItem<T> item)
        {
            string hash = "";
            hash += ":name=" + item.Name;
            hash += ":value=" + item.Value;
            hash += ":ro=" + ((item.Readonly != null) ? item.Readonly.Value.ToString() : "?");
            //hash += ":min=" + ((item.MinValue != null) ? item.MinValue.ToString() : "?");
            //hash += ":max=" + ((item.MaxValue != null) ? item.MaxValue.ToString() : "?");
            //hash += ":default=" + ((item.Default != null) ? item.Default.ToString() : "?");
            //hash += ":date=" + ((item.ModifiedDate != null) ? item.ModifiedDate.Value.ToString() : "?");
            return hash;
        }


        /// <summary>
        /// Calculates the checksum/hash value of all Settings items
        /// </summary>
        /// <returns>checksum/hash value</returns>
        public string CalculateChecksumValue()
        {
            string part = "";
            ArrayList list = new ArrayList(Settings.Count);
            foreach (string key in Settings.Keys)
            {
                object setting = Settings[key, true];
                if (setting is SettingsItem<int?>)
                    part = ComputePartialHash<int?>(setting as SettingsItem<int?>);
                else if (setting is SettingsItem<bool?>)
                    part = ComputePartialHash<bool?>(setting as SettingsItem<bool?>);
                else if (setting is SettingsItem<double?>)
                    part = ComputePartialHash<double?>(setting as SettingsItem<double?>);
                else if (setting is SettingsItem<Color?>)
                    part = ComputePartialHash<Color?>(setting as SettingsItem<Color?>);
                else if (setting is SettingsItem<DateTime?>)
                    part = ComputePartialHash<DateTime?>(setting as SettingsItem<DateTime?>);
                //else if (setting is SettingsItem<Font>)
                //;// todo: part = ComputePartialHash<Font>(setting as SettingsItem<Font>);
                else
                    part = ComputePartialHash<string>(setting as SettingsItem<string>);

                list.Add(part);
            }

            // we need a list, to ensure that the order of the elements remains the same regardless of load-order
            list.Sort();
            StringBuilder sb = new StringBuilder("");
            foreach (string s in list)
                sb.Append(s);

            sb.Append(_magic_salt);

            ulong hash = 0;
            for (ulong i = 0; i < (ulong)sb.Length; i++)
                hash += i * (ulong)sb.Length + (ulong)Convert.ToInt64(sb[(int)i]);

            // adding some magic salt
            hash = long.MaxValue ^ hash + 08 + 20 + 1985;

            // mixing the values
            hash = (~hash) + (hash << 21);
            hash = hash ^ (hash >> 24);
            hash = (hash + (hash << 3)) + (hash << 8);
            hash = hash ^ (hash >> 14);
            hash = (hash + (hash << 2)) + (hash << 4);
            hash = hash ^ (hash >> 28);
            hash = hash + (hash << 31);

            return hash.ToString();
        }


        /// <summary>
        /// Generates and returns PropertyGrid-readable object that could be used as PropertyGrid.SelectedObject
        /// </summary>
        /// <returns>PropertyGrid-readable object</returns>
        public object GeneratePropertyGridObject()
        {
            PropertyContainer container = new PropertyContainer();
            Property.Settings = Settings;
            foreach (string key in Settings.Keys)
            {
                object setting = Settings[key, true];
                if (setting is SettingsItem<int?>)
                    container.AddProperty(new Property(key, Type.GetType("System.Int32"), (setting as SettingsItem<int?>).Readonly == null ? false : (setting as SettingsItem<int?>).Readonly.Value,
                        (setting as SettingsItem<int?>).MetaData.Description, (setting as SettingsItem<int?>).MetaData.Category, (setting as SettingsItem<int?>).MetaData.Visible));
                else if (setting is SettingsItem<bool?>)
                    container.AddProperty(new Property(key, Type.GetType("System.Boolean"), (setting as SettingsItem<bool?>).Readonly == null ? false : (setting as SettingsItem<bool?>).Readonly.Value,
                        (setting as SettingsItem<bool?>).MetaData.Description, (setting as SettingsItem<bool?>).MetaData.Category, (setting as SettingsItem<bool?>).MetaData.Visible));
                else if (setting is SettingsItem<double?>)
                    container.AddProperty(new Property(key, Type.GetType("System.Double"), (setting as SettingsItem<double?>).Readonly == null ? false : (setting as SettingsItem<double?>).Readonly.Value,
                        (setting as SettingsItem<double?>).MetaData.Description, (setting as SettingsItem<double?>).MetaData.Category, (setting as SettingsItem<double?>).MetaData.Visible));
                else if (setting is SettingsItem<Color?>)
                    container.AddProperty(new Property(key, typeof(System.Drawing.Color), (setting as SettingsItem<Color?>).Readonly == null ? false : (setting as SettingsItem<Color?>).Readonly.Value,
                        (setting as SettingsItem<Color?>).MetaData.Description, (setting as SettingsItem<Color?>).MetaData.Category, (setting as SettingsItem<Color?>).MetaData.Visible));
                else if (setting is SettingsItem<DateTime?>)
                    container.AddProperty(new Property(key, Type.GetType("System.DateTime"), (setting as SettingsItem<DateTime?>).Readonly == null ? false : (setting as SettingsItem<DateTime?>).Readonly.Value,
                        (setting as SettingsItem<DateTime?>).MetaData.Description, (setting as SettingsItem<DateTime?>).MetaData.Category, (setting as SettingsItem<DateTime?>).MetaData.Visible));
                else if (setting is SettingsItem<Font>)
                    container.AddProperty(new Property(key, typeof(System.Drawing.Font), (setting as SettingsItem<Font>).Readonly == null ? false : (setting as SettingsItem<Font>).Readonly.Value,
                        (setting as SettingsItem<Font>).MetaData.Description, (setting as SettingsItem<Font>).MetaData.Category, (setting as SettingsItem<Font>).MetaData.Visible));
                else // string
                    container.AddProperty(new Property(key, Type.GetType("System.String"), (setting as SettingsItem<string>).Readonly == null ? false : (setting as SettingsItem<string>).Readonly.Value,
                        (setting as SettingsItem<string>).MetaData.Description, (setting as SettingsItem<string>).MetaData.Category, (setting as SettingsItem<string>).MetaData.Visible));
                container[key] = Settings[key];
            }

            return container;
        }
    }

    #region Property Grid generation related code (PropertyContainer/PropertyDescriptor)
#pragma warning disable 1591
    [System.Diagnostics.DebuggerNonUserCode]
    public class PropertyContainer : Dictionary<string, object>, ICustomTypeDescriptor
    {
        public PropertyContainer()
        {
        }

        #region Fields
        Dictionary<string, Property> propertiesDefinition = new Dictionary<string, Property>();
        #endregion

        public void AddProperty(Property property)
        {
            this.propertiesDefinition.Add(property.Name, property);
        }

        #region ICustomTypeDescriptor Members
        public AttributeCollection GetAttributes() { return TypeDescriptor.GetAttributes(this, true); }
        public string GetClassName() { return TypeDescriptor.GetClassName(this, true); }
        public string GetComponentName() { return TypeDescriptor.GetComponentName(this); }
        public TypeConverter GetConverter() { return TypeDescriptor.GetConverter(this, true); }
        public PropertyDescriptor GetDefaultProperty() { return TypeDescriptor.GetDefaultProperty(this, true); }
        public object GetEditor(System.Type editorBaseType) { return TypeDescriptor.GetEditor(this, editorBaseType, true); }
        public EventDescriptor GetDefaultEvent() { return TypeDescriptor.GetDefaultEvent(this, true); }
        public EventDescriptorCollection GetEvents(System.Attribute[] attributes) { return TypeDescriptor.GetEvents(this, attributes, true); }
        public EventDescriptorCollection GetEvents() { return TypeDescriptor.GetEvents(this, true); }

        public object GetPropertyOwner(PropertyDescriptor pd) { return this; }
        public PropertyDescriptorCollection GetProperties(System.Attribute[] attributes) { return GetProperties(); }

        public PropertyDescriptorCollection GetProperties()
        {
            PropertyDescriptorCollection pdc = new PropertyDescriptorCollection(null);
            foreach (KeyValuePair<string, object> prop in this)
            {
                Property property;
                if (this.propertiesDefinition.TryGetValue(prop.Key, out property))
                    pdc.Add(property);
            }
            return pdc;
        }
        #endregion
    }

    [System.Diagnostics.DebuggerNonUserCode]
    public class Property : PropertyDescriptor
    {
        static public SettingsHashtable Settings;

        #region Fields
        private Type type;
        private bool readOnly;
        public string _description;
        public string _category;
        public bool? _visible = null;
        #endregion

        public Property(string name, Type type, bool readOnly, string description, string category, bool? visible)
            : base(name, null)
        {
            this.type = type;
            this.readOnly = readOnly;
            this._description = description;
            this._category = category;
            this._visible = visible;
        }

        public override bool IsReadOnly { get { return this.readOnly; } }
        public override Type PropertyType { get { return this.type; } }
        public override Type ComponentType { get { throw new NotImplementedException(); } }
        public override string DisplayName { get { return this.Name; } }
        public override bool CanResetValue(object component) { return false; }
        public override void ResetValue(object component) { }
        public override bool ShouldSerializeValue(object component) { return true; }
        public override string Description { get { return this._description; } }
        public override string Category { get { return this._category; } }
        public override bool IsBrowsable
        {
            get
            {
                if (_visible == null)
                    return true;
                return _visible.Value;
            }
        }

        public override object GetValue(object component)
        {
            PropertyContainer container = component as PropertyContainer;
            if (container == null)
                throw new Exception("Invalid component type");
            return container[Name];
        }

        public override void SetValue(object component, object value)
        {
            if (this.readOnly)
                throw new InvalidOperationException();

            PropertyContainer container = component as PropertyContainer;
            if (container == null)
                throw new Exception("Invalid component type");

            container[Name] = value;
            Settings[Name] = value;
        }
    }
#pragma warning restore 1591
    #endregion
    
}
