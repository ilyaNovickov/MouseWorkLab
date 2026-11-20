using CommunityToolkit.Mvvm.Input;
using MouseLabAvaloniaDesktop.Services.Abstractions;
using MouseLabAvaloniaDesktop.Services.Implementations;
using ProTranslate;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;

namespace MouseLabAvaloniaDesktop.ViewModels
{
    public partial class MenuViewModel : TranslatedViewModelBase
    {
        private readonly IDialogService _dialogService;
        private readonly FileDialogService _fileDialogService;
        private readonly ProjectService _projectService;

        public MenuViewModel(ITranslationService translation,
            IDialogService dialogService,
            FileDialogService fileDialogService,
            ProjectService projectService) : base(translation)
        {
            _dialogService = dialogService;
            _fileDialogService = fileDialogService;
            _projectService = projectService;
        }

        [RelayCommand]
        private async Task CreateProject()
        {
            var file = await GetProjectFilePathAsync();

            if (file is null)
                return;

            _projectService.SaveTo(file);
        }

        [RelayCommand]
        private async Task SaveProject()
        {
            if (!File.Exists(_projectService.CurrentFilePath))
            {
                var file = await GetProjectFilePathAsync();
                if (file is null)
                    return;
                _projectService.SaveTo(file);
            }
            else
            {
                _projectService.Save();
            }
        }

        private async Task<string?> GetProjectFilePathAsync()
        {
            var file = await _fileDialogService.SaveFileAsync([FilePickerFileTypes.Json]);

            if (file is null)
                return null;

            if (Path.GetExtension(file.Path.LocalPath) != ".json")
                return null;

            // gnu/linux не поддерживает кириллические пути ?
            File.Create(file.Path.AbsolutePath).Close();

            return file.Path.AbsolutePath;
        }

        /*
               < MenuItem Header="Загрузить проект"/>*/

        [RelayCommand]
        private async Task LoadProject()
        {
            var file = await _fileDialogService.OpenFileAsync([FilePickerFileTypes.Json]);

            if (file is null)
                return;

            if (Path.GetExtension(file.Path.LocalPath) != ".json")
                return;

            if (!File.Exists(file.Path.AbsolutePath))
                return;

            _projectService.Load(file.Path.AbsolutePath);
        }

        [RelayCommand]
        private void ShowSettings() => _dialogService.Show<SettingsWindowViewModel>();
    }
}
