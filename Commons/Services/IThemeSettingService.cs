namespace Commons.Services;

public interface IThemeSettingService
{
    bool IsDarkTheme { get; }

    void ToggleThemeLightDark();

    void ApplyThemeLightDark(bool isDarkTheme);
}