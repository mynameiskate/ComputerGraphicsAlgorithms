using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace GraphicsServices.GraphicObjTypes
{
    class Rasterization
    {
        public static void DrawPixelForRasterization(List<PixelInfo> sidesList, Bgr24Bitmap bitmap, ZBuffer zBuf, Color color)
        {
            int minY, maxY;
            PixelInfo xStartPixel, xEndPixel;
            FindMinAndMaxY(sidesList, out minY, out maxY);

            for (int y = minY + 1; y < maxY; y++)
            {
                FindStartAndEndXByY(sidesList, y, out xStartPixel, out xEndPixel);
                int signZ = xStartPixel.Z < xEndPixel.Z ? 1 : -1;
                float deltaZ = Math.Abs((xEndPixel.Z - xStartPixel.Z) / (float)((xEndPixel.X) - (xStartPixel.X)));
                float curZ = xStartPixel.Z;

                for (int x = xStartPixel.X + 1; x < xEndPixel.X; x++)
                {
                    curZ += signZ * deltaZ;

                    if (x > 0 && x < zBuf.Width && y > 0 && y < zBuf.Height)
                    {
                        if (curZ <= zBuf[x, y])
                        {

                            if (curZ > 0 && curZ < 1)
                            {
                                zBuf[x, y] = curZ;
                                bitmap[x, y] = color;
                            }
                        }
                    }
                }
            }
        }

        private static void FindMinAndMaxY(List<PixelInfo> sidesList, out int min, out int max)
        {
            var list = sidesList.OrderBy(x => x.Y).ToList();
            min = list[0].Y;
            max = list[sidesList.Count - 1].Y;
        }

        private static void FindStartAndEndXByY(List<PixelInfo> sidesList, int y, out PixelInfo xStartPixel, out PixelInfo xEndPixel)
        {
            List<PixelInfo> sameYList = sidesList.Where(x => x.Y == y).OrderBy(x => x.X).ToList(); //sorted by X

            xStartPixel = sameYList[0];
            xEndPixel = sameYList[sameYList.Count - 1];
            for (int i = 1; i < sameYList.Count; i++)
            {
                if (sameYList[i].X - sameYList[i - 1].X > 1)
                {
                    xEndPixel = sameYList[i];
                    break;
                }
                else
                {
                    xStartPixel = sameYList[i];
                }
            }
        }
    }
}
