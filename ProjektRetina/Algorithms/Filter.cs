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
        public static int[,] gaussianMatrix;

        private static int[,] generateGaussian(int range = 5, double sd = 1)
        {
            int[,] matrix = new int[range, range];
            double[,] gaussian = new double[range, range];
            range /= 2;
            double total = 0;
            double min = double.MaxValue;
            double max = 0;

            for (int y = -range; y <= range; y++)
            {
                for (int x = -range; x <= range; x++)
                {
                    // https://homepages.inf.ed.ac.uk/rbf/HIPR2/gsmooth.htm

                    gaussian[x + range, y + range] = 1 / (2 * Math.PI * sd * sd) * Math.Pow(Math.E, -(x * x + y * y / (2 * sd * sd)));
                    total += gaussian[x + range, y + range];
                    min = Math.Min(min, gaussian[x + range, y + range]);
                    max = Math.Max(max, gaussian[x + range, y + range]);
                }
            }
            total /= (range * range);

            for (int y = -range; y <= range; y++)
            {
                for (int x = -range; x <= range; x++)
                {
                    // https://homepages.inf.ed.ac.uk/rbf/HIPR2/gsmooth.htm

                    double z = (gaussian[x + range, y + range] - min) / (max - min);
                }
            }
            return matrix;
        }
        public static Bitmap GausFilter(Bitmap bmp)
        {
            if (bmp == null)
            {
                return new Bitmap(1, 1);
            }

            if (gaussianMatrix == null)
            {
                gaussianMatrix = generateGaussian();
                System.Diagnostics.Debug.WriteLine("nigger");
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

        public static Bitmap MaxMinFilter(Bitmap bmp, int range = 3, bool isMin = false)
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
                    int tmp = isMin ? 255 : 0;

                    for (int yy = y - range; yy <= y + range; yy++)
                    {
                        for (int xx = x - range * 3; xx <= x + range * 3; xx += 3)
                        {
                            if (xx >= 0 && xx < inputArray.GetLength(0) && yy >= 0 && yy < inputArray.GetLength(1))
                            {
                                if (isMin)
                                    tmp = Math.Min(tmp, inputArray[xx, yy]);
                                else
                                    tmp = Math.Max(tmp, inputArray[xx, yy]);
                            }
                        }
                    }

                    resultArray[x, y] =
                    resultArray[x + 1, y] =
                    resultArray[x + 2, y] = tmp;
                }
            }
            return Utility.IntArrayToBitmap(resultArray);
        }

        public static Bitmap MedianFilter(Bitmap bmp, int range = 7)
        {
            if (bmp == null)
            {
                return new Bitmap(1, 1);
            }

            int[,] intArray = Utility.ImageTo2DIntArray(bmp);
            range /= 2;
            List<int> median = new List<int>();
            var returnArray = new int[intArray.GetLength(0), intArray.GetLength(1)];

            for (int y = 0; y < intArray.GetLength(1); y++)
                {
                for (int x = 0; x < intArray.GetLength(0) - 2; x += 3)
                {
                    median.Clear();

                    for (int yy = y - range; yy <= y + range; yy++)
                    {
                        for (int xx = x - range * 3; xx <= x + range * 3; xx += 3)
                        {
                            if (xx >= 0 && xx < intArray.GetLength(0) && yy >= 0 && yy < intArray.GetLength(1))
                            {
                                median.Add(intArray[xx, yy]);
                            }
                        }
                    }

                    median.Sort();

                    returnArray[x, y] =
                    returnArray[x + 1, y] =
                    returnArray[x + 2, y] = median[median.Count() / 2];
                }
            }
            return Utility.IntArrayToBitmap(returnArray);
        }
    }
}

