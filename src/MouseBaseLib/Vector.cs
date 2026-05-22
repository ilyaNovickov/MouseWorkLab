using System;
using System.Collections.Generic;
using System.Text;

namespace MouseBaseLib
{
    public struct Vector : ICloneable
    {
        public Vector() : this(0, 0)
        {

        }

        public Vector(int dx, int dy)
        {
            this.Dx = dx;
            this.Dy = dy;
        }

        public int Dx { get; set; }
        public int Dy { get; set; }

        public void Inverse()
        {
            this.Dx = -Dx;
            this.Dy = -Dy;
        }

        public object Clone()
        {
            return new Vector(Dx, Dy);
        }

        public static Vector operator -(Vector vector)
        {
            return new Vector(-vector.Dx, -vector.Dy);
        }
    }
}
