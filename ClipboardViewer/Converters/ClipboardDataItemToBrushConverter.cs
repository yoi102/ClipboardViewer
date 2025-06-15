using Commons;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace ClipboardViewer.Converters;

internal class ClipboardDataItemToBrushConverter : IValueConverter
{
    public static readonly ClipboardDataItemToBrushConverter Instance = new();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not ClipboardDataItem clipboardDataItem)
        {
            return Brushes.Red;
        }

        if (clipboardDataItem.Error is not null)
        {
            return Brushes.Red;
        }
        if (clipboardDataItem.Content is null)
        {
            return Brushes.Gray;
        }

        return Brushes.Green;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}