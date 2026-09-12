using Avalonia.Controls;
using Avalonia.Platform.Storage;
using MouseLabAvalonia.Core.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MouseLabAvalonia.Core
{
    /// <summary>
    /// Сервис диалоговых окон работы с файлами (открытие и сохранение)
    /// </summary>
    public class FileDialogService : IFileDialogService
    {
        private readonly IWindowLocator windowLocator;

        /// <summary>
        /// Создание экземпляра сервиса диалоговых окон
        /// </summary>
        /// <param name="windowLocator">Локатор окон приложения</param>
        public FileDialogService(IWindowLocator windowLocator)
        {
            this.windowLocator = windowLocator;
        }

        /// <summary>
        /// Показывает диалог открытия файла и возвращает выбранный файл
        /// </summary>
        /// <param name="title">Заголовок окна</param>
        /// <param name="fileTypeChoices">Фильтр расширений файлов</param>
        /// <returns>Выбранный файл либо <c>null</c>, если выбор был отменён</returns>
        public async Task<IStorageFile?> ShowOpenFileAsync(string title, IReadOnlyList<FilePickerFileType>? fileTypeChoices = null)
        {
            var owner = windowLocator.GetRequiredWindow();

            var files = await owner.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = title,
                FileTypeFilter = fileTypeChoices,
            });

            return files.Count > 0 ? files[0] : null;
        }

        /// <summary>
        /// Показывает диалог сохранения файла и возвращает выбранный путь
        /// </summary>
        /// <param name="title">Заголовок окна</param>
        /// <param name="fileTypeChoices">Фильтр расширений файлов</param>
        /// <param name="defaultExtension">Расширение по умолчанию</param>
        /// <returns>Файл для сохранения либо <c>null</c>, если выбор был отменён</returns>
        public async Task<IStorageFile?> ShowSaveFileAsync(string title, IReadOnlyList<FilePickerFileType>? fileTypeChoices = null, string? defaultExtension = null)
        {
            var owner = windowLocator.GetRequiredWindow();

            return await owner.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = title,
                FileTypeChoices = fileTypeChoices,
                DefaultExtension = defaultExtension,
            });
        }
    }
}