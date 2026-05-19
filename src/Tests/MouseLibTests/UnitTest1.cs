using Microsoft.ApplicationInsights;
using MouseBaseLib;
using MouseStdLib;
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
            byte[,] src = new byte[,]
            {
                { 0, 1, 2, 3, 4, 5 },
                { 6, 7, 8, 9, 10, 11 },
                { 12, 13, 14, 15, 16, 17 }
            };

            IMatrix first = new ImageMatrix(src);

            IMatrixCutter cut = new ImageCutter();

            //IMatrix sec = cut.Cut(first, 0, 0, 4, 3);
            //IMatrix sec1 = cut.Cut(first, 1, 1, 4, 3);
            IMatrix sec2 = cut.Cut(first, 10, 10, 4, 3);

            Assert.Pass();
        }
    }
}
