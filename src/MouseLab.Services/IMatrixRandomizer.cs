using MouseLab.Core;
using MouseLab.Core.Models;
using MouseLab.Core.Providers;

namespace MouseLab.Services
{
    public interface IMatrixRandomizer
    {
        void Randomize(IMatrix dest, int? seed = null);

        IMatrix Randomize(int width, int height, int? seed = null);

        IMatrix Randomize(int Width, int Height, IMatrixProvider provider, int? seed = null);

        IMatrix Randomize(Size size, IMatrixProvider provider, int? seed = null)
        {
            return Randomize(size.Width, size.Height, provider, seed);
        }

        IMatrix Randomize(Size size, int? seed = null)
        {
            return Randomize(size.Width, size.Height, seed);
        }     
    }
}
