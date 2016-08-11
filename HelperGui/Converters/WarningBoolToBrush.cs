using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace HelperGui.Converters
{
    /// <summary>
    /// Converts a bool value to a brush color:
    /// - 'true' -> pale blue
    /// - 'false' -> red
    /// </summary>
    public class WarningBoolToBrush : IValueConverter
    {
        private static readonly Brush s_okBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0x70, 0xC0, 0xE7));
        private static readonly Brush s_warnBrush = Brushes.Red;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.Equals(DependencyProperty.UnsetValue))
            {
                return s_okBrush;
            }

            if (!(value is bool))
            {
                throw new ArgumentException("Converter parameter has to be of boolean type.");
            }

            bool predicate = (bool)value;

            return predicate ? s_okBrush : s_warnBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
