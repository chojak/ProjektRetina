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
using System.Threading;
using System.Diagnostics;
using Newtonsoft.Json;

namespace ProjectRetina
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public class Options
    {
        public Options()
        {
        }
        public Options(int GrayscaleOption, int BlurOption, int NoiceReductionOption, int MorphologicalOption, int BlurRange, int NoiceRange, int MorphologicalRange)
        {
            this.GrayscaleOption = GrayscaleOption;
            this.BlurOption = BlurOption;
            this.NoiceReductionOption = NoiceReductionOption;
            this.MorphologicalOption = MorphologicalOption;

            this.BlurRange = BlurRange;
            this.NoiceRange = NoiceRange;
            this.MorphologicalRange = MorphologicalRange;
        }

        public int GrayscaleOption;
        public int BlurOption;
        public int NoiceReductionOption;
        public int MorphologicalOption;

        public int BlurRange;
        public int NoiceRange;
        public int MorphologicalRange;
    }
    public partial class MainWindow : Window
    {
        string Source;
        string SourceDirectory;
        string DestinationDirectory;
        List<string> FileNames;

        public int GrayscaleOption = 0;
        public int BlurOption = 0;
        public int NoiceReductionOption = 0;
        public int MorphologicalOption = 0;

        public int BlurRange = 3;
        public int NoiceRange = 3;
        public int MorphologicalRange = 3;

        int Counter = 0;
        Bitmap OriginalBitmap;
        Bitmap FinalBitmap;
        Bitmap tmp;

        private void TransformImageWithMessages(string path)
        {
            OriginalBitmap = new Bitmap(Source);

            Image.Source = Utility.BitmapToImageSource(OriginalBitmap);
            MessageBox.Show("Original bitmap ");

            Bitmap GrayScaleBitmap = GrayScale.Scale(OriginalBitmap, GrayScaleComboBox.SelectedIndex);
            Image.Source = Utility.BitmapToImageSource(GrayScaleBitmap);
            MessageBox.Show($"Grayscale for {GrayScaleComboBox.SelectedValue.ToString().Split(' ')[GrayScaleComboBox.SelectedValue.ToString().Split(' ').Length - 1]} channel");

            if (BlurOption == 0)
            {
                tmp = Filter.GaussBlurFilter(GrayScaleBitmap, BlurRange);
                Image.Source = Utility.BitmapToImageSource(tmp);
                MessageBox.Show("Gauss blur");
            }
            else
            {
                tmp = Filter.BoxBlurFilter(GrayScaleBitmap, BlurRange);
                Image.Source = Utility.BitmapToImageSource(tmp);
                MessageBox.Show("BoxBlur blur");
            }

            FinalBitmap = Utility.ImageSubstraction(GrayScaleBitmap, tmp);
            Image.Source = Utility.BitmapToImageSource(FinalBitmap);
            MessageBox.Show("Image substraction");

            FinalBitmap = Binaryzation.OtsuBinarization(FinalBitmap);
            Image.Source = Utility.BitmapToImageSource(FinalBitmap);
            MessageBox.Show("Otsu binarization");

            if (NoiceReductionOption == 0)
            {
                FinalBitmap = Filter.MedianFilter(FinalBitmap, NoiceRange);
                Image.Source = Utility.BitmapToImageSource(FinalBitmap);
                MessageBox.Show("Median filter");
            }
            else
            {
                // another filter removing noices
                MessageBox.Show("NIE MA XD");
            }

            if (MorphologicalOption == 3)
            {
                // max
                FinalBitmap = Filter.MaxMinFilter(FinalBitmap, MorphologicalRange, false);
                Image.Source = Utility.BitmapToImageSource(FinalBitmap);
                MessageBox.Show("Max filter");

            }
            else if (MorphologicalOption == 2)
            {
                // min
                FinalBitmap = Filter.MaxMinFilter(FinalBitmap, MorphologicalRange, true);
                Image.Source = Utility.BitmapToImageSource(FinalBitmap);
                MessageBox.Show("Min/Max filter");
            }
            else if (MorphologicalOption == 0)
            {
                FinalBitmap = Filter.ErosionDilationFilter(FinalBitmap, MorphologicalRange, false);
                Image.Source = Utility.BitmapToImageSource(FinalBitmap);
                MessageBox.Show("Erosion filter");
            }
            else if (MorphologicalOption == 1)
            {
                FinalBitmap = Filter.ErosionDilationFilter(FinalBitmap, MorphologicalRange, true);
                Image.Source = Utility.BitmapToImageSource(FinalBitmap);
                MessageBox.Show("Dilation filter");
            }
        }

        private Bitmap TransformImage(string path)
        {
            Bitmap ImageToTransform = new Bitmap(path);

            Bitmap FinalBitmap = GrayScale.Scale(ImageToTransform, GrayscaleOption);

            if (BlurOption == 0)
                FinalBitmap = Utility.ImageSubstraction(FinalBitmap, Filter.GaussBlurFilter(FinalBitmap, BlurRange));
            else if (BlurOption == 1)
                FinalBitmap = Utility.ImageSubstraction(FinalBitmap, Filter.BoxBlurFilter(FinalBitmap, BlurRange));

            FinalBitmap = Binaryzation.OtsuBinarization(FinalBitmap);
            
            if (NoiceReductionOption == 0)
                FinalBitmap = Filter.MedianFilter(FinalBitmap, NoiceRange);
            else if (NoiceReductionOption == 1)
            {
                // jakis inny filtr
                MessageBox.Show("jest tylko median");
            }

            if (MorphologicalOption == 3)
                FinalBitmap = Filter.MaxMinFilter(FinalBitmap, MorphologicalRange, false); // max
            else if (MorphologicalOption == 2)
                FinalBitmap = Filter.MaxMinFilter(FinalBitmap, MorphologicalRange, true);  // min
            else if (MorphologicalOption == 0)
                FinalBitmap = Filter.ErosionDilationFilter(FinalBitmap, MorphologicalRange, false);  // erosion
            else if (MorphologicalOption == 1) 
                FinalBitmap = Filter.ErosionDilationFilter(FinalBitmap, MorphologicalRange, true);   // dilation

            return FinalBitmap; 
        }

        private void ProcessImage(object obj)
        {
            TransformImage(obj.ToString()).Save(DestinationDirectory + '\\' + System.IO.Path.GetFileName(obj.ToString()));

            Counter++;

            this.Dispatcher.Invoke(() =>
            {
                ProgressBar.Value = (double)Counter;
                ProgressTextBlock.Text = $"{Counter}/{FileNames.Count} ({Math.Round((decimal)Counter / FileNames.Count * 100, 2)}%)";
            });
        }

        public MainWindow()
        {
            InitializeComponent();

            //ThreadPool.SetMinThreads(8, 2);
            //ThreadPool.SetMaxThreads(8, 2);
        }

        private void SaveConfig_Click(object sender, RoutedEventArgs e)
        {
            Utility.SaveConfig(GrayscaleOption, BlurOption, NoiceReductionOption, MorphologicalOption, BlurRange, NoiceRange, MorphologicalRange);
        }

        private void LoadConfig_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Choose config file";
            openFileDialog.Filter = "JSON files|*.json";
            if (openFileDialog.ShowDialog() == true)
            {
                string json = File.ReadAllText(openFileDialog.FileName);
                Options options = JsonConvert.DeserializeObject<Options>(json);

                GrayScaleComboBox.SelectedIndex = GrayscaleOption = options.GrayscaleOption;

                BlurComboBox.SelectedIndex = BlurOption = options.BlurOption;

                BlurRange = options.BlurRange;
                BlurRangeTextBox.Text = BlurRange.ToString();
                NoiceRange = options.NoiceRange;
                NoiceRangeTextBox.Text = NoiceRange.ToString();
                MorphologicalRange = options.MorphologicalRange;
                MorphologyRangeTextBox.Text = MorphologicalRange.ToString();

                NoiceReductionComboBox.SelectedIndex = NoiceReductionOption = options.NoiceReductionOption;

                MorphologyComboBox.SelectedIndex =  MorphologicalOption = options.MorphologicalOption;
            }
        }

        private void ChooseFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Choose an image";
            openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.tif;...";
            if (openFileDialog.ShowDialog() == true)
            {
                Source = openFileDialog.FileName;
                TransformImageWithMessages(Source);
            }
        }

        private void ReloadFile_Click(object sender, RoutedEventArgs e)
        {
            TransformImageWithMessages(Source);
        }

        private void SaveFile_Click(object sender, RoutedEventArgs e)
        {
            if (FinalBitmap != null)
            {
                Utility.SaveImage(FinalBitmap);
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
                        .Where(file => file.EndsWith(".jpg") ||
                                file.EndsWith(".jpeg") ||
                                file.EndsWith(".png") ||
                                file.EndsWith(".gif") ||
                                file.EndsWith(".tif"))
                        .ToList();

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
            Counter = 0;
            ProgressBar.Maximum = FileNames.Count;

            foreach (string file in FileNames)
            {
                ThreadPool.QueueUserWorkItem(obj => ProcessImage(file)); 
            }
        }

        private void RangeTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var tb = (TextBox)sender;
            int range;

            if (int.TryParse(tb.Text, out range))
            {
                if (range > 0 && range <= 50)
                {
                    if (tb.Name == "BlurRangeTextBox")
                    {
                        BlurRange = range;
                        Filter.generateGaussian(BlurRange);
                    }
                    if (tb.Name == "NoiceRangeTextBox")
                    {
                        NoiceRange = range;
                    }
                    if (tb.Name == "MorphologyRangeTextBox")
                    {
                        MorphologicalRange = range;
                    }
                }
                else
                {
                    MessageBox.Show("Range value must be between 0 and 50", "Invalid range value", MessageBoxButton.OK, MessageBoxImage.Error);
                    tb.Text = "0";
                }
            }
            else
            {
                MessageBox.Show("Range value can be only integer", "Invalid range value", MessageBoxButton.OK, MessageBoxImage.Error);
                tb.Text = "0";
            }
        }

        private void MorphologyComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MorphologicalOption = MorphologyComboBox.SelectedIndex;
        }

        private void GrayScaleComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GrayscaleOption = GrayScaleComboBox.SelectedIndex;
        }

        private void BlurComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BlurOption = BlurComboBox.SelectedIndex;
        }

        private void NoiceReductionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            NoiceReductionOption = NoiceReductionComboBox.SelectedIndex;
        }

    }
}