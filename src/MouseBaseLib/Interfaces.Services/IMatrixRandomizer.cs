using System;
using System.Collections.Generic;
using System.Text;

namespace MouseBaseLib.Interfaces.Services
{
    public interface IMatrixRandomizer
    {
        IMatrix Randomize(int Width, int Height, int? seed = null);
        IMatrix Randomize(Size size, int? seed = null);
    }
}
