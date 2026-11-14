namespace MouseLabAvaloniaDesktop.ViewModels;

/// <summary>
/// Секция окна настроек. Save публикует рабочую копию в сервис настроек,
/// Reset откатывает рабочую копию к значениям, применённым сервисом, — так
/// Cancel отменяет только несохранённые правки, а уже сохранённые остаются.
/// </summary>
public interface ISettingsSectionViewModel
{
    void Save();

    void Reset();
}
