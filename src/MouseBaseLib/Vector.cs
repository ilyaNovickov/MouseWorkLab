using System;
using System.Collections.Generic;
using System.Text;

namespace MouseBaseLib
{
    /// <summary>
    /// Запись вектора
    /// </summary>
    public record struct Vector
    {
        public Vector() : this(0, 0)
        {

        }

        public Vector(int dx, int dy)
        {
            this.Dx = dx;
            this.Dy = dy;
        }

        /// <summary>
        /// Смещение по оси X
        /// </summary>
        public int Dx { get; set; }
        /// <summary>
        /// Смещение по оси Y
        /// </summary>
        public int Dy { get; set; }

        /// <summary>
        /// Смена знака (инверсия) вектора
        /// </summary>
        public void Inverse()
        {
            this.Dx = -Dx;
            this.Dy = -Dy;
        }

        public static Vector operator -(Vector vector)
        {
            return new Vector(-vector.Dx, -vector.Dy);
        }
    }
}
