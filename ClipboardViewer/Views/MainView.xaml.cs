using ClipboardViewer.ViewModel.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Windows.Controls;

namespace ClipboardViewer.Views;

/// <summary>
/// MainView.xaml 的交互逻辑
/// </summary>
public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
        this.DataContext = App.Current.Services.GetService<MainViewModel>();
    }
}