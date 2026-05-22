using Microsoft.ApplicationInsights;
using MouseBaseLib;
using MouseStdLib;
using System.Diagnostics.Metrics;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace MouseLibTests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            StdMoveFinder finder = new();

            byte[] m1 = new byte[]
            {
                0,   0,   0,   0, 0,
                0,   0, 255,   0, 0,
                0, 255, 255, 255, 0,
                0,   0, 255,   0, 0,
                0,   0,   0,   0, 0,
            };

            byte[] m2 = new byte[]
            {
                  0, 255,   0, 0, 0,
                255, 255, 255, 0, 0,
                  0, 255,   0, 0, 0,
                  0,   0,   0, 0, 0,
                  0,   0,   0, 0, 0,
            };

            //byte[] m2 = new byte[]
            //{
            //    0,   0, 255,   0, 0,
            //    0,   0,   0,   0, 0,
            //    0,   0,   0,   0, 0,
            //    0,   0,   0,   0, 0,
            //    0,   0,   0,   0, 0,
            //};

            IMatrix matrix1 = new ImageMatrix(m1, 5, 5);
            IMatrix matrix2 = new ImageMatrix(m2, 5, 5);

            int p = 3;
            int s = 3;

            //Vector v = finder.Find(matrix1, matrix2, p, s);
            //Vector v = finder.OptFind(matrix1, matrix2, p, s);
            //Vector v = finder.FillFind(matrix1, matrix2, p, s);
            Vector v2 = finder.FindSimd(matrix1, matrix2, p, s);

            Assert.Pass();
        }

        [Test]
        public void Test2()
        {
            int R = 40;
            int p = 16;
            int s = (R - p) / 2;

            IMatrix src = (new ImageRandomizer()).Randomize(R + 2 * s, R + 2 * s);

            Point position = new Point((src.Width - R) / 2, (src.Height - R) / 2);

            ImageCutter cutter = new();

            MoveFinderFast f1 = new();
            MoveFinderBoundy f2 = new();

            Random random = new();

            Vector vector = new Vector();
            vector.Dx = random.Next(-s, +s);
            vector.Dy = random.Next(-s, +s);

            IMatrix m1 = cutter.Cut(src, position, new Size(R));
            IMatrix m2 = cutter.Cut(src, position + vector, new Size(R));

            Vector v1 = f1.Find(m1, m2, p, s);
            Vector v2 = f2.Find(m1, m2, p, s);

            Assert.Pass();
        }
    }
}
