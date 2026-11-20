using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MouseLabAvaloniaDesktop.Services.Implementations
{
    /*
    public abstract class FileDialogInfo
    {
        public string? Title { get; set; }
    }

    public class OpenFileDialogInfo : FileDialogInfo
    {

        public bool AllowMultiple { get; set; }

        public string? SuggestedFileName { get; set; }
    }
    */

    public class FileDialogService
    {
        //private readonly Window _target;

        //public FileDialogService(Window target)
        //{
        //    _target = target;
        //}

        public async Task<IStorageFile?> OpenFileAsync(string title,
            IReadOnlyList<FilePickerFileType>? filePickerFiles = null)
        {
            var files = await GetWindow().StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions()
            {
                Title = title,
                AllowMultiple = false,
                FileTypeFilter = filePickerFiles
            });

            return files.Count >= 1 ? files[0] : null;
        }

        public async Task<IStorageFile?> SaveFileAsync(string title,
            IReadOnlyList<FilePickerFileType>? filePickerFiles = null)
        {
            return await GetWindow().StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions()
            {
                Title = title,
                ShowOverwritePrompt = true,
                FileTypeChoices = filePickerFiles
            });
        }

        private static Window GetWindow()
        {
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                return desktop.MainWindow!;
            }
            return null!;
        }
    }
}
