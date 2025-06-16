using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Point = System.Windows.Point;

namespace ClipboardViewer.wpf.UserControls.UserControls;

/// <summary>
/// ImageEx.xaml 的交互逻辑
/// </summary>
public partial class ImageEx : UserControl
{
    public static readonly DependencyProperty ImageSourceProperty =
        DependencyProperty.Register("ImageSource",
            typeof(ImageSource), typeof(ImageEx),
            new FrameworkPropertyMetadata(
                            null,
                            FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender,
                            new PropertyChangedCallback(OnImageSourceChanged),
                            null),
                    null);

    private byte[,]? imageData;

    private Vec3b[,]? imageData3b;

    private int maxX;

    private int maxY;

    private Point middleButtonClickedPosition;

    private int mouseDownCount = 0;

    public ImageEx()
    {
        InitializeComponent();
    }

    public ImageSource? ImageSource
    {
        get { return (ImageSource?)GetValue(ImageSourceProperty); }
        set { SetValue(ImageSourceProperty, value); }
    }

    private static void OnImageSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var imageEx = (ImageEx)d;
        if (e.NewValue is ImageSource image)
        {
            imageEx.image.Source = image;
            imageEx.GetImageSourceData();
        }
        else
        {
            imageEx.image.Source = null;
        }
    }

    private void BackFrame_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.MiddleButton == MouseButtonState.Pressed)
        {
            mouseDownCount += 1;
            DispatcherTimer timer = new();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 300);
            timer.Tick += (s, e1) => { timer.IsEnabled = false; mouseDownCount = 0; };
            timer.IsEnabled = true;
            if (mouseDownCount % 2 == 0)
            {
                timer.IsEnabled = false;
                mouseDownCount = 0;

                var group = (TransformGroup)image.RenderTransform;
                group.Children[0] = new ScaleTransform();
                group.Children[1] = new TranslateTransform();
            }
        }
    }

    private void GetImageSourceData()
    {
        if (ImageSource is BitmapSource bmp)
        {
            int width = bmp.PixelWidth;
            int height = bmp.PixelHeight;
            int stride = (width * bmp.Format.BitsPerPixel + 7) / 8;
            byte[] pixels = new byte[height * stride];
            bmp.CopyPixels(pixels, stride, 0);

            // 处理像素格式
            if (bmp.Format == PixelFormats.Gray8) // 1通道灰度
            {
                imageData = new byte[height, width];
                imageData3b = null;
                for (int y = 0; y < height; y++)
                    for (int x = 0; x < width; x++)
                        imageData[y, x] = pixels[y * stride + x];

                maxY = height;
                maxX = width;
                GrayPanel.Visibility = Visibility.Visible;
                RGBPanel.Visibility = Visibility.Collapsed;
            }
            else if (bmp.Format == PixelFormats.Bgr24) // 3通道
            {
                imageData = null;
                imageData3b = new Vec3b[height, width];
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int idx = y * stride + x * 3;
                        byte b = pixels[idx];
                        byte g = pixels[idx + 1];
                        byte r = pixels[idx + 2];
                        imageData3b[y, x] = new Vec3b(b, g, r);
                    }
                }
                maxY = height;
                maxX = width;
                GrayPanel.Visibility = Visibility.Collapsed;
                RGBPanel.Visibility = Visibility.Visible;
            }
            else if (bmp.Format == PixelFormats.Bgr32) // 4字节 BGR，不含Alpha
            {
                imageData = null;
                imageData3b = new Vec3b[height, width];
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int idx = y * stride + x * 4;
                        byte b = pixels[idx];
                        byte g = pixels[idx + 1];
                        byte r = pixels[idx + 2];
                        // 第4字节通常是填充位，可忽略
                        imageData3b[y, x] = new Vec3b(b, g, r);
                    }
                }
                maxY = height;
                maxX = width;
                GrayPanel.Visibility = Visibility.Collapsed;
                RGBPanel.Visibility = Visibility.Visible;
            }
            else if (bmp.Format == PixelFormats.Bgra32) // 4通道，转3通道
            {
                imageData = null;
                imageData3b = new Vec3b[height, width];
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int idx = y * stride + x * 4;
                        byte b = pixels[idx];
                        byte g = pixels[idx + 1];
                        byte r = pixels[idx + 2];
                        // 忽略Alpha
                        imageData3b[y, x] = new Vec3b(b, g, r);
                    }
                }
                maxY = height;
                maxX = width;
                GrayPanel.Visibility = Visibility.Collapsed;
                RGBPanel.Visibility = Visibility.Visible;
            }
            else
            {
                // 其他格式（如Pbgra32等），可自行补充
                imageData = null;
                imageData3b = null;
                maxY = maxX = 0;
                GrayPanel.Visibility = Visibility.Collapsed;
                RGBPanel.Visibility = Visibility.Collapsed;
            }
        }
    }

    private void Image_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
        {
            middleButtonClickedPosition = e.GetPosition((IInputElement)e.Source);
        }
    }

    private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
    }

    private void Image_MouseMove(object sender, MouseEventArgs e)
    {
        var cursorPosition = e.GetPosition((IInputElement)e.Source);

        //获取控件大小
        Image imageControl = (Image)(IInputElement)e.Source;

        if (ImageSource is null)
            return;

        double xRatio = ImageSource.Width / imageControl.ActualWidth;
        double yRatio = ImageSource.Height / imageControl.ActualHeight;

        double x = cursorPosition.X * xRatio;
        double y = cursorPosition.Y * yRatio;
        int int_x = (int)x;
        int int_y = (int)y;

        Path_X.Text = x.ToString("0.00");
        Path_Y.Text = y.ToString("0.00");
        //获取图片像素信息
        if (imageData3b != null)
        {
            if (y < maxY && x < maxX)
            {
                Path_B.Text = imageData3b[int_y, int_x].Item0.ToString("000");
                Path_G.Text = imageData3b[int_y, int_x].Item1.ToString("000");
                Path_R.Text = imageData3b[int_y, int_x].Item2.ToString("000");
            }
        }
        else if (imageData != null)
        {
            Path_Gray.Text = imageData[int_y, int_x].ToString("000");
            Path_Gray.Text = imageData[int_y, int_x].ToString("000");
            Path_Gray.Text = imageData[int_y, int_x].ToString("000");
        }

        //当中键按下，移动图片
        if (e.LeftButton == MouseButtonState.Pressed)
        {
            Image im = (Image)sender;
            var group = (TransformGroup)im.RenderTransform;
            var ttf = (TranslateTransform)group.Children[1];//对应Xaml位置    这样搞，放大缩小有点奇怪

            ttf.X += cursorPosition.X - middleButtonClickedPosition.X;
            ttf.Y += cursorPosition.Y - middleButtonClickedPosition.Y;
        }
    }

    //int mouseDownCount = 0;
    private void Image_MouseWheel(object sender, MouseWheelEventArgs e)
    {
        Image sf = (Image)sender;
        var group = (TransformGroup)sf.RenderTransform;
        var sc = (ScaleTransform)group.Children[0];//对应Xaml位置    这样搞，放大缩小有点奇怪
        var cursorPosition = e.GetPosition((IInputElement)e.Source);
        sc.CenterX = cursorPosition.X;
        sc.CenterY = cursorPosition.Y;
        //sc.ScaleX += e.Delta * 0.001;
        //sc.ScaleY += e.Delta * 0.001;

        if (e.Delta > 0)
        {
            sc.ScaleX += 0.05;
            sc.ScaleY += 0.05;
        }
        else
        {
            if (sc.ScaleX > 0.55)
            {
                sc.ScaleX -= 0.05;
                sc.ScaleY -= 0.05;
            }
        }
    }

    public void Fit()
    {
        var scr = BackFrame;

        var im = (Image)scr.Content;

        var group = (TransformGroup)im.RenderTransform;
        group.Children[0] = new ScaleTransform();
        group.Children[1] = new TranslateTransform();
    }
}

public struct Vec3b
{
    public byte Item0;
    public byte Item1;
    public byte Item2;

    public Vec3b(byte b, byte g, byte r)
    {
        Item0 = b; Item1 = g; Item2 = r;
    }
}