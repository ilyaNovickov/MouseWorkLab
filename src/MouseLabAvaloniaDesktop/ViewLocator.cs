using System;
using System.Diagnostics.CodeAnalysis;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Dock.Model.Core;
using MouseLabAvaloniaDesktop.ViewModels;
using StaticViewLocator;

namespace MouseLabAvaloniaDesktop;

/// <summary>
/// Given a view model, returns the corresponding view if possible.
/// </summary>
/// <summary>
/// Сопоставляет модель представления с представлением, которое должно её отрисовать
/// внутри <c>ContentControl</c>.
/// </summary>
/// <remarks>
/// <para>
/// Разрешение выполняется таблицей <c>s_views</c>, которую генерирует
/// <c>StaticViewLocator</c> на этапе сборки: это <c>Dictionary&lt;Type, Func&lt;Control&gt;&gt;</c>
/// с готовыми делегатами, то есть обычный поиск по типу. Рефлексии в рантайме нет,
/// поэтому триммер не может вырезать конструкторы представлений, а
/// <c>PublishAot=true</c> остаётся безопасным.
/// </para>
/// <para>
/// Собственные <c>Build</c> и <c>Match</c> удалены намеренно: при
/// <c>GenerateIDataTemplate = true</c> генератор создаёт их сам вместе с реализацией
/// <c>IDataTemplate</c>. Написать их вручную - значит вернуться к пути, который
/// обходит сгенерированную таблицу.
/// </para>
/// <para>
/// Именование по умолчанию: пространство <c>ViewModels</c> заменяется на
/// <c>Views</c>, суффикс <c>ViewModel</c> - на <c>View</c>. В этом проекте
/// согласуется ни одна из трёх моделей, поэтому у всех явные атрибуты:
/// </para>
/// <list type="bullet">
/// <item><description>
/// <c>SettingsWindowViewModel</c> - согласуемое имя дало бы несуществующий
/// <c>Views.Settings.SettingsWindowView</c>.
/// </description></item>
/// <item><description>
/// <c>AppSettingsViewModel</c> - по папкам представление лежит в
/// <c>Views\Settings\</c>, но объявлено в пространстве <c>MouseLabAvaloniaApp.Views</c>
/// (см. несоответствие папок и пространств имён). Согласование ищет
/// <c>Views.Settings.AppSettingsView</c>, которого нет.
/// </description></item>
/// <item><description>
/// <c>MainWindowViewModel</c> - сопоставления нет намеренно, см. ниже.
/// </description></item>
/// </list>
/// <para>
/// <see cref="Dock.Model.Core.IDockable"/> разворачивается до <c>Context</c>.
/// <c>DockControl</c> показывает содержимое вкладки не по <c>Context</c>, а по
/// самому <c>IDockable</c>: в <c>DocumentControl</c> стоит
/// <c>DockableControl DataContext="{Binding ActiveDockable}"</c>, и уже у него
/// <c>DeferredContentControl Content="{Binding}"</c>. Поэтому в <c>Build</c>
/// приходит объект дока, а не модель представления. Здесь <c>Context</c>
/// достаётся вручную, а <c>DataContext</c> результата выставляется явно —
/// иначе представление унаследовало бы контекст дока.
/// </para>
/// <para>
/// Если новое представление не подхватывается, проверьте <c>s_views</c> в
/// <c>obj/gen/StaticViewLocator/.../ViewLocator_StaticViewLocator.cs</c>: там видно,
/// что ушло в таблицу, а что в <c>s_missingViews</c> с готовым объяснением.
/// </para>
/// <para>
/// <c>MainWindowViewModel</c> намеренно не сопоставлен: главное окно создаётся
/// напрямую в <c>App.OnFrameworkInitializationCompleted</c> и назначается в
/// <c>desktop.MainWindow</c>. Сопоставление рисковало бы попытаться вложить окно в
/// само себя. Он попадает в <c>s_missingViews</c>, и сгенерированный
/// <c>Match</c> для него возвращает <c>false</c> - то есть DataTemplate его не
/// подхватывает вообще.
/// </para>
/// </remarks>
[StaticViewLocator(
    GenerateIDataTemplate = true,
    GenerateRuntimeTypeFallbackMethods = false,
    DataTemplateMatchTypes = new[] { typeof(ViewModelBase) })]
public partial class ViewLocator : IDataTemplate
{

    public Control? Build(object? data)
    {
        if (data is null)
            return null;

        // Dock передаёт сюда сам IDockable, а не модель представления из его
        // Context. Разворачиваем до Context и строим представление уже для него.
        if (data is IDockable { Context: not null } dockable)
            return BuildFor(dockable.Context, data);

        return BuildFor(data, data);
    }

    public bool Match(object? data)
    {
        if (data is null)
        {
            return false;
        }

        return MatchTarget(data is IDockable { Context: not null } dockable
            ? dockable.Context
            : data);
    }

    private static bool MatchTarget(object? target) => target is not null && s_views.ContainsKey(target.GetType());

    private Control? BuildFor(object? target, object fallbackData)
    {
        if (target is null)
            return null;

        if (!s_views.TryGetValue(target.GetType(), out var func))
            throw new Exception($"Unable to create view for type: {fallbackData.GetType()}");

        Control? control = func.Invoke();

        // DataContext выставляем явно: без этого представление унаследовало бы
        // контекст дока, а не модель представления из Context.
        if (control is not null && !ReferenceEquals(target, fallbackData))
            control.DataContext = target;

        return control;
    }
}
