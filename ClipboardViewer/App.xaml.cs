using Commons.Extensions;
using Commons.Helpers;
using Commons.Services;
using ExceptionHandling;
using Microsoft.Extensions.DependencyInjection;
using Resources.Strings;
using System.Diagnostics;
using System.Windows;

namespace ClipboardViewer;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public App()
    {
        string lang = System.Globalization.CultureInfo.CurrentCulture.Name;
        var culture = new System.Globalization.CultureInfo(lang);
        Thread.CurrentThread.CurrentCulture = culture;
        Thread.CurrentThread.CurrentUICulture = culture;
        I18NExtension.Culture = culture;

        Services = ConfigureServices();

        var clipboardService = Services.GetService<IClipboardService>()!;
        clipboardService.Start();

        RegisterGlobalExceptionHandlers();

        Application.Current.Exit += (s, e) =>
        {
            var clipboardService = Services.GetService<IClipboardService>()!;
            clipboardService.Stop();
        };
    }

    private void RegisterGlobalExceptionHandlers()
    {
        Application.Current.DispatcherUnhandledException += async (sender, e) =>
        {
            // ❗ 先标记 Handled，避免 await 造成异常泄露
            e.Handled = true;
            var handled = await Services.GetService<IExceptionDispatcher>()!.DispatchAsync(e.Exception);

            if (!handled)
            {
                var dialogService = Services.GetService<IDialogService>()!;

                //阻塞UI线程，等待对话框关闭
                await dialogService.ShowOrReplaceMessageInActiveWindowAsync(Strings.UnhandledException, e.Exception.Message);

                //Debug模式的话就强制中断
                Debugger.Break();
                //不允许存在未处理的异常，应用程序将会终止
                App.Current.Shutdown(1);
            }
        };
    }

    public IServiceProvider Services { get; }

    /// <summary>
    /// Gets the current <see cref="App"/> instance in use
    /// </summary>
    public new static App Current => (App)Application.Current;

    /// <summary>
    /// Configures the services for the application.
    /// </summary>
    private static IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        var assemblies = ReflectionHelper.GetAllReferencedAssemblies();
        services.RunModuleInitializers(assemblies);

        services.RegisterHandlers(assemblies);

        return services.BuildServiceProvider();
    }
}