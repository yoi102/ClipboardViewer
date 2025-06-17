using Commons;
using Commons.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace ClipboardViewer.ViewModel.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly IClipboardService clipboardService;
    private readonly ISnackbarService snackbarService;

    [ObservableProperty]
    private ObservableCollection<ClipboardData> clipboardDataCollection = [];

    private ClipboardData? selectedClipboardData;

    [ObservableProperty]
    private ClipboardDataItem? selectedClipboardDataItem;

    public MainViewModel(IClipboardService clipboardService, ISnackbarService snackbarService)
    {
        this.clipboardService = clipboardService;
        this.snackbarService = snackbarService;
        AddClipboardData();
        clipboardService.ClipboardUpdated += async (sender, e) =>
        {
            await Task.Delay(600); // Delay to allow clipboard to stabilize
            AddClipboardData();
            snackbarService.Enqueue("MainWindow", $"Updated at {DateTimeOffset.Now}");
        };
    }

    public ClipboardData? SelectedClipboardData
    {
        get { return selectedClipboardData; }
        set
        {
            if (SetProperty(ref selectedClipboardData, value))
            {
                SelectedClipboardDataItem = SelectedClipboardData?.Items?.LastOrDefault();
            }
        }
    }

    [RelayCommand]
    private void AddClipboardData()
    {
        var data = clipboardService.GetClipboardData();
        if (data is null)
            return;
        var lastData = ClipboardDataCollection.LastOrDefault();

        if (lastData is not null && lastData.Items.SequenceEqual(data.Items))
            return;

        ClipboardDataCollection.Add(data);
        SelectedClipboardData = ClipboardDataCollection.LastOrDefault();
        SelectedClipboardDataItem = SelectedClipboardData?.Items?.FirstOrDefault();
    }

    [RelayCommand]
    private void ClearClipboardData()
    {
        ClipboardDataCollection.Clear();
        SelectedClipboardData = null;
        SelectedClipboardDataItem = null;
    }
}