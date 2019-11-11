using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphicsServices.GraphicObjTypes
{
    public class ZBuffer : IEnumerable<double>
    {
        private double[] buffer;

        public int Width { get; set; }
        public int Height { get; set; }

        public ZBuffer(int width, int height)
        {
            Width = width;
            Height = height;
            buffer = System.Linq.Enumerable.Repeat(2d, width * height).ToArray();
        }

        private int GetAddress(int x, int y)
        {
            return (y - 1) * Width + (x - 1);
        }

        private bool IsValidParams(int x, int y)
        {
            bool result = true;

            if (x < 0 || x > Width || y < 0 || y > Height)
            {
                result = false;
            }

            return result;
        }

        public double this[int x, int y]
        {
            get
            {
                if (IsValidParams(x, y))
                {
                    int address = GetAddress(x, y);
                    return buffer[address];
                }
                else
                {
                    throw new IndexOutOfRangeException();
                }
            }
            set
            {
                if (IsValidParams(x, y))
                {
                    int address = GetAddress(x, y);
                    buffer[address] = value;
                }
                else
                {
                    throw new IndexOutOfRangeException();
                }
            }
        }

        public IEnumerator<double> GetEnumerator()
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    yield return this[x, y];
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return buffer.GetEnumerator();
        }
    }
}
