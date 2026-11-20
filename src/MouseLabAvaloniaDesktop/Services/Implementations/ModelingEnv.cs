using MouseLabAvaloniaDesktop.Models;
using MouseLabAvaloniaDesktop.Models.Settings;
using MouseLabAvaloniaDesktop.Services.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace MouseLabAvaloniaDesktop.Services.Implementations
{
    public class ModelingEnv
    {
        private readonly IMatrixRandomizer _matrixRandomizer;
        private Matrix _worktop;

        public ModelingEnv(IMatrixRandomizer matrixRandomizer)
        {
            //_worktop = new Matrix(ModelingSettings.DefaultWorktopResolution, ModelingSettings.DefaultWorktopResolution);
            _worktop = new Matrix(512, 512);
            _matrixRandomizer = matrixRandomizer;
        }

        public IReadOnlyMatrix Worktop { get => _worktop; }

        public int? Seed { get; private set; }

        public void GenerateNewWorktop(int resolution, int? seed = null)
        {
            if (seed is null)
            {
                seed = Random.Shared.Next();
            }
            Seed = seed;
            _matrixRandomizer.Randomize(_worktop.SpanBytes, Seed);
        }


    }
}
