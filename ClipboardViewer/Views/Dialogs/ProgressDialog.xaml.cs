using ClipboardViewer.ViewModel.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Windows.Controls;

namespace ClipboardViewer.Views.Dialogs;

/// <summary>
/// ProgressDialog.xaml 的交互逻辑
/// </summary>
public partial class ProgressDialog : UserControl
{
    public ProgressDialog()
    {
        InitializeComponent();
        this.DataContext = App.Current.Services.GetService<MainViewModel>();
    }
}