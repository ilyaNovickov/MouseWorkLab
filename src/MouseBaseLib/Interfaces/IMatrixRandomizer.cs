using System;
using System.Collections.Generic;
using System.Text;

namespace MouseBaseLib
{
    public interface IMatrixRandomizer
    {
        IMatrix Randomize(int Width, int Height, int? seed = null);
        IMatrix Randomize(Size size, int? seed = null);
    }
}
