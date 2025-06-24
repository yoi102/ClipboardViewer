using System.Collections;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace ClipboardViewer.Converters;
internal class ObjectToStringConverter : IValueConverter
{
    public static readonly ObjectToStringConverter Instance = new();

    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is null)
            return value;
        if (value is string stringValue)
        {
            return stringValue;
        }

        if (value is IEnumerable enumerable)
        {

            var sb = new StringBuilder();
            sb.AppendLine(enumerable.ToString());
            sb.AppendLine("{");
            foreach (object item in enumerable)
            {
                sb.AppendLine("\t" + item + ",");
            }
            sb.Append('}');
     
            return sb.ToString();   
        }

        return value.ToString();
    }

    public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
               => throw new NotSupportedException();

}
