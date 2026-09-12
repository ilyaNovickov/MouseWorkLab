using MouseBaseLib;
using MouseBaseLib.Interfaces.Services;
using MouseStdLib;
using MouseUnsafeLib;
using MouseUnsafeLib.Finders;

namespace Mouse.Services
{
    /// <summary>
    /// Менеджер по поиску перемещения по 2-м изображениям
    /// </summary>
    public class MoveFinderManager : IMoveFinderManager
    {
        private class ConstMoveFinder : IMoveFinder
        {
            public Vector Find(IMatrix matrix1, IMatrix matrix2, int patchSize, int searchRange, bool fillZero = true)
            {
                return new Vector(-1, -1);
            }
        }

        /// <summary>
        /// Кол-во логических ядер ЦП
        /// </summary>
        private readonly int LogicalCpuCount;
        /// <summary>
        /// Максимальное разрешение сенсора
        /// </summary>
        private const int MAX_RESOLUTION = 128;

        private int resolution = 3;
        private int patchSize = 1;
        private int searchRange = (3 - 1) / 2;
        /// <summary>
        /// Разрешение для включения параллельной обработки
        /// </summary>
        private const int ParallelResolutionThreshold = 24;
        /// <summary>
        /// Минимальное кол-во ядер ЦП для параллельной обработки
        /// </summary>
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

        /// <summary>
        /// Текущий сервис поиска
        /// </summary>
        private IMoveFinder? CurrentFinder { get; set; } 

        /// <summary>
        /// Разрешение сенсора
        /// </summary>
        public int Resolution
        {
            get => resolution;
            set
            {
                if (value <= 0)
                    throw new Exception("Размер изображения не может быть меньше или равень 0");
                if (value > MAX_RESOLUTION)
                    throw new Exception($"Размер изображения не может быть больше {MAX_RESOLUTION}");

                resolution = value;

                //Изменить шаблон, если он больше разрешения
                //(изменение интервала поиска проиходит в обоих случаях)
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
        /// <summary>
        /// Размер шаблона
        /// </summary>
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

        /// <summary>
        /// Интервал поиска
        /// </summary>
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
        /// <summary>
        /// Пороговое значение интервала поиска
        /// </summary>
        private int ThresholdSearchRange
        {
            get => (Resolution + PatchSize) / 2;
        }
        /// <summary>
        /// Обновление интервала поиска
        /// </summary>
        private void UpdateSearchRange()
        {
            if (SearchRange > ThresholdSearchRange)
            {
                SearchRange = ThresholdSearchRange;
            }
        }
        /// <summary>
        /// Обновление по сервиса поиска
        /// </summary>
        private void UpdateFinder()
        {
            CurrentFinder = SelectFinder();
        }
        /// <summary>
        /// Максимальная вычислительная сложности при размере шаблона p = R / 2 
        /// и интервале поиска s = (R - p) / 2
        /// R^2 * (R  + 2)^2 / 16
        /// </summary>
        /// <param name="resolution">Разрешение шаблона</param>
        /// <returns>Кол-во операций</returns>
        private static int IdealMaxDifficult(int resolution)
        {
            return (resolution * resolution) * Convert.ToInt32(Math.Pow(resolution + 2, 2)) / 16;
        }
        
        /// <summary>
        /// Получение значения вычислительной сложности N
        /// p^2 * (2 * s + 1)^2
        /// </summary>
        /// <param name="patchSize">Размер шаблона</param>
        /// <param name="searchRange">Интервал поиска</param>
        /// <returns>Значение кол-во операций</returns>
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

            //Использовать параллельности при
            // - Определённом кол-во ядер
            // - Определённом разрешении
            // _ Сложности больше чем максимальная при идеальных обстоятельствах
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
