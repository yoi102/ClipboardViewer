using ClipboardViewer.wpf.UserControls.Controls;
using ClipboardViewer.wpf.UserControls.UserControls;
using MahApps.Metro.Controls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ClipboardViewer.UserControls;
/// <summary>
/// cImageViewer.xaml 的交互逻辑
/// </summary>
public partial class cImageViewer : UserControl
{
    public cImageViewer()
    {
        InitializeComponent();
    }

    public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(
  nameof(Source), typeof(ImageSource), typeof(cImageViewer), new PropertyMetadata(null));

    public ImageSource? Source
    {
        get => (ImageSource?)GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    private void SaveAs(object sender, RoutedEventArgs e)
    {
        if (Source is not BitmapSource bitmapSource)
            return;


        var saveFileDialog = new Microsoft.Win32.SaveFileDialog
        {
            Filter = "Image Files|*.bmp;",
            DefaultExt = ".bmp"
        };
        if (saveFileDialog.ShowDialog() == true)
        {
            var encoder = new BmpBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
            using (var stream = new System.IO.FileStream(saveFileDialog.FileName, System.IO.FileMode.Create))
            {
                encoder.Save(stream);
            }
        }
    }

    private void ShowInWindow(object sender, RoutedEventArgs e)
    {
        var window = new MetroWindow
        {
            Title = "Image Viewer",
            Content = new ImageEx { ImageSource = Source },
            Width = 800,
            Height = 600,
            WindowStartupLocation = WindowStartupLocation.CenterScreen
        };
        window.Show();
    }

    private void Fit(object sender, RoutedEventArgs e)
    {
        ImageEx.Fit();
    }
}