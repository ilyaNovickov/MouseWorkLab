using MouseBaseLib;
using MouseBaseLib.Interfaces.Services;
using MouseStdLib;
using MouseUnsafeLib;
using MouseUnsafeLib.Finders;
using System;
using System.Collections.Generic;
using System.Text;

namespace MouseLibTests.Finders.Base
{
    [TestFixture]
    public abstract class MoveFinderTest
    {
        public abstract byte[] Image1 { get; }


        public abstract Vector IdealMove { get; }

        public abstract int Solution { get; }
        public abstract int PatchSize { get; }
        public abstract int SearchRange { get; }

        protected IMatrix Matrix1 { get; private set; }

        protected IMatrix Matrix2 { get; private set; }

        [SetUp]
        public void Setup()
        {
            Matrix1 = new ImageMatrix(Image1, Solution, Solution);


            IMatrixCutter cutter = new ImageCutter();

            //Point position = new Point((Solution - PatchSize) / 2, (Solution - PatchSize) / 2);
            //position = position + IdealMove;

            //Matrix2 = cutter.Cut(Matrix1, position, new Size(Solution), fillZero : true);
            Matrix2 = cutter.Cut(Matrix1, Point.Zero + IdealMove, new Size(Solution), fillZero: true);

            TestContext.Out.WriteLine(" --- Matrix1 --- ");
            TestContext.Out.WriteLine(((ImageMatrix)Matrix1).ToString("d"));
            TestContext.Out.WriteLine(" --- ------- --- ");
            TestContext.Out.WriteLine(" --- Matrix2 --- ");
            TestContext.Out.WriteLine(((ImageMatrix)Matrix2).ToString("d"));
            TestContext.Out.WriteLine(" --- ------- --- ");
            TestContext.Out.WriteLine(" --- IdelVector --- ");
            TestContext.Out.WriteLine(IdealMove);
            TestContext.Out.WriteLine(" --- ---------- --- ");
        }

        [TearDown]
        public void TearDown()
        {
            Matrix1?.Dispose();
            Matrix2?.Dispose();
        }

        [Test]
        public abstract void Fast();

        [Test]
        public abstract void Bounty();

        [Test]
        public abstract void Simd();
        [Test]
        public abstract void SimdBounty();
        [Test]
        public abstract void SimdParallel();
        [Test]
        public abstract void SimdBountyParallel();
    }
}
