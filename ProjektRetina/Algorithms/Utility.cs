using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows;
using System.Text.Json;
using Newtonsoft.Json;

namespace ProjectRetina.Algorithms
{
    internal class Utility
    {
        public static BitmapImage BitmapToImageSource(System.Drawing.Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();
                return bitmapimage;
            }
        }

        public static byte[,] ImageTo2DByteArray(Bitmap bmp)
        {
            BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            byte[] array = new byte[data.Width * 3 * data.Height];
            Marshal.Copy(data.Scan0, array, 0, array.Length);


            byte[,] result = new byte[data.Width * 3, data.Height];

            for (int y = 0; y < data.Height; y++)
                for (int x = 0; x < data.Width * 3 - 2; x += 3)
                {
                    int index = y * data.Width * 3 + x;

                    result[x, y] = array[index];
                    result[x + 1, y] = array[index + 1];
                    result[x + 2, y] = array[index + 2];
                }

            bmp.UnlockBits(data);
            return result;
        }

        public static Bitmap ByteArrayToBitmap(byte[,] byteArray)
        {
            Bitmap result = new Bitmap(byteArray.GetLength(0) / 3, byteArray.GetLength(1));
            BitmapData data = result.LockBits(new Rectangle(0, 0, result.Width, result.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            byte[] array = new byte[byteArray.GetLength(0) * byteArray.GetLength(1)];

            for (int y = 0; y < byteArray.GetLength(1); y++)
            {
                for (int x = 0; x < byteArray.GetLength(0); x++)
                {
                    int index = y * byteArray.GetLength(0) + x;
                    array[index] = byteArray[x, y];
                }
            }

            Marshal.Copy(array, 0, data.Scan0, array.Length);
            result.UnlockBits(data);

            return result;
        }

        public static Bitmap ImageToBinaryImage(Bitmap bmp)
        {
            var byteArray = ImageTo2DByteArray(bmp);
            var newByteArray = new byte[byteArray.GetLength(0), byteArray.GetLength(1)];

            for (int y = 0; y < byteArray.GetLength(1); y++)
            {
                for (int x = 0; x < byteArray.GetLength(0) - 2; x += 3)
                {
                    int average = (byteArray[x, y] + byteArray[x + 1, y] + byteArray[x + 2, y]) / 3;
                    newByteArray[x, y] = newByteArray[x + 1, y] = newByteArray[x + 2, y] = (byte)average;
                }
            }
            return ByteArrayToBitmap(newByteArray);
        }

        public static int[,] ImageTo2DIntArray(Bitmap bmp)
        {
            BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            byte[] array = new byte[data.Width * 3 * data.Height];
            Marshal.Copy(data.Scan0, array, 0, array.Length);

            int[,] result = new int[data.Width * 3, data.Height];

            for (int y = 0; y < data.Height; y++)
                for (int x = 0; x < data.Width * 3 - 2; x += 3)
                {
                    int index = y * data.Width * 3 + x;

                    result[x, y] = array[index];
                    result[x + 1, y] = array[index + 1];
                    result[x + 2, y] = array[index + 2];
                }

            bmp.UnlockBits(data);
            return result;
        }

        public static Bitmap IntArrayToBitmap(int[,] intArray)
        {
            Bitmap result = new Bitmap(intArray.GetLength(0) / 3, intArray.GetLength(1));
            BitmapData data = result.LockBits(new Rectangle(0, 0, result.Width, result.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            byte[] array = new byte[intArray.GetLength(0) * intArray.GetLength(1)];

            for (int y = 0; y < intArray.GetLength(1); y++)
            {
                for (int x = 0; x < intArray.GetLength(0); x++)
                {
                    int index = y * intArray.GetLength(0) + x;
                    array[index] = (byte)intArray[x, y];
                }
            }

            Marshal.Copy(array, 0, data.Scan0, array.Length);
            result.UnlockBits(data);

            return result;
        }

        public static Bitmap ImageSubstraction(Bitmap originalBmp, Bitmap blurredBmp)
        {
            if (originalBmp == null || blurredBmp == null)
            {
                return null;
            }

            var originalArray = ImageTo2DIntArray(originalBmp);
            var blurredArray = ImageTo2DIntArray(blurredBmp);

            for (int y = 0; y < originalArray.GetLength(1); y++)
            {
                for (int x = 0; x < originalArray.GetLength(0); x++)
                {
                    originalArray[x, y] = originalArray[x, y] - blurredArray[x, y] > 0 ? originalArray[x, y] - blurredArray[x, y] : 0;
                }
            }

            return IntArrayToBitmap(originalArray);
        }

        public static void SaveConfig(int GrayscaleOption, int BlurOption, int NoiceReductionOption, int MorphologicalOption, int BlurRange, int NoiceRange, int MorphologicalRange)
        {
            Options options = new Options(GrayscaleOption, BlurOption, NoiceReductionOption, MorphologicalOption, BlurRange, NoiceRange, MorphologicalRange);
            string json = JsonConvert.SerializeObject(options);

            System.Windows.Forms.SaveFileDialog saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            saveFileDialog.Filter = "JSON file (*.json)| *.json";
            var result = saveFileDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                if (saveFileDialog.CheckPathExists)
                {
                    File.WriteAllText($"{saveFileDialog.FileName}", json);
                }
                else
                {
                    MessageBox.Show("Wrong folder, select another one", "Alert");
                    return;
                }
            }
        }

    }
}
