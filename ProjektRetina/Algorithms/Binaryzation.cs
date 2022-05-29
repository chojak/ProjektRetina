using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ProjectRetina.Algorithms
{
    internal class Binaryzation
    {
        private static int[] Histogram(Bitmap bmp)
        {
            int bmpHeight = bmp.Height;
            int bmpWidth = bmp.Width;

            double[] histogram = new double[256];

            int[] red = new int[256];
            int[] green = new int[256];
            int[] blue = new int[256];

            for (int x = 0; x < bmpWidth; x++)
            {
                for (int y = 0; y < bmpHeight; y++)
                {
                    Color pixel = bmp.GetPixel(x, y);

                    red[pixel.R]++;
                    green[pixel.G]++;
                    blue[pixel.B]++;

                    int mean = (pixel.R + pixel.G + pixel.B) / 3;
                    histogram[mean]++;
                }
            }

            return histogram.Select(d => (int)d).ToArray();
        }
        private static byte Otsu(Bitmap bmp)
        {
            // https://www.programmerall.com/article/1106135802/

            int[] tmp = Histogram(bmp);
            double[] histogram = tmp.Select(x => (double)x).ToArray();

            int size = bmp.Height * bmp.Width;
            for (int i = 0; i < 256; i++)
            {
                histogram[i] = histogram[i] / size;
            }

            double avgValue = 0;
            for (int i = 0; i < 256; i++)
            {
                avgValue += i * histogram[i];
            }

            int threshold = 0;
            double maxVariance = 0;
            double w = 0, u = 0;
            for (int i = 0; i < 256; i++)
            {
                w += histogram[i];
                u += i * histogram[i];
                double t = avgValue * w - u;
                double variance = t * t / (w * (1 - w));

                if (variance > maxVariance)
                {
                    maxVariance = variance;
                    threshold = i;
                }
            }

            return (byte)threshold;
        }

        public static Bitmap OtsuBinarization(Bitmap OriginalBmp)
        {
            Bitmap bmp = new Bitmap(OriginalBmp);
            byte threshold = Otsu(OriginalBmp);

            var data = bmp.LockBits(
                new Rectangle(0, 0, bmp.Width, bmp.Height),
                System.Drawing.Imaging.ImageLockMode.ReadWrite,
                System.Drawing.Imaging.PixelFormat.Format24bppRgb
            );

            var bmpData = new byte[data.Width * 3 * data.Height];

            Marshal.Copy(data.Scan0, bmpData, 0, bmpData.Length);

            for (int i = 0; i < bmpData.Length; i += 3)
            {
                byte r = bmpData[i + 0];
                byte g = bmpData[i + 1];
                byte b = bmpData[i + 2];

                byte mean = (byte)((r + g + b) / 3);

                bmpData[i + 0] =
                bmpData[i + 1] =
                bmpData[i + 2] = mean > threshold ? byte.MaxValue : byte.MinValue;
            }

            Marshal.Copy(bmpData, 0, data.Scan0, bmpData.Length);

            bmp.UnlockBits(data);
            return bmp;
        }
    }
}
