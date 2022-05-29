using ProjectRetina.Algorithms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ProjectRetina
{
    internal class Filter
    {
        public static int[,] matrixX = new int[,] { { 1, 2, 1, }, { 2, 4, 2, }, { 1, 2, 1, } };

        public static Bitmap GausFilter(Bitmap bmp)
        {
            if (bmp == null)
            {
                return new Bitmap(1, 1);
            }

            int[,] byteArray = Utility.ImageTo2DIntArray(bmp);
            int[,] resultArray = new int[byteArray.GetLength(0), byteArray.GetLength(1)];

            int range = 2;

            for (int y = 1; y < bmp.Height; y++)
            {
                for (int x = 3; x < bmp.Width; x++)
                {
                    double magX = 0;

                    for (int yy = 0; yy < 3; yy++)
                    {
                        for (int xx = 0; xx < 3; xx++)
                        {
                            int xn = x + xx - 1;
                            int yn = y + yy - 1;

                            magX += byteArray[xn * 3, yn] * matrixX[xx, yy];
                        }
                    }

                    int value = (int)Math.Sqrt(magX * magX);
                    resultArray[x, y] = resultArray[x + 1, y] = resultArray[x + 2, y] = value > 255 ? 255 : value < 0 ? 0 : value;

                }
            }
            return Utility.IntArrayToBitmap(resultArray);
        }

        public static Bitmap BoxBlurFilter(Bitmap bmp, int range = 3)
        {
            if (bmp == null)
            {
                return new Bitmap(1, 1);
            }

            int[,] inputArray = Utility.ImageTo2DIntArray(bmp);
            int[,] resultArray = new int[inputArray.GetLength(0), inputArray.GetLength(1)];
            range /= 2;

            for (int y = 0; y < inputArray.GetLength(1); y++)
            {
                for (int x = 0; x < inputArray.GetLength(0) - 2; x += 3)
                {
                    double total = 0;
                    int counter = 0;

                    for (int yy = y - range; yy <= y + range; yy++)
                    {
                        for (int xx = x - range * 3; xx <= x + range * 3; xx += 3)
                        {
                            if (xx >= 0 && xx < inputArray.GetLength(0) && yy >= 0 && yy < inputArray.GetLength(1))
                            {
                                total += inputArray[xx, yy];
                                counter++;
                            }
                        }
                    }

                    resultArray[x, y] =
                    resultArray[x + 1, y] =
                    resultArray[x + 2, y] = (int)total / counter;
                }
            }
            return Utility.IntArrayToBitmap(resultArray);
        }

        public static Bitmap MedianFilter(Bitmap bmp, int range)
        {
            if (bmp == null)
            {
                return new Bitmap(1, 1);
            }

            Bitmap newBmp = new Bitmap(bmp.Width, bmp.Height);
            var byteArray = Utility.ImageTo2DByteArray(bmp);
            range /= 2;
            List<int> reds = new List<int>();
            List<int> greens = new List<int>();
            List<int> blues = new List<int>();

            var returnArray = new int[byteArray.GetLength(0), byteArray.GetLength(1)];

            for (int y = 0; y < bmp.Height; y++)
            {
                for (int x = 0; x < bmp.Width; x++)
                {
                    reds.Clear();
                    greens.Clear();
                    blues.Clear();

                    for (int yy = y - range; yy <= y + range; ++yy)
                    {
                        if (yy >= 0 && yy < bmp.Height)
                            for (int xx = (x - range) * 3; xx <= (x + range) * 3; xx += 3)
                            {
                                if (xx >= 0 && xx < bmp.Width * 3)
                                {
                                    reds.Add(byteArray[xx, y]);
                                    greens.Add(byteArray[xx + 1, y]);
                                    blues.Add(byteArray[xx + 2, y]);
                                }
                            }
                    }

                    reds.Sort();
                    greens.Sort();
                    blues.Sort();

                    returnArray[x * 3, y] =
                    returnArray[x * 3 + 1, y] =
                    returnArray[x * 3 + 2, y] = reds[reds.Count() / 2];
                }
            }
            return Utility.IntArrayToBitmap(returnArray);
        }
    }
}

