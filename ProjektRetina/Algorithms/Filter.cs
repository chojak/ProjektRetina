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
        private static double[,] gaussianMatrix;

        public static void generateGaussian(int range = 5, double sd = 1)
        {
            double[,] gaussian = new double[range, range];
            int newRange = range / 2;

            for (int y = -newRange; y <= newRange; y++)
            {
                for (int x = -newRange; x <= newRange; x++)
                {
                    gaussian[x + newRange, y + newRange] = (1 / (2 * Math.PI * sd * sd)) * Math.Exp(-(x * x + y * y) / (2 * sd * sd));
                }
            }

            //for (int y = 0; y < range; y++)
            //{
            //    string linia = "{ ";
            //    for (int x = 0; x < range; x++)
            //    {
            //        // https://homepages.inf.ed.ac.uk/rbf/HIPR2/gsmooth.htm
            //        linia += $"{gaussian[x, y]} \t\t";
            //    }
            //    linia += "}";
            //    System.Diagnostics.Debug.WriteLine(linia);
            //}

            gaussianMatrix = gaussian;
        }
        public static Bitmap GaussBlurFilter(Bitmap bmp, int range = 5)
        {
            if (bmp == null)
            {
                return new Bitmap(1, 1);
            }

            if (gaussianMatrix == null)
            {
                generateGaussian(range);
            }

            int[,] inputArray = Utility.ImageTo2DIntArray(bmp);
            int[,] resultArray = new int[inputArray.GetLength(0), inputArray.GetLength(1)];
            range /= 2;
            double maxdowyjebania = 0;

            for (int y = 0; y < inputArray.GetLength(1); y++)
            {
                for (int x = 0; x < inputArray.GetLength(0) - 2; x += 3)
                {
                    double total = 0;
                    double counter = 0;
                    int gaussYCounter = 0;
                    for (int yy = y - range; yy <= y + range; yy++)
                    {
                        int gaussXCounter = 0;
                        for (int xx = x - range * 3; xx <= x + range * 3; xx += 3)
                        {
                            if (xx >= 0 && xx < inputArray.GetLength(0) && yy >= 0 && yy < inputArray.GetLength(1))
                            {
                                total += inputArray[xx, yy] * gaussianMatrix[gaussXCounter, gaussYCounter];
                                counter += gaussianMatrix[gaussXCounter, gaussYCounter];
                            }
                            gaussXCounter ++;
                        }
                        gaussYCounter++;
                    }

                    resultArray[x, y] =
                    resultArray[x + 1, y] =
                    resultArray[x + 2, y] = (int)(total / counter);

                    maxdowyjebania = Math.Max(maxdowyjebania, counter);

                    //resultArray[x, y] =
                    //resultArray[x + 1, y] =
                    //resultArray[x + 2, y] = (int)(total);
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

