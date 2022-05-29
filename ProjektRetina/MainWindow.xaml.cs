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

        string SourceDirectory;
        string DestinationDirectory;
        List<string> FileNames;

        Bitmap OriginalBitmap;
        Bitmap FinalBitmap;
        int RangeForLinearFilters = 3;
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

        private Bitmap TransformImage(string path)
        {
            Bitmap ImageToTransform = new Bitmap(path);

            Bitmap GrayScaleBitmap = GrayScale.Scale(ImageToTransform, GrayScaleComboBox.SelectedIndex);

            GrayScaleBitmap = Utility.ImageSubstraction(GrayScaleBitmap, Filter.BoxBlurFilter(GrayScaleBitmap, RangeForLinearFilters));

            GrayScaleBitmap  = Binaryzation.OtsuBinarization(GrayScaleBitmap);
            GrayScaleBitmap = Filter.MedianFilter(GrayScaleBitmap, RangeForLinearFilters);
            
            GrayScaleBitmap = Filter.MaxMinFilter(GrayScaleBitmap, RangeForLinearFilters, 
                MorphologyComboBox.SelectedValue.ToString() == "Min");

            return GrayScaleBitmap; 
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
                        .Where(file => file.EndsWith(".jpg") ||
                                file.EndsWith(".jpeg") ||
                                file.EndsWith(".png") ||
                                file.EndsWith(".gif") ||
                                file.EndsWith(".tif"))
                        .ToList();

                    System.Diagnostics.Debug.WriteLine(FileNames.Count().ToString());
                    if (FileNames.Count == 0)
                    {
                        MessageBox.Show("Could not find any files", "Alert");
                        return;
                    }
                    SourceDirectoryTextBlock.Text = SourceDirectory = folderBrowserDialog.SelectedPath;
                    AllFilesButton.IsEnabled = DestinationDirectory != null && SourceDirectory != null ? true : false;
                }
                else
                {
                    MessageBox.Show("Wrong folder, select another one", "Alert");
                    return;
                }
            }
        }

        private void DestinationFolderButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            var result = folderBrowserDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                if (Directory.Exists(folderBrowserDialog.SelectedPath))
                {
                    //if (Directory.EnumerateFiles(folderBrowserDialog.SelectedPath).Any())
                    //{
                    //    MessageBox.Show("Destination folder must be empty!", "Alert");
                    //    return;
                    //}
                    DestinationDirectoryTextBlock.Text = DestinationDirectory = folderBrowserDialog.SelectedPath;
                    AllFilesButton.IsEnabled = DestinationDirectory != null && SourceDirectory != null ? true : false;
                }
                else
                {
                    MessageBox.Show("Wrong folder, select another one", "Alert");
                    return;
                }
            }
        }

        private void AllFilesButton_Click(object sender, RoutedEventArgs e)
        {
            //int counter = 0;
            //ProgressBar.Maximum = FileNames.Count;
            foreach (var file in FileNames)
            {
                //counter++;
                //ProgressBar.Value = (double)counter;
                //ProgressTextBlock.Text = $"{counter}/{FileNames.Count} ({Math.Round((decimal)(counter/FileNames.Count) * 100)})%";
                TransformImage(file).Save(DestinationDirectory + '\\' + System.IO.Path.GetFileName(file));
            }
        }

        private void RangeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            int range;
            if (int.TryParse(RangeTextBox.Text, out range))
            {
                if (range > 0 && range <= 50)
                {
                    RangeForLinearFilters = range;
                }
                else
                {
                    MessageBox.Show("Range value must be between 0 and 50", "Invalid range value", MessageBoxButton.OK, MessageBoxImage.Error);
                    RangeTextBox.Text = RangeForLinearFilters.ToString();
                }
            }
            else
            {
                MessageBox.Show("Range value can be only integer", "Invalid range value", MessageBoxButton.OK, MessageBoxImage.Error);
                RangeTextBox.Text = RangeForLinearFilters.ToString();
            }
        }
    }
}
