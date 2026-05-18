using MouseBaseLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace MouseStdLib
{
    public class ImageRandomizer : IMatrixRandomizer
    {
        public IMatrix Randomize(Size size, int? seed = null)
        {
            return Randomize(size.Width, size.Height, seed);
        }

        public IMatrix Randomize(int width, int height, int? seed = null)
        {
            Random random;

            if (seed is not null)
                random = new Random(seed.Value);
            else
                random = new Random();

            byte[] buffer = new byte[width * height];

            random.NextBytes(buffer);

            return  new ImageMatrix(buffer, width, height);
        }
    }
}
