using MouseLab.Core;
using MouseLab.Core.Models;
using MouseLab.Core.Providers;

namespace MouseLab.Core.Services
{
    public interface IMatrixRandomizer
    {
        void Randomize(Span<byte> dest, int? seed = null);

        void Randomize(IMatrix dest, int? seed = null);

        IMatrix Randomize(int width, int height, int? seed = null);

        IMatrix Randomize(Size size, int? seed = null);

        IMatrix Randomize(int width, int height, IMatrixProvider provider, int? seed = null);

        IMatrix Randomize(Size size, IMatrixProvider provider, int? seed = null);
    }
}
