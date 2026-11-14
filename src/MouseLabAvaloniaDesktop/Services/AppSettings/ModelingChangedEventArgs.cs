using System;

namespace MouseLabAvaloniaDesktop.Services.AppSettings;

/// <summary>Аргументы события смены настроек моделирования.</summary>
public class ModelingChangedEventArgs : EventArgs
{
    public ModelingChangedEventArgs(int worktopResolution, bool experimental)
    {
        WorktopResolution = worktopResolution;
        Experimental = experimental;
    }

    /// <summary>Разрешение полотна, выбранное в настройках.</summary>
    public int WorktopResolution { get; }

    /// <summary>
    /// Экспериментальные настройки. Пока Core-модель их не применяет
    /// (<c>ModelingSettings.ExperimentalSettings</c> закомментирована), значение
    /// живёт только на уровне настроек.
    /// </summary>
    public bool Experimental { get; }
}
