using Microsoft.Win32;
using ProjectRetina.Algorithms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ProjectRetina
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string Source;
        Bitmap OriginalBitmap;
        Bitmap FinalBitmap;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ChooseFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.tif;...";
            if (openFileDialog.ShowDialog() == true)
            {
                Source = openFileDialog.FileName;
                OriginalBitmap = new Bitmap(Source);

                FinalBitmap = Filter.MedianFilter(OriginalBitmap);
                Image.Source = Utility.BitmapToImageSource(FinalBitmap);
                //Image.Source = Utility.BitmapToImageSource(OriginalBitmap);
                //MessageBox.Show("xD");

                //Bitmap GrayScaleBitmap = GrayScale.Scale(OriginalBitmap, GrayScaleComboBox.SelectedIndex);
                //Image.Source = Utility.BitmapToImageSource(GrayScaleBitmap);
                //MessageBox.Show("xD");

                //Image.Source = Utility.BitmapToImageSource(Filter.BoxBlurFilter(GrayScaleBitmap, 7));
                //MessageBox.Show("xD");

                //FinalBitmap = Utility.ImageSubstraction(GrayScaleBitmap, Filter.BoxBlurFilter(GrayScaleBitmap));
                //Image.Source = Utility.BitmapToImageSource(FinalBitmap);
                //MessageBox.Show("xD");

                //FinalBitmap = Binaryzation.OtsuBinarization(FinalBitmap);
                //Image.Source = Utility.BitmapToImageSource(FinalBitmap);

                //MessageBox.Show("xD");
                //FinalBitmap = Filter.MedianFilter(FinalBitmap);
                //Image.Source = Utility.BitmapToImageSource(FinalBitmap);

                //MessageBox.Show("xD");
                //FinalBitmap = Filter.MaxMinFilter(FinalBitmap, 3, true);
                //Image.Source = Utility.BitmapToImageSource(FinalBitmap);
            }
        }

        private void SourceFolderButton_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}
