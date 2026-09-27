using System;
using System.Collections.Generic;
using System.Text;

namespace MouseLab.Core
{
    public record struct Vector
    {
        public static Vector Zero => new Vector(0, 0);

        public Vector() : this(0, 0)
        {

        }

        public Vector(int dx, int dy)
        {
            this.Dx = dx;
            this.Dy = dy;
        }

        public Vector(int move) : this(move, move)
        {

        }

        public int Dx { get; init; }

        public int Dy { get; init; }

        public static Vector operator +(Vector a, Vector b)
        {
            return new Vector(a.Dx + b.Dx, a.Dy + b.Dy);
        }

        public static Vector operator -(Vector a, Vector b)
        {
            return new Vector(a.Dx - b.Dx, a.Dy - b.Dy);
        }

        public static Vector operator -(Vector a)
        {
            return new Vector(-a.Dx, -a.Dy);
        }

        public static Vector operator *(Vector a, int x)
        {
            return new Vector(a.Dx * x, a.Dy * x);
        }

        public static Vector operator /(Vector a, int x)
        {
            return new Vector(a.Dx / x, a.Dy / x);
        }
    }
}
