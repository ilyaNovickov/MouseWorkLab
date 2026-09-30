using MouseBaseLib;
using MouseBaseLib.Interfaces.Services;
using MouseLabAvalonia.Core.Interfaces;
using MouseStdLib;
using MouseStdLib.Providers;
using System;
using System.Collections.Generic;
using System.Text;

namespace MouseLabAvalonia.Core
{
    public class ModelingEnv : IModelingEnv
    {
        //delete
        public ModelingEnv() 
        {
            this.ImageRandomizer = new ImageRandomizer();
            this.Settings = new ModelingSettingsService();

            this.SurfaceWidth = 100;
            this.SurfaceHeight = 100;
        }

        public ModelingEnv(IMatrixRandomizer randomizer, IModelingSettings settings)
        {
            this.ImageRandomizer = randomizer;
            this.Settings = settings;
        }

        private bool SurfaceNeedsToUpdate { get; set; }

        public int SurfaceWidth
        {
            get;
            set
            {
                if (value <= 0)
                    throw new Exception("Значение не может быть отричательным");
                field = value;
                SurfaceNeedsToUpdate = true;
            }
        }

        public int SurfaceHeight
        {
            get;
            set
            {
                if (value <= 0)
                    throw new Exception("Значение не может быть отричательным");
                field = value;
                SurfaceNeedsToUpdate = true;
            }
        }

        public int? Seed
        {
            get;
            private set;
        }

        public IMatrix? Surface
        {
            get;
            private set;
        }

        public IModelingSettings Settings
        {
            get;
            private set;
        }

        public void RandomizeSurface(int? seed = null)
        {
            this.Seed = seed;

            if (Surface is null || SurfaceNeedsToUpdate)
            {
                Surface?.Dispose();

                IMatrixProvider provider = DefaultMatrixProvider.Instance;

                this.Surface = provider.Create(this.SurfaceWidth, this.SurfaceHeight);
            }
            

            this.ImageRandomizer.Randomize(Surface, seed);

            SurfaceChanged?.Invoke(this, EventArgs.Empty);
        }
        public IMatrixRandomizer ImageRandomizer { get; private set; }

        public event EventHandler? SurfaceChanged;
    }
}
/*
 Подумать над настройками
 */