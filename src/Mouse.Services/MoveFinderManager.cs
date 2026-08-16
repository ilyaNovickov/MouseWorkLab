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

        private const int ParallelResolutionThreshold = 24;
        private const int MinLogicalCpuCountForParallel = 3;

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
            UpdateFinder();
        }

        private IMoveFinder? CurrentFinder { get; set; } 

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
                else
                {
                    UpdateSearchRange();
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
                UpdateSearchRange();
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

                searchRange = value < ThresholdSearchRange ? value : ThresholdSearchRange;
                UpdateFinder();
            }
        }

        private int ThresholdSearchRange
        {
            get => (Resolution + PatchSize) / 2;
        }

        private void UpdateSearchRange()
        {
            if (SearchRange > ThresholdSearchRange)
            {
                SearchRange = ThresholdSearchRange;
            }
        }

        private void UpdateFinder()
        {
            CurrentFinder = SelectFinder();
        }

        private static int IdealMaxDifficult(int resolution)
        {
            return (resolution * resolution) * Convert.ToInt32(Math.Pow(resolution + 2, 2)) / 16;
        }
        
        private static int GetDiffucult(int patchSize, int searchRange)
        {
            return (patchSize * patchSize) * Convert.ToInt32(Math.Pow(2 * searchRange + 1, 2));
        }

        private IMoveFinder SelectFinder()
        {
            //if (patchSize == resolution)
            if (SearchRange == 0)
                return ConstFinder;

            //bool useParallel =
            //    LogicalCpuCount >= MinLogicalCpuCountForParallel &&
            //    resolution >= ParallelSolutionThreshold;
            bool useParallel =
                LogicalCpuCount >= MinLogicalCpuCountForParallel &&
                (Resolution >= ParallelResolutionThreshold && 
                GetDiffucult(PatchSize, SearchRange) > IdealMaxDifficult(Resolution));

            bool patchAlwaysInside = IsPatchAlwaysInside(Resolution, PatchSize, SearchRange);

            return (patchAlwaysInside, useParallel) switch
            {
                (true, true) => SimdParallelFinder,
                (true, false) => SimdFinder,
                (false, true) => SimdParallelBountyFinder,
                _ => SimdBountyFinder
            };
        }

        private static bool IsPatchAlwaysInside(int solution, int patchSize, int searchRange)
        {
            int leftMargin = (solution - patchSize) / 2;
            int rightMargin = solution - (leftMargin + patchSize);

            return searchRange <= leftMargin &&
                   searchRange <= rightMargin;
        }


        public Vector Find(IMatrix matrix1, IMatrix matrix2, bool fillZero = true)
        {
            return CurrentFinder!.Find(matrix1, matrix2, PatchSize, SearchRange, fillZero);
        }
    }
}
