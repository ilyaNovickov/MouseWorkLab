using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dock.Model.Mvvm.Controls;
using MouseBaseLib;
using MouseLabAvalonia.Core;
using MouseLabAvalonia.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace MouseLabAvalonia.ViewModels
{
    public partial class WorkSurfaceViewModel : ViewModelBase
    {
        //private readonly IModelingEnv modelingEnv;
        private readonly ModelingEnv modelingEnv;

        private const int imgWidth = 4096;
        private const int imgHeight = 4096;

        //потом удалить
        public WorkSurfaceViewModel()
        {
            modelingEnv = new ModelingEnv();
            modelingEnv.SurfaceChanged += (_, args) => this.SurfaceChanged?.Invoke(this, args);
        }

        //public WorkSurfaceViewModel(IModelingEnv env)
        //{
        //    this.modelingEnv = env;
        //}

        [ObservableProperty]
        private string? seed;

        public int SurfaceWidth 
        { 
            get; 
            private set
            {
                field = value < 10 ? 10 : value;
            }
        } = 10;

        public int SurfaceHeight 
        {
            get;
            private set
            {
                field = value < 10 ? 10 : value;
            }
        } = 10;

        public IMatrix? Surface
        {
            get => modelingEnv.Surface;
        }

        [RelayCommand]
        private void RandomizeSurface()
        {
            int seed;

            if (Seed is not null || int.TryParse(Seed, out seed))
            {
                seed = int.Parse(Seed);
            }
            else
            {
                seed = DateTime.Now.Ticks.GetHashCode();
            }
                

            modelingEnv.RandomizeSurface(seed);

            this.SurfaceWidth = modelingEnv.SurfaceWidth;
            this.SurfaceHeight = modelingEnv.SurfaceHeight;

            this.OnPropertyChanged(nameof(SurfaceWidth));
            this.OnPropertyChanged(nameof(SurfaceHeight));
        }

        public event EventHandler SurfaceChanged;


#if DEBUG
        public static WorkSurfaceViewModel Instance => new WorkSurfaceViewModel();
#endif
    }
}
