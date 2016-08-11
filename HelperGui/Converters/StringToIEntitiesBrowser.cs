using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

namespace HelperGui.Converters
{
    /// <summary>
    /// Used to convert a string denoting a type of an entities browser into the real object representing that entities browser.
    /// </summary>
    public class StringToIEntitiesBrowser : TypeConverter
    {
        private const string c_filesBrowser = "FilesBrowser";
        private const string c_executablesBrowser = "ExecutablesBrowser";
        private const string c_foldersBrowser = "FoldersBrowser";

        private static readonly Dictionary<string, IEntitiesBrowser> s_entityBrowsers =
            new Dictionary<string, IEntitiesBrowser>
            {
                { c_filesBrowser, new FilesBrowser() },
                { c_executablesBrowser, new ExecutablesBrowser() },
                { c_foldersBrowser, new FoldersBrowser() }
            };

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(s_entityBrowsers.Values);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }

            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            string input = value as string;

            if (input != null && s_entityBrowsers.ContainsKey(input))
            {
                return s_entityBrowsers[input];
            }

            return base.ConvertFrom(context, culture, value);
        }
    }
}