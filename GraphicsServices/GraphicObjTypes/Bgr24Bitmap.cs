using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GraphicsServices.GraphicObjTypes
{
    public class Bgr24Bitmap : IEnumerable<Color>
    {
        private int BackBuffer { get; set; }
        private int BackBufferStride { get; set; }
        private int BytesPerPixel { get; set; }

        public int PixelWidth { get; private set; }
        public int PixelHeight { get; private set; }
        public WriteableBitmap Source { get; private set; }

        public Bgr24Bitmap(WriteableBitmap source)
        {
            var formatConvertedBitmap = new FormatConvertedBitmap(source, PixelFormats.Bgra32, null, 0);
            source = new WriteableBitmap(formatConvertedBitmap);
            Source = source;
            PixelWidth = Source.PixelWidth;
            PixelHeight = Source.PixelHeight;
            BackBuffer = Source.BackBuffer.ToInt32();
            BackBufferStride = Source.BackBufferStride;
            BytesPerPixel = Source.Format.BitsPerPixel / 8;
        }

        private unsafe byte* GetAddress(int x, int y)
        {
            return (byte*)(BackBuffer + y * BackBufferStride + x * BytesPerPixel);
        }
        

        public void Clear(Color color)
        {
            for (int i = 0; i < PixelWidth; i++)
            {
                for (int j = 0; j < PixelHeight; j++)
                {
                    this[i, j] = color;
                }
            }
        }

        public unsafe Color this[int x, int y]
        {
            get
            {
                byte* address = GetAddress(x, y);
                return Color.FromArgb(address[3], address[2], address[1], address[0]);
            }

            set
            {
                if (x < PixelWidth && y < PixelHeight && x >= 0 && y >= 0)
                {
                    byte* address = GetAddress(x, y);
                    address[0] = Convert.ToByte(value.B);
                    address[1] = Convert.ToByte(value.G);
                    address[2] = Convert.ToByte(value.R);
                    address[3] = Convert.ToByte(value.A);
                }
            }
        }

        public IEnumerator<Color> GetEnumerator()
        {
            for (int y = 0; y < PixelHeight; y++)
            {
                for (int x = 0; x < PixelWidth; x++)
                {
                    yield return this[x, y];
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
