using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DbSeeder.WPF.Converters
{
    public class RadioButtonContentToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null) return DependencyProperty.UnsetValue;
            if (parameter is null) return DependencyProperty.UnsetValue;

            return value.Equals(parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.Equals(true) ? parameter : Binding.DoNothing;
        }

    }
}
