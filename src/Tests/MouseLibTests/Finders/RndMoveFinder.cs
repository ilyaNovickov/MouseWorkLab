using MouseBaseLib;
using MouseBaseLib.Interfaces.Services;
using MouseLibTests.Finders.Base;
using MouseStdLib;
using MouseUnsafeLib;
using MouseUnsafeLib.Finders;
using System;
using System.Collections.Generic;
using System.Text;

namespace MouseLibTests.Finders
{
    [TestFixture]
    public class RndMoveFinder
    {
        public Vector IdealMove { get; private set; }

        public int Solution => 40;

        public int PatchSize => Convert.ToInt32(0.25d * Solution);

        public int SearchRange => (Solution - PatchSize) / 2;

        public IMatrix Matrix1 { get; private set; }

        public IMatrix Matrix2 { get; private set; }

        [SetUp]
        public void Setup()
        {
            IMatrixCutter cutter = new ImageCutter();
            IMatrixRandomizer randomizer = new ImageRandomizer();

            ImageMatrix orig = (ImageMatrix)randomizer.Randomize(new Size(Solution + (int)(2d * SearchRange)));

            int dx = Random.Shared.Next(-SearchRange, +SearchRange);
            int dy = Random.Shared.Next(-SearchRange, +SearchRange);

            IdealMove = new Vector(dx, dy);

            Point position1 = new Point((orig.Width - Solution) / 2, (orig.Height - Solution) / 2);
            Point position2 = position1 + IdealMove;

            Matrix1 = cutter.Cut(orig, position1, new Size(Solution), fillZero: true);
            Matrix2 = cutter.Cut(orig, position2, new Size(Solution), fillZero: true);

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
        public void Fast()
        {
            MoveFinderFast finder = new();

            Vector vector = finder.Find(Matrix1, Matrix2, PatchSize, SearchRange);

            Assert.That(() => vector == IdealMove);

            TestContext.Out.WriteLine(" --- FoundMove --- ");
            TestContext.Out.WriteLine(vector);
            TestContext.Out.WriteLine(" --- --------- --- ");
        }

        [Test]
        public void Bounty()
        {
            MoveFinderBoundary finder = new();

            Vector vector = finder.Find(Matrix1, Matrix2, PatchSize, SearchRange);

            Assert.That(() => vector == IdealMove);

            TestContext.Out.WriteLine(" --- FoundMove --- ");
            TestContext.Out.WriteLine(vector);
            TestContext.Out.WriteLine(" --- --------- --- ");
        }

        [Test]
        public void Simd()
        {
            MoveFinderSimd finder = new();

            Vector vector = finder.Find(Matrix1, Matrix2, PatchSize, SearchRange);

            Assert.That(() => vector == IdealMove);

            TestContext.Out.WriteLine(" --- FoundMove --- ");
            TestContext.Out.WriteLine(vector);
            TestContext.Out.WriteLine(" --- --------- --- ");
        }
        [Test]
        public void SimdBounty()
        {
            MoveFinderSimdBoundary finder = new();

            Vector vector = finder.Find(Matrix1, Matrix2, PatchSize, SearchRange);

            Assert.That(() => vector == IdealMove);

            TestContext.Out.WriteLine(" --- FoundMove --- ");
            TestContext.Out.WriteLine(vector);
            TestContext.Out.WriteLine(" --- --------- --- ");
        }

        [Test]
        public void SimdParallel()
        {
            MoveFinderSimdParallel finder = new();

            Vector vector = finder.Find(Matrix1, Matrix2, PatchSize, SearchRange);

            Assert.That(() => vector == IdealMove);

            TestContext.Out.WriteLine(" --- FoundMove --- ");
            TestContext.Out.WriteLine(vector);
            TestContext.Out.WriteLine(" --- --------- --- ");
        }
        [Test]
        public void SimdBountyParallel()
        {
            MoveFinderSimdBoundaryParallel finder = new();

            Vector vector = finder.Find(Matrix1, Matrix2, PatchSize, SearchRange);

            Assert.That(() => vector == IdealMove);

            TestContext.Out.WriteLine(" --- FoundMove --- ");
            TestContext.Out.WriteLine(vector);
            TestContext.Out .WriteLine(" --- --------- --- ");
        }

    }
}
