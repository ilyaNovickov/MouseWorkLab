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
            IMatrix m = new ImageMatrix(new byte[,]
            {
                { 1, 2, 3 },
                { 4, 5, 6 },
                { 7, 8, 9 }
            });

            byte[] matrix = [1, 2, 3];

            var x = Unsafe.Add<byte[]>(ref matrix, 2);


            Assert.Pass();
        }
    }
}
