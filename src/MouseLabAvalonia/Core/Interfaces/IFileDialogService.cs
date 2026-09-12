using Avalonia.Platform.Storage;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MouseLabAvalonia.Core.Interfaces
{
    /// <summary>
    /// Сервис диалогового окна работы с файлами
    /// </summary>
    public interface IFileDialogService
    {
        /// <summary>
        /// Открытие диалогового окна открытия файла
        /// </summary>
        /// <param name="title">Заголовок окна</param>
        /// <param name="fileTypeChoices">Фильтр расширений файлов</param>
        /// <returns></returns>
        Task<IStorageFile?> ShowOpenFileAsync(string title, IReadOnlyList<FilePickerFileType>? fileTypeChoices = null);

        /// <summary>
        /// Открытие диалогового окна сохранения файла
        /// </summary>
        /// <param name="title">Заголовок окна</param>
        /// <param name="fileTypeChoices">Фильтр расширений файлов</param>
        /// <param name="defaultExtension">Расширение по умолчанию</param>
        /// <returns></returns>
        Task<IStorageFile?> ShowSaveFileAsync(string title, IReadOnlyList<FilePickerFileType>? fileTypeChoices = null, string? defaultExtension = null);
    }
}