using System;
using System.Globalization;
using System.Net.Http;
using System.Windows.Controls;
using System.Windows.Data;

namespace DbSeeder.WPF.Converters
{
    /// <summary>
    /// Class to convert a string to a valid HttpMethod object.
    /// Value must be of string and must be matchable with Post/Patch/Put/Delete
    /// </summary>
    public class StringToHttpMethodConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null) throw new ArgumentNullException($"Invalid value received {nameof(value)}");

            return value switch
            {
                string _ when string.Equals(value.ToString(), HttpMethod.Post.ToString(),
                    StringComparison.OrdinalIgnoreCase) => HttpMethod.Post,
                string _ when string.Equals(value.ToString(), HttpMethod.Patch.ToString(),
                    StringComparison.OrdinalIgnoreCase) => HttpMethod.Patch,
                string _ when string.Equals(value.ToString(), HttpMethod.Put.ToString(),
                    StringComparison.OrdinalIgnoreCase) => HttpMethod.Put,
                string _ when string.Equals(value.ToString(), HttpMethod.Delete.ToString(),
                    StringComparison.OrdinalIgnoreCase) => HttpMethod.Delete,
                _ => new InvalidCastException($"Value received cannot be cast to HttpMethod - {value}")
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null) throw new ArgumentNullException($"Invalid value received {value}");

            if (!(value is HttpMethod)) throw new InvalidCastException($"Value received is not a valid HttpMethod. {value}");

            return value switch
            {
                HttpMethod _ when string.Equals(value.ToString(), HttpMethod.Post.ToString(),
                    StringComparison.OrdinalIgnoreCase) => "Post",
                HttpMethod _ when string.Equals(value.ToString(), HttpMethod.Patch.ToString(),
                    StringComparison.OrdinalIgnoreCase) => "Patch",
                HttpMethod _ when string.Equals(value.ToString(), HttpMethod.Put.ToString(),
                    StringComparison.OrdinalIgnoreCase) => "Put",
                HttpMethod _ when string.Equals(value.ToString(), HttpMethod.Delete.ToString(),
                    StringComparison.OrdinalIgnoreCase) => "Delete",
                HttpMethod _ when string.Equals(value.ToString(), HttpMethod.Get.ToString(),
                    StringComparison.OrdinalIgnoreCase) => "Get",
                HttpMethod _ when string.Equals(value.ToString(), HttpMethod.Head.ToString(),
                    StringComparison.OrdinalIgnoreCase) => "Head",
                HttpMethod _ when string.Equals(value.ToString(), HttpMethod.Options.ToString(),
                    StringComparison.OrdinalIgnoreCase) => "Options",
                HttpMethod _ when string.Equals(value.ToString(), HttpMethod.Trace.ToString(),
                    StringComparison.OrdinalIgnoreCase) => "Trace",
                _ => new InvalidCastException($"Value received is not a valid HttpMethod - {value}")

            };
        }
    }
}