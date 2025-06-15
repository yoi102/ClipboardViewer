using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace ClipboardViewer.TemplateSelectors;

internal class ClipboardDataItemTemplateSelector : DataTemplateSelector
{
    public DataTemplate? StringTemplate { get; set; }
    public DataTemplate? ImageTemplate { get; set; }
    public DataTemplate? HtmlTemplate { get; set; }
    public DataTemplate? RtfTemplate { get; set; }

    public override DataTemplate? SelectTemplate(object item, DependencyObject container)
    {
        if (item is null)
        {
            return StringTemplate;
        }
        var content = item;

        // 判断图片（BitmapSource 或 Drawing.Bitmap）
        if (content is System.Windows.Media.ImageSource ||
            content is System.Windows.Media.Imaging.BitmapSource ||
            content is System.Drawing.Bitmap)
        {
            return ImageTemplate;
        }

        if (content is System.IO.Stream stream)
        {
            if (IsImageStream(stream))
            {
                return ImageTemplate;
            }
            else
            {
                return StringTemplate;
            }
        }

        // 判断字符串内容
        if (content is string str)
        {
            var txt = str.TrimStart();

            // 判断HTML
            if (IsClipboardHtml(txt))
            {
                return HtmlTemplate;
            }

            // 判断RTF
            if (txt.StartsWith(@"{\rtf", StringComparison.OrdinalIgnoreCase))
            {
                return RtfTemplate;
            }

            // 其他字符串
            return StringTemplate;
        }

        // 其他类型默认
        return StringTemplate;
    }

    private bool IsClipboardHtml(string html)
    {
        if (string.IsNullOrWhiteSpace(html))
            return false;

        // 剪贴板 HTML 格式特征：头部带 Version: 和 StartHTML:，内容含 <html>
        var t = html.TrimStart();
        return (t.StartsWith("Version:", StringComparison.OrdinalIgnoreCase) && t.Contains("<html", StringComparison.OrdinalIgnoreCase))
            || t.StartsWith("<!DOCTYPE html", StringComparison.OrdinalIgnoreCase)
            || t.StartsWith("<html", StringComparison.OrdinalIgnoreCase);
    }

    //这么做有点卡
    public static bool IsImageStream(Stream stream)
    {
        if (stream == null || !stream.CanRead)
            return false;

        long pos = stream.CanSeek ? stream.Position : 0;

        try
        {
            // 尝试标准图片
            var decoder = BitmapDecoder.Create(
                stream,
                BitmapCreateOptions.PreservePixelFormat,
                BitmapCacheOption.OnLoad);
            if (decoder.Frames.Count > 0)
                return true;
        }
        catch
        {

        }
        finally
        {
            if (stream.CanSeek)
                stream.Position = pos;
        }

        // 尝试DIB（典型头部：第1字节0x28为BITMAPINFOHEADER的biSize字段）
        try
        {
            byte[] header = new byte[40];
            int read = stream.Read(header, 0, header.Length);
            stream.Position = pos;
            if (read < 16) return false;

            int biSize = BitConverter.ToInt32(header, 0);
            if (biSize != 40 && biSize != 108 && biSize != 124)
                return false;
            int width = BitConverter.ToInt32(header, 4);
            int height = BitConverter.ToInt32(header, 8);
            if (width <= 0 || width > 32000 || Math.Abs(height) > 32000)
                return false;
            short planes = BitConverter.ToInt16(header, 12);
            if (planes != 1)
                return false;
            short bits = BitConverter.ToInt16(header, 14);
            if (bits != 1 && bits != 4 && bits != 8 && bits != 16 && bits != 24 && bits != 32)
                return false;
            return true;
        }
        catch
        {

        }
        finally
        {
            if (stream.CanSeek)
                stream.Position = pos;
        }

        return false;
    }
}