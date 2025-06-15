using Commons.Services;
using MaterialDesignThemes.Wpf;

namespace ClipboardViewer.Services;

internal class ThemeSettingService : IThemeSettingService
{
    private readonly PaletteHelper paletteHelper;
    private readonly Theme theme;

    public ThemeSettingService()
    {
        paletteHelper = new PaletteHelper();
        theme = paletteHelper.GetTheme();
    }

    public bool IsDarkTheme
    {
        get
        {
            var currentBaseTheme = theme.GetBaseTheme();
            return currentBaseTheme == BaseTheme.Dark;
        }
    }

    public void ApplyThemeLightDark(bool isDarkTheme)
    {
        if (isDarkTheme)
        {
            theme.SetDarkTheme();
        }
        else
        {
            theme.SetLightTheme();
        }
        paletteHelper.SetTheme(theme);
    }

    public void ToggleThemeLightDark()
    {
        var currentBaseTheme = theme.GetBaseTheme();
        if (currentBaseTheme != BaseTheme.Dark)
        {
            theme.SetDarkTheme();
        }
        else
        {
            theme.SetLightTheme();
        }
        paletteHelper.SetTheme(theme);
    }
}