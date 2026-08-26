using MouseBaseLib;
using MouseBaseLib.Interfaces.Services;
using MouseStdLib.Providers;
using System;

namespace MouseStdLib
{
    public class ImageRandomizer : IMatrixRandomizer
    {
        public IMatrix Randomize(Size size, int? seed = null)
            => Randomize(size.Width, size.Height, seed);

        public IMatrix Randomize(int width, int height, int? seed = null)
            => Randomize(width, height, DefaultMatrixProvider.Instance, seed);

        public IMatrix Randomize(Size size, IMatrixProvider provider, int? seed = null)
            => Randomize(size.Width, size.Height, provider, seed);

        public IMatrix Randomize(int width, int height, IMatrixProvider provider, int? seed = null)
        {
            provider ??= DefaultMatrixProvider.Instance;

            IMatrix matrix = provider.Create(width, height);

            Random random = seed is not null ? new Random(seed.Value) : new Random();
            random.NextBytes(matrix.RawData.AsSpan(0, width * height));

            return matrix;
        }
    }
}
