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
        public ModelingEnv() 
        {
            this.ImageRandomizer = new ImageRandomizer();

            this.SurfaceWidth = 100;
            this.SurfaceHeight = 100;
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

        public event EventHandler SurfaceChanged;
    }
}
/*
 Исправить вылед при генерации шума (продумать проброс его в View)
 Подумать над настройками
 */