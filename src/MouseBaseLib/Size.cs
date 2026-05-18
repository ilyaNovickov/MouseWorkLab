using System;
using System.Collections.Generic;
using System.Text;

namespace MouseBaseLib
{
    public struct Size : ICloneable
    {
        public Size() : this(0, 0)
        {

        }

        public Size(int size) : this(size, size)
        {
            
        }

        public Size(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public int Width
        {
            get;
            set
            {
                if (field < 0)
                    throw new ArgumentException("Ширина не может быть отрицательной");

                field = value;
            }
        }

        public int Height
        {
            get;
            set
            {
                if (field < 0)
                    throw new ArgumentException("Высота не может быть отрицательной");

                field = value;
            }
        }

        public object Clone()
        {
            return new Size(Width, Height);
        }

        public string ToString(string arg)
        {
            switch (arg.ToLower())
            {
                case "vs" or "veryshort" or "very short":
                    return $"(w:{Width}|h:{Height})";
                case "s" or "short":
                    return $"(Width :{Width} | Height : {Height})";
                case "f" or "full":
                    return $"Size : (Width :{Width} | Height : {Height})";
                default:
                    return this.ToString()!;
            }
        }
    }
}
