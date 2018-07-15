using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFDcontrol.Settings4Net
{
    /// <summary>
    /// Class for storing Settings items
    /// </summary>
    [Serializable]
    public class SettingsHashtable : Hashtable
    {
        /// <summary>
        /// Adds a new Settings item
        /// </summary>
        /// <param name="key">Settings item name</param>
        /// <param name="value">Settings item value</param>
        public override void Add(object key, object value)
        {
            base[key] = value;
        }

        /// <summary>
        /// Returns the whole Settings Item object
        /// </summary>
        /// <param name="key">Item key in the hashtable</param>
        /// <param name="argument">Additional internal argument</param>
        /// <returns>Item object</returns>
        public object this[object key, object argument]
        {
            get
            {
                return base[key];
            }
            // not set
            private set
            {
            }
        }

        /// <summary>
        /// Settings items
        /// </summary>
        /// <param name="key">Settings item name</param>
        /// <returns>Settings item value</returns>
        public override object this[object key]
        {
            get
            {
                if (base[key] == null)
                    throw new Exception("Item doesn't exist.");

                if (base[key] is SettingsItem<int?>)
                    return ((base[key] as SettingsItem<int?>).Value).Value;
                else if (base[key] is SettingsItem<string>)
                    return (base[key] as SettingsItem<string>).Value;
                else if (base[key] is SettingsItem<bool?>)
                    return (base[key] as SettingsItem<bool?>).Value;
                else if (base[key] is SettingsItem<double?>)
                    return (base[key] as SettingsItem<double?>).Value;
                else if (base[key] is SettingsItem<Color?>)
                    return (base[key] as SettingsItem<Color?>).Value;
                else if (base[key] is SettingsItem<DateTime?>)
                    return (base[key] as SettingsItem<DateTime?>).Value;
                else if (base[key] is SettingsItem<Font>)
                    return (base[key] as SettingsItem<Font>).Value;

                return null;
            }
            set
            {
                if (base[key] == null)
                {
                    if (value is int)
                        base[key] = new SettingsItem<int?>(key.ToString(), (int)value, null, null, null, null, null, null);
                    else if (value is string)
                        base[key] = new SettingsItem<string>(key.ToString(), (string)value, null, null, null, null, null, null);
                    else if (value is bool)
                        base[key] = new SettingsItem<bool?>(key.ToString(), (bool)value, null, null, null, null, null, null);
                    else if (value is double)
                        base[key] = new SettingsItem<double?>(key.ToString(), (double)value, null, null, null, null, null, null);
                    else if (value is Color)
                        base[key] = new SettingsItem<Color?>(key.ToString(), (Color)value, null, null, null, null, null, null);
                    else if (value is DateTime)
                        base[key] = new SettingsItem<DateTime?>(key.ToString(), (DateTime)value, null, null, null, null, null, null);
                    else if (value is Font)
                        base[key] = new SettingsItem<Font>(key.ToString(), (Font)value, null, null, null, null, null, null);
                }
                else
                {
                    if (base[key] is SettingsItem<int?>)
                        ((SettingsItem<int?>)base[key]).Value = (int?)value;
                    else if (base[key] is SettingsItem<string>)
                        ((SettingsItem<string>)base[key]).Value = (string)value;
                    else if (base[key] is SettingsItem<bool?>)
                        ((SettingsItem<bool?>)base[key]).Value = (bool)value;
                    else if (base[key] is SettingsItem<double?>)
                        ((SettingsItem<double?>)base[key]).Value = (double)value;
                    else if (base[key] is SettingsItem<Color>)
                        ((SettingsItem<Color?>)base[key]).Value = (Color)value;
                    else if (base[key] is SettingsItem<DateTime>)
                        ((SettingsItem<DateTime?>)base[key]).Value = (DateTime)value;
                    else if (base[key] is SettingsItem<Font>)
                        (base[key] as SettingsItem<Font>).Value = (Font)value;
                }
            }
        }
    }
}
