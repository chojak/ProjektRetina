using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ProjectRetina.Algorithms
{
    internal class GrayScale
    {
        public static Bitmap Scale(Bitmap bmp, int option)
        {
            Bitmap newBmp = new Bitmap(bmp);

            var data = newBmp.LockBits(
                new Rectangle(0, 0, newBmp.Width, newBmp.Height),
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

                //if (g == 255)
                    //MessageBox.Show("XD");

                if (option == 0)
                {
                    bmpData[i + 0] = bmpData[i + 1] = bmpData[i + 2] = mean;
                }
                else if (option == 1)
                {
                    bmpData[i + 0] = bmpData[i + 1] = bmpData[i + 2] = b;
                }
                else if (option == 2)
                {
                    bmpData[i + 0] = bmpData[i + 1] = bmpData[i + 2] = g;
                }
                else if (option == 3)
                {
                    bmpData[i + 0] = bmpData[i + 1] = bmpData[i + 2] = r;
                }
            }

            Marshal.Copy(bmpData, 0, data.Scan0, bmpData.Length);

            newBmp.UnlockBits(data);
            return newBmp;
        }
    }
}
