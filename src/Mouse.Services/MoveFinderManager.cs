using MouseBaseLib;
using MouseBaseLib.Interfaces.Services;
using MouseStdLib;
using MouseUnsafeLib;
using MouseUnsafeLib.Finders;

namespace Mouse.Services
{
    public class MoveFinderManager : IMoveFinderManager
    {
        private class ConstMoveFinder : IMoveFinder
        {
            public Vector Find(IMatrix matrix1, IMatrix matrix2, int patchSize, int searchRange, bool fillZero = true)
            {
                return new Vector(-1, -1);
            }
        }

        private readonly int LogicalCpuCount;

        private const int MAX_SOLUTION = 128;

        private int resolution = 3;
        private int patchSize = 1;
        private int searchRange = (3 - 1) / 2;

        private readonly IMoveFinder ConstFinder = new ConstMoveFinder();
        private readonly IMoveFinder FastFinder = new MoveFinderFast();
        private readonly IMoveFinder BountyFinder = new MoveFinderBoundary();
        private readonly IMoveFinder SimdFinder = new MoveFinderSimd();
        private readonly IMoveFinder SimdBountyFinder = new MoveFinderSimdBoundary();
        private readonly IMoveFinder SimdParallelFinder = new MoveFinderSimdParallel();
        private readonly IMoveFinder SimdParallelBountyFinder = new MoveFinderSimdBoundaryParallel();

        public MoveFinderManager()
        {
            LogicalCpuCount = Environment.ProcessorCount;
            CurrentFinder = SimdFinder;
        }

        private IMoveFinder CurrentFinder { get; set; } 

        public int Resolution
        {
            get => resolution;
            set
            {
                if (value <= 0)
                    throw new Exception("Размер изображения не может быть меньше или равень 0");
                if (value > MAX_SOLUTION)
                    throw new Exception($"Размер изображения не может быть больше {MAX_SOLUTION}");

                resolution = value;

                if (PatchSize > resolution)
                {
                    PatchSize = resolution;
                    return;
                }

                UpdateFinder();
            }
        }

        public int PatchSize
        {
            get => patchSize;
            set
            {
                if (value <= 0)
                    throw new Exception("Размер шаблона не может быть меньше или равень 0");
                if (value > Resolution)
                    throw new Exception($"Размер шаблона не может быть больше размера изображения");

                patchSize = value;
                UpdateFinder();
            }
        }

        public int SearchRange
        {
            get => searchRange;
            set
            {
                if (value < 0)
                    throw new Exception("Интервал поиска не может быть меньше 0");

                searchRange = value;
                UpdateFinder();
            }
        }

        private void UpdateFinder()
        {
            Point patchPosition = new Point()
            {
                X = (Resolution - PatchSize) / 2,
                Y = (Resolution - PatchSize) / 2
            };
            Point patchBorder = new Point()
            {
                X = patchPosition.X + patchSize,
                Y = patchPosition.Y + patchSize
            };

            if (patchPosition == Point.Zero && patchBorder == new Point(Resolution, Resolution))
            {
                CurrentFinder = ConstFinder;
                return;
            }

            bool isPatchInsideOnly = (patchPosition.X - SearchRange) >= 0 && 
                (patchPosition.Y - SearchRange) >= 0 && 
                (patchBorder.X + SearchRange) < Resolution && 
                (patchBorder.Y + SearchRange) < Resolution;

            if (isPatchInsideOnly && LogicalCpuCount > 2 && Resolution >= 64)
            {
                CurrentFinder = SimdParallelFinder;
            }
            else if (isPatchInsideOnly)
            {
                CurrentFinder = SimdFinder;
            }
            else if (LogicalCpuCount > 2 && Resolution >= 64)
            {
                CurrentFinder = SimdParallelBountyFinder;
            }
            else
            {
                CurrentFinder = SimdBountyFinder;
            }
        }


        public Vector Find(IMatrix matrix1, IMatrix matrix2, bool fillZero = true)
        {
            return CurrentFinder.Find(matrix1, matrix2, PatchSize, SearchRange, fillZero);
        }
    }
}
