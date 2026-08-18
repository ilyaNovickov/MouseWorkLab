using Avalonia.Platform.Storage;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MouseLabAvalonia.Core.Interfaces
{
    public interface IFileDialogService
    {
        Task<IStorageFile?> ShowOpenFileAsync(string title, IReadOnlyList<FilePickerFileType>? fileTypeChoices = null);
        Task<IStorageFile?> ShowSaveFileAsync(string title, IReadOnlyList<FilePickerFileType>? fileTypeChoices = null, string? defaultExtension = null);
    }
}