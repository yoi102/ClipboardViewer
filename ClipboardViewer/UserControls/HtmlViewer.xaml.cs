using System.Windows;
using System.Windows.Controls;

namespace ClipboardViewer.UserControls;
/// <summary>
/// HtmlViewer.xaml 的交互逻辑
/// </summary>
public partial class HtmlViewer : UserControl
{
    public HtmlViewer()
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
        DependencyProperty.Register("Source", typeof(string), typeof(HtmlViewer), new PropertyMetadata("", SourceChanged));

    private static void SourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not HtmlViewer viewer)
            return;

        var htmlString = e.NewValue as string ?? string.Empty;
        if (string.IsNullOrEmpty(htmlString))
        {
            viewer.WebBrowser.NavigateToString("<html><body></body></html>"); // 加载空白页
            viewer.TextBox.Text = htmlString;
            return;
        }
        viewer.WebBrowser.NavigateToString(htmlString);
        viewer.TextBox.Text = htmlString;
    }
}