using ClipboardViewer.ViewModel;
using Commons.Services;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;

namespace ClipboardViewer;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    public MainWindow()
    {
        InitializeComponent();
        this.DataContext = App.Current.Services.GetService<MainWindowViewModel>();
        WindowTracker.Register(this);
        Closing += OnWindowClosing;
    }

    private void OnWindowClosing(object? sender, CancelEventArgs e)
    {
        e.Cancel = true; // 临时取消关闭

        // 不能直接 await，所以用 async void 包装异步处理
        HandleExitConfirmationAsync(e);
    }

    private async void HandleExitConfirmationAsync(CancelEventArgs e)
    {
        var dialog = App.Current.Services.GetRequiredService<IDialogService>();

        bool confirm = await dialog.ShowExitConfirmation();

        if (confirm)
        {
            // 手动移除关闭事件，避免递归触发
            Closing -= OnWindowClosing;

            Close();  // 程序关闭
        }
    }
}