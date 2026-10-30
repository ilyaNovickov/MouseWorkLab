using MouseLab.Common.Services;
using Xunit;

namespace MouseLab.Core.Tests
{
    public class MoveFinderManagerTests
    {
        private static MoveFinderManager CreateManager()
            => new(new MoveFinderFactory());

        [Fact]
        public void DefaultsAreValid()
        {
            var manager = CreateManager();

            Assert.Equal(12, manager.Resolution);
            Assert.Equal(6, manager.PatchSize);
            Assert.Equal(3, manager.SearchRange);
            Assert.False(manager.RelateSearchWithPatch);
            Assert.NotNull(manager.SelectedFinder);
        }

        [Fact]
        public void DefaultsSelectSimpleFinder()
        {
            var factory = new MoveFinderFactory();
            var manager = new MoveFinderManager(factory);

            Assert.Same(factory.CreateSimpleFinder(), manager.SelectedFinder);
        }

        [Fact]
        public void SearchRangeBeyondPatchOffsetSelectsBoundaryFinder()
        {
            var factory = new MoveFinderFactory();
            var manager = new MoveFinderManager(factory);
            manager.SearchRange = 4;

            Assert.Same(factory.CreateSimpleBoundaryFinder(), manager.SelectedFinder);
        }

        [Fact]
        public void LargeWorkSelectsParallelFinderWhenEnoughCores()
        {
            var factory = new MoveFinderFactory();
            var manager = new MoveFinderManager(factory);
            manager.Resolution = 128;
            manager.PatchSize = 64;
            manager.SearchRange = 32;

            var expected = Environment.ProcessorCount >= 4
                ? factory.CreateParallelFinder()
                : factory.CreateSimpleFinder();

            Assert.Same(expected, manager.SelectedFinder);
        }

        [Fact]
        public void LargeWorkWithoutSearchIterationsStaysScalar()
        {
            var factory = new MoveFinderFactory();
            var manager = new MoveFinderManager(factory);
            manager.Resolution = 500;
            manager.PatchSize = 500;
            manager.SearchRange = 0;

            Assert.Same(factory.CreateSimpleFinder(), manager.SelectedFinder);
        }

        [Fact]
        public void LargeBoundaryWorkSelectsParallelBoundaryFinderWhenEnoughCores()
        {
            var factory = new MoveFinderFactory();
            var manager = new MoveFinderManager(factory);
            manager.Resolution = 40;
            manager.PatchSize = 10;
            manager.SearchRange = 20;

            var expected = Environment.ProcessorCount >= 4
                ? factory.CreateParallelBoundaryFinder()
                : factory.CreateSimpleBoundaryFinder();

            Assert.Same(expected, manager.SelectedFinder);
        }

        [Fact]
        public void ResolutionDecreaseClampsPatchAndSearch()
        {
            var manager = CreateManager();
            manager.SearchRange = 9;

            manager.Resolution = 4;

            Assert.Equal(4, manager.PatchSize);
            Assert.Equal(4, manager.SearchRange);
        }

        [Fact]
        public void ResolutionDecreaseClampsOnlySearch()
        {
            var manager = CreateManager();
            manager.SearchRange = 9;

            manager.Resolution = 8;

            Assert.Equal(6, manager.PatchSize);
            Assert.Equal(7, manager.SearchRange);
        }

        [Fact]
        public void ResolutionIncreaseRecomputesRelatedSearch()
        {
            var manager = CreateManager();
            manager.RelateSearchWithPatch = true;

            manager.Resolution = 20;

            Assert.Equal(6, manager.PatchSize);
            Assert.Equal(7, manager.SearchRange);
        }

        [Fact]
        public void PatchSizeDecreaseClampsSearch()
        {
            var manager = CreateManager();
            manager.SearchRange = 9;

            manager.PatchSize = 4;

            Assert.Equal(8, manager.SearchRange);
        }

        [Fact]
        public void PatchSizeEqualToResolutionIsAllowed()
        {
            var manager = CreateManager();
            manager.RelateSearchWithPatch = true;

            manager.PatchSize = 12;

            Assert.Equal(12, manager.PatchSize);
            Assert.Equal(0, manager.SearchRange);
        }

        [Fact]
        public void PatchSizeGreaterThanResolutionThrows()
        {
            var manager = CreateManager();

            Assert.Throws<ArgumentOutOfRangeException>(() => manager.PatchSize = 13);
        }

        [Fact]
        public void SearchRangeGreaterThanMaxThrows()
        {
            var manager = CreateManager();

            Assert.Throws<ArgumentOutOfRangeException>(() => manager.SearchRange = 10);
        }

        [Fact]
        public void NegativeValuesThrow()
        {
            var manager = CreateManager();

            Assert.Throws<ArgumentOutOfRangeException>(() => manager.Resolution = -1);
            Assert.Throws<ArgumentOutOfRangeException>(() => manager.PatchSize = -1);
            Assert.Throws<ArgumentOutOfRangeException>(() => manager.SearchRange = -1);
        }

        [Fact]
        public void SearchRangeSetterThrowsWhileRelateIsOn()
        {
            var manager = CreateManager();
            manager.RelateSearchWithPatch = true;

            Assert.Throws<InvalidOperationException>(() => manager.SearchRange = 3);
        }

        [Fact]
        public void ToggleRelateDerivesSearchAndKeepsItWhenOff()
        {
            var manager = CreateManager();
            manager.Resolution = 20;
            manager.PatchSize = 4;
            manager.SearchRange = 12;

            manager.RelateSearchWithPatch = true;
            Assert.Equal(8, manager.SearchRange);

            manager.RelateSearchWithPatch = false;
            Assert.Equal(8, manager.SearchRange);
        }
    }
}
