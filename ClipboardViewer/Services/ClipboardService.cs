using Commons;
using Commons.Services;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace ClipboardViewer.Services;

internal class ClipboardService : IClipboardService, IDisposable
{
    private HwndSource? _hwndSource;

    private DateTime _lastUpdate = DateTime.MinValue;

    public event EventHandler? ClipboardUpdated;

    public void Dispose() => Stop();

    public ClipboardData? GetClipboardData()
    {
        var dataObject = Clipboard.GetDataObject();

        string[] formats = dataObject.GetFormats();

        if (!formats.Any())
            return null;

        var clipboardData = new ClipboardData() { CreateAt = DateTimeOffset.Now };

        var items = new List<ClipboardDataItem>();
        foreach (var format in formats)
        {
            try
            {
                var data = dataObject.GetData(format, true);
                if (data is null)
                    continue;
                items.Add(new ClipboardDataItem
                {
                    Format = format,
                    Content = data
                });
            }
            catch (Exception ex)
            {
                items.Add(new ClipboardDataItem
                {
                    Format = format,
                    Error = ex
                });
            }
        }

        clipboardData.Items = items.ToArray();

        return clipboardData;
    }

    public void Start()
    {
        if (_hwndSource != null)
            return;

        var parameters = new HwndSourceParameters("ClipboardServiceHwnd")
        {
            Width = 0,
            Height = 0,
            WindowStyle = unchecked((int)0x80000000), // WS_POPUP
            ExtendedWindowStyle = 0x00000080 // WS_EX_TOOLWINDOW
        };

        _hwndSource = new HwndSource(parameters);
        _hwndSource.AddHook(WndProc);
        AddClipboardFormatListener(_hwndSource.Handle);
    }
    public void Stop()
    {
        if (_hwndSource != null)
        {
            RemoveClipboardFormatListener(_hwndSource.Handle);
            _hwndSource.RemoveHook(WndProc);
            _hwndSource.Dispose();
            _hwndSource = null;
        }
    }
    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool AddClipboardFormatListener(IntPtr hwnd);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool RemoveClipboardFormatListener(IntPtr hwnd);

    private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        const int WM_CLIPBOARDUPDATE = 0x031D;
        if (msg == WM_CLIPBOARDUPDATE)
        {
            var now = DateTime.Now;
            if ((now - _lastUpdate).TotalMilliseconds < 300)
                return IntPtr.Zero;
            _lastUpdate = now;

            ClipboardUpdated?.Invoke(this, EventArgs.Empty);
            handled = true;
        }
        return IntPtr.Zero;
    }
}