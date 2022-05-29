using Microsoft.Win32;
using ProjectRetina.Algorithms;
using System;
using System.IO;
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
        int RangeForLinearFilters;
        List<string> FileNames;
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
                RangeForLinearFilters = 3;

                Image.Source = Utility.BitmapToImageSource(OriginalBitmap);
                MessageBox.Show("xD");

                Bitmap GrayScaleBitmap = GrayScale.Scale(OriginalBitmap, GrayScaleComboBox.SelectedIndex);
                Image.Source = Utility.BitmapToImageSource(GrayScaleBitmap);
                MessageBox.Show("xD");

                Image.Source = Utility.BitmapToImageSource(Filter.BoxBlurFilter(GrayScaleBitmap, RangeForLinearFilters));
                MessageBox.Show("xD");
                
                FinalBitmap = Utility.ImageSubstraction(GrayScaleBitmap, Filter.BoxBlurFilter(GrayScaleBitmap, RangeForLinearFilters));
                Image.Source = Utility.BitmapToImageSource(FinalBitmap);
                MessageBox.Show("xD");
                
                FinalBitmap = Binaryzation.OtsuBinarization(FinalBitmap);
                Image.Source = Utility.BitmapToImageSource(FinalBitmap);

                MessageBox.Show("xD");
                FinalBitmap = Filter.MedianFilter(FinalBitmap, RangeForLinearFilters);
                Image.Source = Utility.BitmapToImageSource(FinalBitmap);

                MessageBox.Show("xD");
                FinalBitmap = Filter.MaxMinFilter(FinalBitmap, RangeForLinearFilters, false);
                Image.Source = Utility.BitmapToImageSource(FinalBitmap);
            }
        }

        private void SourceFolderButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            var result = folderBrowserDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                if (Directory.Exists(folderBrowserDialog.SelectedPath))
                {
                    FileNames = Directory.EnumerateFiles(folderBrowserDialog.SelectedPath)
                        .Where(file =>  file.EndsWith(".jpg") || 
                                file.EndsWith(".jpeg") || 
                                file.EndsWith(".png") || 
                                file.EndsWith(".gif") || 
                                file.EndsWith(".tif"))
                        .ToList();
                    if (FileNames.Count == 0)
                    {
                        MessageBox.Show("Could not find any files", "Alert");
                    }
                }
                else
                {
                    MessageBox.Show("Could not find any files", "Alert");
                }
            }
        }
    }
}
