namespace MouseLabAvaloniaApp.Services.AppSettings;

/// <summary>
/// Настройки окна приветствия: живут только в памяти и никогда не пишутся в файл.
/// </summary>
/// <remarks>
/// <para>
/// Заполняется пустым снимком намеренно: у каждого пользователя будут свои
/// настройки, поэтому окно приветствия стартует с умолчаний, а не из
/// settings.json. Не «исправляйте» это на <c>new AppSettingsSnapshot(snapshot)</c>
/// - так окно снова начнёт подхватывать чужие настройки.
/// </para>
/// <para>
/// Постоянные настройки заполняются из этого хранилища один раз, в момент
/// перехода к главному окну (см. <see cref="AppSettingsServiceBase.LoadFrom"/>).
/// </para>
/// </remarks>
public sealed class TemporaryAppSettingsService : AppSettingsServiceBase
{
    public TemporaryAppSettingsService() : base(new AppSettingsSnapshot())
    {
    }
}