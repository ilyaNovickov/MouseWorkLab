using System;
using System.Collections.Generic;
using System.Text;

namespace MouseBaseLib
{
    public struct Vector
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
    }
}
