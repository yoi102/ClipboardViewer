namespace Commons.Services;

public interface IClipboardService
{
    event EventHandler? ClipboardUpdated;

    ClipboardData? GetClipboardData();

    void Start();

    void Stop();
}