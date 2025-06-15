using System.Globalization;
using System.Windows.Data;

namespace ClipboardViewer.Converters;
internal class ObjectTypeToStringConverter : IValueConverter
{
    public static readonly ObjectTypeToStringConverter Instance = new();

    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is null)
            return "Null";
        return value.GetType().ToString();
    }

    public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
               => throw new NotSupportedException();

}
