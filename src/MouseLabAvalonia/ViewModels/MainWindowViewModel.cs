using CommunityToolkit.Mvvm.Input;
using Dock.Model.Controls;
using Dock.Model.Core;
using Dock.Model.Mvvm;
using MouseLabAvalonia.Core.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace MouseLabAvalonia.ViewModels
{

    public partial class MainWindowViewModel : ViewModelBase
    {
        private IRootDock? _layout;
        private readonly IDockFactory dockFactory;

        private readonly IViewModelFactory vmFactory;

        public MainWindowViewModel(IDockFactory dockFactory, IViewModelFactory viewModelFactory)
        {
            this.vmFactory = viewModelFactory;
            this.dockFactory = dockFactory;

            this.PlotViewModel = viewModelFactory.PlotViewModel;

            var layout = dockFactory.CreateLayout();

            if (layout is null) 
                throw new System.Exception("Не найден макет");

            dockFactory.InitLayout(layout);
            Layout = layout;

            MenuItems.Add(new MenuListViewModel("Графики", "PlotViewModelDocument", dockFactory));
        }

        public ObservableCollection<MenuListViewModel> MenuItems { get; } = new();

        public PlotViewModel PlotViewModel { get; private set; }

        /// <summary>
        /// Макет Dock элементов
        /// </summary>
        public IRootDock? Layout
        {
            get => _layout;
            set => SetProperty(ref _layout, value);
        }

        /// <summary>
        /// Сброс макета до значений по умолчанию
        /// </summary>
        public void ResetLayout()
        {
            if (Layout is not null)
            {
                if (Layout.Close.CanExecute(null))
                {
                    Layout.Close.Execute(null);
                }
            }

            var layout = dockFactory.CreateLayout();
            if (layout is not null)
            {
                dockFactory.InitLayout(layout);
                Layout = layout;
            }
        }

        /// <summary>
        /// Закрытие Dock макета
        /// </summary>
        public void CloseLayout()
        {
            if (Layout is IDock dock)
            {
                if (dock.Close.CanExecute(null))
                {
                    dock.Close.Execute(null);
                }
            }
        }

        public async Task<bool> ConfirmCloseAsync()
        {
            var task = await vmFactory.MessageDialogService.ShowOkAbortAsync("Вы уверены, что хотиче закрыть окно?" + Environment.NewLine +
                "Закрытие главного окна приведёт к закрытию главного", "Внимание", 
                MsBox.Avalonia.Enums.Icon.Question);

            return task;
        }
#if DEBUG
        /// <summary>
        /// Экземпляр ViewModel для отладки
        /// </summary>
        public static MainWindowViewModel Instance 
        {
            get
            {
                var vmFactory = new ViewModelFactory();

                return new MainWindowViewModel(new DockFactory(vmFactory), vmFactory);
            }
        }
#endif
    }
}