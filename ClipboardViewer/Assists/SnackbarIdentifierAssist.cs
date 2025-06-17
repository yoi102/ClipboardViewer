using MaterialDesignThemes.Wpf;
using System.Windows;

namespace ClipboardViewer.Assists;

public static class SnackbarIdentifierAssist
{
    private static readonly Dictionary<object, List<Snackbar>> _snackbarGroups = [];

    public static IReadOnlyDictionary<object, List<Snackbar>> SnackbarGroups => _snackbarGroups;

    #region SnackbarIdentifier 附加属性定义

    public static readonly DependencyProperty SnackbarIdentifierProperty =
        DependencyProperty.RegisterAttached(
            "SnackbarIdentifier",
            typeof(object),
            typeof(SnackbarIdentifierAssist),
            new PropertyMetadata(null, OnSnackbarIdentifierChanged));

    public static void SetSnackbarIdentifier(DependencyObject element, object value) =>
        element.SetValue(SnackbarIdentifierProperty, value);

    public static object? GetSnackbarIdentifier(DependencyObject element) =>
        element.GetValue(SnackbarIdentifierProperty);

    #endregion SnackbarIdentifier 附加属性定义

    #region 属性变化处理

    private static void OnSnackbarIdentifierChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not Snackbar snackbar)
            return;

        if (e.OldValue != null)
        {
            // 如果之前有绑定过Identifier，先解绑事件和移除字典
            snackbar.Loaded -= Snackbar_Loaded;
            snackbar.Unloaded -= Snackbar_Unloaded;
            // 从旧的分组中移除
            if (_snackbarGroups.TryGetValue(e.OldValue, out var list))
            {
                list.Remove(snackbar);
                if (list.Count == 0)
                    _snackbarGroups.Remove(e.OldValue);
            }
        }

        if (e.NewValue != null)
        {
            // 绑定新事件
            snackbar.Loaded += Snackbar_Loaded;
            snackbar.Unloaded += Snackbar_Unloaded;
        }
    }

    #endregion 属性变化处理

    #region 事件处理

    private static void Snackbar_Loaded(object sender, RoutedEventArgs e)
    {
        if (sender is not Snackbar snackbar)
            return;

        var identifier = GetSnackbarIdentifier(snackbar);

        if (identifier == null)
            return;

        if (!_snackbarGroups.TryGetValue(identifier, out var list))
        {
            list = [];
            _snackbarGroups[identifier] = list;
        }
        if (!list.Contains(snackbar))
            list.Add(snackbar);
    }

    private static void Snackbar_Unloaded(object sender, RoutedEventArgs e)
    {
        if (sender is not Snackbar snackbar)
            return;

        var identifier = GetSnackbarIdentifier(snackbar);
        if (identifier != null && _snackbarGroups.TryGetValue(identifier, out var list))
        {
            list.Remove(snackbar);
            if (list.Count == 0)
                _snackbarGroups.Remove(identifier);
        }

        // 解绑事件，防止内存泄漏
        snackbar.Loaded -= Snackbar_Loaded;
        snackbar.Unloaded -= Snackbar_Unloaded;
    }

    #endregion 事件处理
}