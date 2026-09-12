using System;
using System.Collections.Generic;
using System.Text;

namespace MouseBaseLib.Interfaces.Services
{
    /// <summary>
    /// Менеджер для поиска смещения по 2-м изображениям
    /// </summary>
    public interface IMoveFinderManager
    {
        /// <summary>
        /// Разрешение матриц
        /// </summary>
        int Resolution { get; set; }
        /// <summary>
        /// Размер шаблона
        /// </summary>
        int PatchSize { get; set; }
        /// <summary>
        /// Интервал поиска
        /// </summary>
        int SearchRange { get; set; }
        /// <summary>
        /// Выхов метода поиска перемещения
        /// </summary>
        Vector Find(IMatrix matrix1, IMatrix matrix2, bool fillZero = true);
    }
}
