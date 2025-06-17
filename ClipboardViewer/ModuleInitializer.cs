using ClipboardViewer.Services;
using Commons;
using Commons.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ClipboardViewer;

internal class ModuleInitializer : IModuleInitializer
{
    public void Initialize(IServiceCollection services)
    {
        services.AddTransient<ICultureSettingService, CultureSettingService>();
        services.AddTransient<IThemeSettingService, ThemeSettingService>();
        services.AddTransient<IDialogService, DialogService>();
        services.AddSingleton<IClipboardService, ClipboardService>();
        services.AddSingleton<ISnackbarService, SnackbarService>();
    }
}