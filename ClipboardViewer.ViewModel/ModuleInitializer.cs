using ClipboardViewer.ViewModel.ViewModels;
using Commons;
using Microsoft.Extensions.DependencyInjection;

namespace ClipboardViewer.ViewModel;

internal class ModuleInitializer : IModuleInitializer
{
    public void Initialize(IServiceCollection services)
    {
        services.AddTransient<MainWindowViewModel>();
        services.AddTransient<MainViewModel>();
    }
}