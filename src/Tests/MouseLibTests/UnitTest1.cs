using Microsoft.ApplicationInsights;
using MouseBaseLib;
using MouseStdLib;
using MouseUnsafeLib;
using MouseUnsafeLib.Finders;
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
            byte[] m1 = new byte[]
            {
                0,   0,   0,   0, 0,
                0,   0, 255,   0, 0,
                0, 255, 255, 255, 0,
                0,   0, 255,   0, 0,
                0,   0,   0,   0, 0,
            };

            //byte[] m2 = new byte[]
            //{
            //      0, 255,   0, 0, 0,
            //    255, 255, 255, 0, 0,
            //      0, 255,   0, 0, 0,
            //      0,   0,   0, 0, 0,
            //      0,   0,   0, 0, 0,
            //};

            byte[] m2 = new byte[]
            {
                0,   0, 255,   0, 0,
                0,   0,   0,   0, 0,
                0,   0,   0,   0, 0,
                0,   0,   0,   0, 0,
                0,   0,   0,   0, 0,
            };

            IMatrix matrix1 = new ImageMatrix(m1, 5, 5);
            IMatrix matrix2 = new ImageMatrix(m2, 5, 5);

            int p = 3;
            int s = 3;

            MoveFinderFast f1 = new MoveFinderFast();
            MoveFinderBoundary f2 = new();
            MoveFinderSimd f3 = new();
            MoveFinderSimdParallel f4 = new();
            MoveFinderSimdBoundary f5 = new();
            MoveFinderSimdBoundaryParallel f6 = new();

            Vector v1 = f1.Find(matrix1, matrix2, p, s);
            Vector v2 = f2.Find(matrix1, matrix2, p, s);
            Vector v3 = f3.Find(matrix1, matrix2, p, s);
            Vector v4 = f4.Find(matrix1, matrix2, p, s);
            Vector v5 = f5.Find(matrix1, matrix2, p, s);
            Vector v6 = f6.Find(matrix1, matrix2, p, s);

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

            MoveFinderFast f1 = new MoveFinderFast();
            MoveFinderBoundary f2 = new();
            MoveFinderSimd f3 = new();
            MoveFinderSimdParallel f4 = new();
            MoveFinderSimdBoundary f5 = new();
            MoveFinderSimdBoundaryParallel f6 = new();

            Random random = new();

            Vector vector = new Vector();
            vector.Dx = random.Next(-s, +s);
            vector.Dy = random.Next(-s, +s);

            IMatrix matrix1 = cutter.Cut(src, position, new Size(R));
            IMatrix matrix2 = cutter.Cut(src, position + vector, new Size(R));

            Vector v1 = f1.Find(matrix1, matrix2, p, s);
            Vector v2 = f2.Find(matrix1, matrix2, p, s);
            Vector v3 = f3.Find(matrix1, matrix2, p, s);
            Vector v4 = f4.Find(matrix1, matrix2, p, s);
            Vector v5 = f5.Find(matrix1, matrix2, p, s);
            Vector v6 = f6.Find(matrix1, matrix2, p, s);

            Assert.Pass();
        }
    }
}
