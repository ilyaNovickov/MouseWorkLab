using MouseLabAvaloniaApp.ViewModels;
using MouseLabAvaloniaApp.ViewModels.Settings;
using MouseLabAvaloniaApp.ViewModels.Welcome;
using MouseLabAvaloniaApp.Views;
using StaticViewLocator;
using Avalonia;
using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;

namespace MouseLabAvaloniaApp;

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
[StaticViewMapping(typeof(SettingsWindowViewModel), typeof(SettingsWindow))]
[StaticViewMapping(typeof(AppSettingsViewModel), typeof(AppSettingsView))]
[StaticViewMapping(typeof(WelcomeWindowViewModel), typeof(WelcomeWindow))]
public partial class ViewLocator : IDataTemplate
{
    private readonly IServiceProvider _provider;

    public ViewLocator(IServiceProvider provider)
    {
        _provider = provider;
    }

    public Control? Build(object? data)
    {
        if (data is null)
            return null;

        var type = data.GetType();
        if (s_views.TryGetValue(type, out var func))
            return func.Invoke();

        throw new Exception($"Unable to create view for type: {type}");
    }

    public bool Match(object? data)
    {
        if (data is null)
        {
            return false;
        }

        var type = data.GetType();
        return s_views.ContainsKey(type);
    }
}
