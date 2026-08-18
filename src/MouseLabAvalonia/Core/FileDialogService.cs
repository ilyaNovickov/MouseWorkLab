using Avalonia.Controls;
using Avalonia.Platform.Storage;
using MouseLabAvalonia.Core.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MouseLabAvalonia.Core
{
    public class FileDialogService : IFileDialogService
    {
        private readonly IWindowLocator windowLocator;

        public FileDialogService(IWindowLocator windowLocator)
        {
            this.windowLocator = windowLocator;
        }

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