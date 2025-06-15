namespace Commons.Services;

public interface IDialogService
{
    void Close(object dialogIdentifier);

    Task ShowOrReplaceMessageDialogAsync(string header, string message, object dialogIdentifier);

    Task ShowOrReplaceMessageInActiveWindowAsync(string header, string message);

    IDisposable ShowProgressBarDialog(object dialogIdentifier);

    Task<bool> ShowExitConfirmation();
}