using System;
using System.Collections.Generic;
using System.Text;

namespace MouseLab.Core
{
    public record struct Size
    {
        private int _width; 
        private int _height;

        public Size(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public Size(int value) : this(value, value)
        {

        }

        public Size() : this(0, 0)
        {

        }

        public int Width
        {
            get => _width;
            set
            {
                if (value < 0)
                    throw new Exception("Width can't be below than 0");

                _width = value;
            }
        }

        public int Height
        {
            get => _height;
            set
            {
                if (value < 0)
                    throw new Exception("Height can't be below than 0");

                _height = value;
            }
        }

        public static Size operator +(Size a, Size b)
        {
            return new Size(a.Width + b.Width, a.Height + b.Height);
        }

        public static Size operator -(Size a, Size b)
        {
            return new Size(a.Width - b.Width, a.Height - b.Height);
        }

        public static Size operator *(Size a, int x)
        {
            return new Size(a.Width * x, a.Height * x);
        }

        public static Size operator /(Size a, int x)
        {
            return new Size(a.Width / x, a.Height / x);
        }
    }
}
