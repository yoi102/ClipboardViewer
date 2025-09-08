using ClipboardViewer.Extensions;
using ClipboardViewer.Views.Dialogs;
using Commons;
using Commons.Services;
using MaterialDesignThemes.Wpf;
using System.Windows;

namespace ClipboardViewer.Services;

internal class DialogService : IDialogService
{
    private readonly IWindowTrackService windowTrackService;

    public DialogService(IWindowTrackService windowTrackService)
    {
        this.windowTrackService = windowTrackService;
    }

    public IDisposable ShowProgressBarDialog(object dialogIdentifier)
    {
        var dialogSession = DialogHost.GetDialogSession(dialogIdentifier);
        ProgressDialog progressDialog = new ProgressDialog();

        if (dialogSession is not null)
        {
            dialogSession.UpdateContent(progressDialog);
        }
        else
        {
            DialogHost.Show(progressDialog, dialogIdentifier);
        }

        return new DeferredScope(() => { Close(dialogIdentifier); });
    }

    public void Close(object dialogIdentifier)
    {
        var dialogSession = DialogHost.GetDialogSession(dialogIdentifier);
        dialogSession?.Close();
    }

    public async Task ShowOrReplaceMessageDialogAsync(string header, string message, object dialogIdentifier)
    {
        var dialogSession = DialogHost.GetDialogSession(dialogIdentifier);
        if (dialogSession is not null)
        {
            //await dialogSession.UpdateContent(messageDialog);//await 不了！！！！遗憾，不能直接更新
            dialogSession.Close();//无奈之举，只能关闭后重开
        }

        MessageDialog messageDialog = new MessageDialog(header, message);
        await DialogHost.Show(messageDialog, dialogIdentifier);
    }

    public async Task ShowOrReplaceMessageInActiveWindowAsync(string header, string message)
    {
        var activeWindow = windowTrackService.LastActivatedWindow;

        var dialogHost = GetFirstDialogHost(activeWindow);
        if (dialogHost is null)
            return;

        // 关闭当前打开的对话框，确保新的对话框可以正确显示
        var identifier = dialogHost.Identifier;

        var dialogSession = DialogHost.GetDialogSession(identifier);
        dialogSession?.Close();

        MessageDialog messageDialog = new MessageDialog(header, message);
        await dialogHost.ShowDialog(messageDialog);
    }

    private static DialogHost? GetFirstDialogHost(Window window)
    {
        if (window is null) throw new ArgumentNullException(nameof(window));

        DialogHost? dialogHost = window.VisualDepthFirstTraversal().OfType<DialogHost>().FirstOrDefault();

        return dialogHost;
    }

    public async Task<bool> ShowExitConfirmation()
    {
        var activeWindow = windowTrackService.LastActivatedWindow;

        var dialogHost = GetFirstDialogHost(activeWindow);
        if (dialogHost is null)
            return false;

        var identifier = dialogHost.Identifier;
        var dialogSession = DialogHost.GetDialogSession(identifier);
        dialogSession?.Close();

        var exitDialog = new ExitConfirmDialog();  // 自定义的 UserControl
        var result = await dialogHost.ShowDialog(exitDialog);

        return result is string resultString && resultString == bool.TrueString;
    }
}