using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace ClipboardViewer.Converters;

internal class ObjectToBitmapSourceConverter : IValueConverter
{
    public static readonly ObjectToBitmapSourceConverter Instance = new();

    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is null)
            return null;

        // 已经是BitmapSource
        if (value is BitmapSource bitmapSource)
            return bitmapSource;

        // WriteableBitmap 也是BitmapSource的子类，无需特殊处理
        // 字节数组 byte[]
        if (value is byte[] bytes)
        {
            try
            {
                using var ms = new MemoryStream(bytes);
                return LoadBitmapFromStream(ms);
            }
            catch { return null; }
        }

        // Stream 类型
        if (value is Stream stream)
        {
            try
            {
                // Stream 可能不可Seek，为保险起见拷贝一份
                if (!stream.CanSeek)
                {
                    using var ms = new MemoryStream();
                    stream.CopyTo(ms);
                    ms.Position = 0;
                    return LoadBitmapFromStream(ms);
                }
                else
                {
                    // 注意：不要重置外部 stream 的 Position
                    long originalPosition = stream.Position;
                    var img = LoadBitmapFromStream(stream);
                    stream.Position = originalPosition; // 尽量恢复
                    return img;
                }
            }
            catch { return null; }
        }
        // 支持 System.Drawing.Bitmap
        if (value is System.Drawing.Bitmap gdiBitmap)
        {
            var hBitmap = gdiBitmap.GetHbitmap();
            try
            {
                var bs = Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, System.Windows.Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                bs.Freeze();
                return bs;
            }
            finally
            {
                NativeDeleteObject(hBitmap);
            }
        }
        // 文件路径 string 或 Base64
        if (value is string str)
        {
            try
            {
                str = str.Trim();
                // 兼容data:image/png;base64,xxxx
                int commaIndex = str.IndexOf(',');
                if (commaIndex >= 0 && str.Substring(0, commaIndex).Contains("base64"))
                    str = str[(commaIndex + 1)..];

                // Base64
                if (IsBase64String(str))
                {
                    var bytes2 = System.Convert.FromBase64String(str);
                    using var ms = new MemoryStream(bytes2);
                    return LoadBitmapFromStream(ms);
                }

                // 文件路径
                if (File.Exists(str))
                {
                    var img = new BitmapImage();
                    img.BeginInit();
                    img.CacheOption = BitmapCacheOption.OnLoad;
                    img.UriSource = new Uri(str, UriKind.Absolute);
                    img.EndInit();
                    img.Freeze();
                    return img;
                }
            }
            catch { return null; }
        }

        return null;
    }

    public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
               => throw new NotSupportedException();

    private static bool IsBase64String(string s)
    {
        // 简单判断
        s = s.Trim();
        return (s.Length % 4 == 0) && System.Convert.TryFromBase64String(s, new Span<byte>(new byte[s.Length]), out _);
    }

    [System.Runtime.InteropServices.DllImport("gdi32.dll")]
    private static extern bool DeleteObject(IntPtr hObject);

    private static void NativeDeleteObject(IntPtr hBitmap)
    {
        if (hBitmap != IntPtr.Zero)
            DeleteObject(hBitmap);
    }

    /// <summary>
    /// 从流创建 BitmapSource，并 Freeze。
    /// </summary>
    private static BitmapSource? LoadBitmapFromStream(Stream stream)
    {
        try
        {
            if (stream is null) return null;
            if (stream.CanSeek) stream.Position = 0;

            // 读头部判断格式
            byte[] header = new byte[8];
            stream.Read(header, 0, header.Length);
            stream.Position = 0;

            // DIB (from clipboard): 没有标准图片格式头
            bool isDib =
                !(header[0] == 0x89 && header[1] == 0x50) && // 不是PNG
                !(header[0] == 0xFF && header[1] == 0xD8) && // 不是JPEG
                !(header[0] == 0x42 && header[1] == 0x4D) && // 不是BMP
                !(header[0] == 0x47 && header[1] == 0x49 && header[2] == 0x46); // 不是GIF

            if (isDib)
            {
                // 装载整个流
                byte[] dibBuffer;
                using (var ms = new MemoryStream())
                {
                    stream.CopyTo(ms);
                    dibBuffer = ms.ToArray();
                }
                return DibToBitmapSource(dibBuffer);
            }
            else
            {
                // 标准格式
                var img = new BitmapImage();
                img.BeginInit();
                img.CacheOption = BitmapCacheOption.OnLoad;
                img.StreamSource = stream;
                img.EndInit();
                img.Freeze();
                return img;
            }
        }
        catch
        {
            return null;
        }
    }

    public static BitmapSource? DibToBitmapSource(byte[] dibBuffer)
    {
        // DIB -> BMP
        const int BMP_HEADER_SIZE = 14;
        int fileSize = BMP_HEADER_SIZE + dibBuffer.Length;
        int dataOffset = 40; // 假设为 BITMAPINFOHEADER（常见于剪贴板）

        using var ms = new MemoryStream();
        using (var bw = new BinaryWriter(ms, Encoding.Default, true))
        {
            bw.Write((byte)'B');
            bw.Write((byte)'M');
            bw.Write(fileSize);
            bw.Write(0); // reserved
            bw.Write(BMP_HEADER_SIZE + dataOffset);
            bw.Write(dibBuffer);
            bw.Flush();

            ms.Position = 0;
            var img = new BitmapImage();
            img.BeginInit();
            img.CacheOption = BitmapCacheOption.OnLoad;
            img.StreamSource = ms;
            img.EndInit();
            img.Freeze();
            return img;
        }
    }
}