using Avalonia.Platform.Storage;
using System.IO;
using System.Threading.Tasks;

namespace MouseLabAvaloniaDesktop.Services.Implementations
{
    // Единая точка работы с файлом проекта: выбор пути, проверка расширения,
    // сохранение и загрузка. Раньше этот поток был в MenuViewModel, а
    // SettingsWindowViewModel вызывал его команды напрямую.
    public class ProjectFileService
    {
        private readonly FileDialogService _fileDialogService;
        private readonly ProjectService _projectService;

        public ProjectFileService(FileDialogService fileDialogService, ProjectService projectService)
        {
            _fileDialogService = fileDialogService;
            _projectService = projectService;
        }

        // Всегда спрашивает путь: для команды «создать проект».
        public async Task CreateAsync(string title, string filterName)
        {
            var path = await PickSavePathAsync(title, filterName);

            if (path is null)
                return;

            _projectService.SaveTo(path);
        }

        // Сохраняет в текущий файл проекта; если проекта ещё нет — предлагает создать.
        public async Task SaveAsync(string title, string filterName)
        {
            if (File.Exists(_projectService.CurrentFilePath))
            {
                _projectService.Save();
                return;
            }

            await CreateAsync(title, filterName);
        }

        public async Task LoadAsync(string title, string filterName)
        {
            var file = await _fileDialogService.OpenFileAsync(title, [CreateJsonFilter(filterName)]);

            if (file is null)
                return;

            var path = file.Path.LocalPath;

            if (Path.GetExtension(path) != ".json")
                return;

            if (!File.Exists(path))
                return;

            _projectService.Load(path);
        }

        private async Task<string?> PickSavePathAsync(string title, string filterName)
        {
            var file = await _fileDialogService.SaveFileAsync(title, [CreateJsonFilter(filterName)]);

            if (file is null)
                return null;

            var path = file.Path.LocalPath;

            if (Path.GetExtension(path) != ".json")
                return null;

            // ProjectService.SaveProject требует существующий файл.
            File.Create(path).Close();

            return path;
        }

        private static FilePickerFileType CreateJsonFilter(string name) =>
            new(name) { Patterns = ["*.json"] };
    }
}
