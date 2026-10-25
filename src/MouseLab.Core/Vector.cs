using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics.CodeAnalysis;

namespace MouseLab.Core
{
    public struct Vector
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

        public bool Equals(Vector vector)
        {
            return this.Dx == vector.Dx && this.Dy == vector.Dy;
        }

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            if (obj is not Vector vector)
                throw new Exception("The compared object is not vector");

            return this.Equals(vector);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

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

        public static bool operator ==(Vector a, Vector b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Vector a, Vector b)
        {
            return a.Equals(b);
        }
    }
}
