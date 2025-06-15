using Commons.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ClipboardViewer.ViewModel;

public partial class MainWindowViewModel : ObservableObject
{
    private readonly ICultureSettingService cultureSettingService;

    private readonly IThemeSettingService themeSettingService;

    [ObservableProperty]
    private bool _topmost;

    [ObservableProperty]
    private int currentCultureLCID;

    private bool isDarkTheme;

    public MainWindowViewModel(IThemeSettingService themeSettingService, ICultureSettingService cultureSettingService)
    {
        this.themeSettingService = themeSettingService;
        this.cultureSettingService = cultureSettingService;
        currentCultureLCID = System.Globalization.CultureInfo.CurrentCulture.LCID;
        isDarkTheme = themeSettingService.IsDarkTheme;
    }

    public bool IsDarkTheme
    {
        get { return isDarkTheme; }
        set
        {
            if (SetProperty(ref isDarkTheme, value))
            {
                themeSettingService.ApplyThemeLightDark(value);
            }
        }
    }

    [RelayCommand]
    private void ChangeCulture(string lcidString)
    {
        if (!int.TryParse(lcidString, out var lcid))
            return;
        CurrentCultureLCID = lcid;
        cultureSettingService.ChangeCulture(lcid);
    }

    [RelayCommand]
    private void ChangeTopmost()
    {
        Topmost = !Topmost;
    }
}