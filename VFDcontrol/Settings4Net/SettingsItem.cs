using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFDcontrol.Settings4Net
{
    /// <summary>
    /// Class for storing and handling a single Settings' item
    /// </summary>
    /// <typeparam name="T">Generic type</typeparam>
    public class SettingsItem<T>
    {
        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="Name">Name of the settings item</param>
        /// <param name="Value">Value of the settings item</param>
        /// <param name="ModifiedDate">Date/Time of last modification</param>
        /// <param name="Readonly">Writing/changing permissions</param>
        /// <param name="MinValue">Minimum allowed value</param>
        /// <param name="MaxValue">Maximum allowed value</param>
        /// <param name="Default">Default value</param>
        /// <param name="MetaData">MetaData object</param>
        public SettingsItem(string Name, T Value, DateTime? ModifiedDate, bool? Readonly, T MinValue, T MaxValue, T Default, SettingsMetaData MetaData)
        {
            this.Name = Name;
            this.Value = Value;
            this.ModifiedDate = ModifiedDate;
            this.Readonly = Readonly;
            this.MinValue = MinValue;
            this.MaxValue = MaxValue;
            this.Default = Default;
            if (MetaData != null)
                this.MetaData = MetaData;
        }

        private SettingsMetaData _metaData = new SettingsMetaData();

        public SettingsMetaData MetaData
        {
            set
            {
                _metaData = value;
            }
            get
            {
                return _metaData;
            }
        }

        /// <summary>
        /// Name of the settings item
        /// </summary>
        public string Name { set; get; }

        private T _value = default(T);
        /// <summary>
        /// Value of the settings item. Has to be not null, not readonly and (if set) between [MinValue; MaxValue]
        /// </summary>
        public T Value
        {
            set
            {
                if (value != null && (Readonly == null || Readonly.Value == false))
                {
                    bool ok = true;
                    if (value is int?)
                    {
                        if (MinValue != null && (MinValue as int?).Value > (value as int?).Value)
                            ok = false;
                        if (MaxValue != null && (MaxValue as int?).Value < (value as int?).Value)
                            ok = false;
                    }
                    else if (value is double?)
                    {
                        if (MinValue != null && (MinValue as double?).Value > (value as double?).Value)
                            ok = false;
                        if (MaxValue != null && (MaxValue as double?).Value < (value as double?).Value)
                            ok = false;
                    }
                    else if (value is double?)
                    {
                        if (MinValue != null && (MinValue as DateTime?).Value > (value as DateTime?).Value)
                            ok = false;
                        if (MaxValue != null && (MaxValue as DateTime?).Value < (value as DateTime?).Value)
                            ok = false;
                    }
                    if (ok)
                        _value = value;
                }
            }
            get
            {
                return _value;
            }
        }

        /// <summary>
        /// Last modification date/time
        /// </summary>
        public DateTime? ModifiedDate { set; get; }

        /// <summary>
        /// Permission to change the Value
        /// </summary>
        public bool? Readonly { set; get; }

        private T _minvalue = default(T);
        /// <summary>
        /// Minimum allowed value (for int/double/DateTime typs) or null
        /// </summary>
        public T MinValue
        {
            set
            {
                if (value != null && (value is int? || value is double? || value is DateTime?))
                    _minvalue = value;
            }
            get { return _minvalue; }
        }

        private T _maxvalue = default(T);
        /// <summary>
        /// Maximum allowed value (for int/double/DateTime typs) or null
        /// </summary>
        public T MaxValue
        {
            set
            {
                if (value != null && (value is int? || value is double? || value is DateTime?))
                    _maxvalue = value;
            }
            get { return _maxvalue; }
        }

        /// <summary>
        /// Default nullable value of the settings item
        /// </summary>
        public T Default { set; get; }
    }
}
