using GraphicsServices.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;

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
            buffer = new double[width * height];
            buffer.Populate(2d);
        }

        private int GetAddress(int x, int y)
        {
            return (y - 1) * Width + (x - 1);
        }

        private bool CoordinatesInRange(int x, int y)
        {
            return !(x < 0 || x > Width || y < 0 || y > Height);
        }

        public double this[int x, int y]
        {
            get
            {
                if (CoordinatesInRange(x, y))
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
                if (CoordinatesInRange(x, y))
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
