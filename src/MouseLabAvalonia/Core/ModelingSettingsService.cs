using MouseLabAvalonia.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace MouseLabAvalonia.Core
{
    public class ModelingSettingsService : IModelingSettings
    {
        public int SurfaceWidth { get; set; }

        public int SurfaceHeight { get; set; }
    }
}
