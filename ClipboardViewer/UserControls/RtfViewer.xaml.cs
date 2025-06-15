using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace ClipboardViewer.UserControls;
/// <summary>
/// RtfViewer.xaml 的交互逻辑
/// </summary>
public partial class RtfViewer : UserControl
{
    public RtfViewer()
    {
        InitializeComponent();
    }
    public string Source
    {
        get { return (string)GetValue(SourceProperty); }
        set { SetValue(SourceProperty, value); }
    }

    // Using a DependencyProperty as the backing store for Source.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty SourceProperty =
        DependencyProperty.Register("Source", typeof(string), typeof(RtfViewer), new PropertyMetadata("", SourceChanged));

    private static void SourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not RtfViewer viewer)
            return;
        var rtfString = e.NewValue as string ?? string.Empty;
        if (string.IsNullOrEmpty(rtfString))
            return;
        var textRange = new TextRange(viewer.RichTextBox.Document.ContentStart, viewer.RichTextBox.Document.ContentEnd);
        using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(rtfString)))
        {
            textRange.Load(stream, DataFormats.Rtf);
        }

        viewer.TextBox.Text = rtfString;
    }
}
