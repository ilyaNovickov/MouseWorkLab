using CommunityToolkit.Mvvm.Input;
using MouseLabAvaloniaDesktop.Services.Abstractions;
using MouseLabAvaloniaDesktop.Services.Implementations;
using ProTranslate;
using System.Threading.Tasks;

namespace MouseLabAvaloniaDesktop.ViewModels
{
    public partial class MenuViewModel : TranslatedViewModelBase
    {
        private readonly IDialogService _dialogService;
        private readonly ProjectFileService _projectFileService;

        public MenuViewModel(ITranslationService translation,
            IDialogService dialogService,
            ProjectFileService projectFileService) : base(translation)
        {
            _dialogService = dialogService;
            _projectFileService = projectFileService;
        }

        [RelayCommand]
        private Task CreateProject() =>
            _projectFileService.CreateAsync(
                Strings.FileDialogSaveProjectTitle, Strings.FileDialogJsonFilter);

        [RelayCommand]
        private Task SaveProject() =>
            _projectFileService.SaveAsync(
                Strings.FileDialogSaveProjectTitle, Strings.FileDialogJsonFilter);

        [RelayCommand]
        private Task LoadProject() =>
            _projectFileService.LoadAsync(Strings.FileDialogOpenProjectTitle,
                Strings.FileDialogJsonFilter);

        [RelayCommand]
        private void ShowSettings() => _dialogService.Show<SettingsWindowViewModel>();
    }
}
