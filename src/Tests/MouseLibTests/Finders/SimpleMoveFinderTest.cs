using MouseStdLib;
using System;
using System.Collections.Generic;
using System.Text;
using MouseLibTests.Finders.Base;
using MouseBaseLib;
using MouseUnsafeLib;
using MouseUnsafeLib.Finders;

namespace MouseLibTests.Finders
{
    [TestFixture]
    public class SimpleMoveFinderTest : MoveFinderTest
    {
        public override byte[] Image1 => new byte[]
            {
                0,   0,   0,   0, 0,
                0,   0, 255,   0, 0,
                0, 255, 255, 255, 0,
                0,   0, 255,   0, 0,
                0,   0,   0,   0, 0,
            };

        public override Vector IdealMove { get; } = new Vector(-1, -1);

        public override int Solution => 5;

        public override int PatchSize => 3;

        public override int SearchRange => (Solution - PatchSize) / 2;

        [Test]
        public override void Fast()
        {
            MoveFinderFast finder = new();

            Vector vector = finder.Find(Matrix1, Matrix2, PatchSize, SearchRange);

            Assert.That(() => vector == IdealMove);
        }

        [Test]
        public override void Bounty()
        {
            MoveFinderBoundy finder = new();

            Vector vector = finder.Find(Matrix1, Matrix2, PatchSize, SearchRange);

            Assert.That(() => vector == IdealMove);
        }
        [Test]
        public override void Simd()
        {
            MoveFinderSimd finder = new();

            Vector vector = finder.Find(Matrix1, Matrix2, PatchSize, SearchRange);

            Assert.That(() => vector == IdealMove);
        }
        [Test]
        public override void SimdBounty()
        {
            MoveFinderSimdBounty finder = new();

            Vector vector = finder.Find(Matrix1, Matrix2, PatchSize, SearchRange);

            Assert.That(() => vector == IdealMove);
        }
        [Test]
        public override void SimdParallel()
        {
            MoveFinderSimdParallel finder = new();

            Vector vector = finder.Find(Matrix1, Matrix2, PatchSize, SearchRange);

            Assert.That(() => vector == IdealMove);
        }
        [Test]
        public override void SimdBountyParallel()
        {
            MoveFinderSimdBountyParallel finder = new();

            Vector vector = finder.Find(Matrix1, Matrix2, PatchSize, SearchRange);

            Assert.That(() => vector == IdealMove);
        }

    }
}
