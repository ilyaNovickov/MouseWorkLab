namespace MouseLab.Matcher.Tests
{
    internal static class TestImageFactory
    {
        /// <summary>Белошумный «донор»: исходные байты случайные, совпадений почти нет.</summary>
        public static byte[] Noise(int width, int height, int seed)
        {
            var data = new byte[width * height];
            new Random(seed).NextBytes(data);
            return data;
        }

        /// <summary>
        /// Фрагмент из донора: аналог IMatrixCutter, точная копия области.
        /// </summary>
        public static TestMatrix Cut(byte[] donor, int donorWidth, int x, int y, int width, int height)
        {
            var data = new byte[width * height];

            for (int row = 0; row < height; row++)
            {
                Array.Copy(
                    donor,
                    (long)(y + row) * donorWidth + x,
                    data,
                    (long)row * width,
                    width);
            }

            return new TestMatrix(data, width, height);
        }
    }
}
