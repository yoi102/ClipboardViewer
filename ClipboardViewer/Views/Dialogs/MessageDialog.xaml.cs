using System.Windows.Controls;

namespace ClipboardViewer.Views.Dialogs;

/// <summary>
/// MessageDialog.xaml 的交互逻辑
/// </summary>
public partial class MessageDialog : UserControl
{
    public MessageDialog(string header, string message)
    {
        InitializeComponent();
        HeaderTextBlock.Text = header;
        MessageTextBlock.Text = message;
    }
}