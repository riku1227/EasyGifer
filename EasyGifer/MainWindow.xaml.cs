using System;
using System.Diagnostics;
using System.Windows;
using System.IO;
using Microsoft.Win32;
using System.Windows.Input;
using System.Windows.Controls;

namespace EasyGifer
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowViewModel viewModel = new MainWindowViewModel();
        private const String FFMPEG_PATH= @"lib\ffmpeg.exe";
        private const String GIFSICLE_PAHT = @"lib\gifsicle.exe";
        private String optimizeOutPaht = "";
        private Boolean nowProcessing = false;
        public MainWindow()
        {
            InitializeComponent();
            RegistryKey rkey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");
            if(rkey != null)
            {
                var useLightTheme = (int)rkey.GetValue("AppsUseLightTheme");
                if(useLightTheme == 0)
                {
                    var themeManager = new ThemeManager(this);
                    themeManager.SetUpTheme();
                }
            }
            this.DataContext = viewModel;
        }

        private void InputFileButton_Click(object sender, RoutedEventArgs e)
        {
            if(!nowProcessing)
            {
                var openFileDialog = new OpenFileDialog();
                var result = openFileDialog.ShowDialog();
                if (result == true)
                {
                    viewModel.InpuPath = openFileDialog.FileName;
                    notifyInputPathChanged();
                }
            }
        }

        private void OutputFileButton_Click(object sender, RoutedEventArgs e)
        {
            if(!nowProcessing)
            {
                var openFileDialog = new SaveFileDialog();
                openFileDialog.Filter = "gif|*.gif";
                openFileDialog.AddExtension = true;
                var result = openFileDialog.ShowDialog();
                if (result == true)
                {
                    viewModel.OutputPath = openFileDialog.FileName;
                    notifyOutputPathChanged();
                }
            }
        }

        private void notifyInputPathChanged()
        {
            InputPathTextBox.Text = viewModel.InpuPath;

            OutputPathTextBox.Text = viewModel.InpuPath.Replace(Path.GetExtension(viewModel.InpuPath), ".gif");
        }

        private void notifyOutputPathChanged()
        {
            OutputPathTextBox.Text = viewModel.OutputPath;
        }

        private void GenerateGifButton_Click(object sender, RoutedEventArgs e)
        {
            if(nowProcessing)
            {
                return;
            }
            if(viewModel.InpuPath == "")
            {
                return;
            }
            if(!File.Exists(viewModel.InpuPath))
            {
                return;
            }
            nowProcessing = true;
            optimizeOutPaht = viewModel.OutputPath;
            if(OptimizeGif.IsChecked == true)
            {
                viewModel.OutputPath = viewModel.OutputPath.Replace(".gif", "_temp.gif");
            }
            if(!File.Exists(AppDomain.CurrentDomain.BaseDirectory + FFMPEG_PATH))
            {
                nowProcessing = false;
                return;
            }
            ProcessStartInfo pInfo = new ProcessStartInfo();
            pInfo.FileName = AppDomain.CurrentDomain.BaseDirectory + FFMPEG_PATH;
            String argments = "";
            if(GifQualityLow.IsChecked == true)
            {
                argments = CreateLowArgments();
            } else if (GifQualityMedium.IsChecked == true)
            {
                argments = CreateMediumArgments();
            } else if (GifQualityHigh.IsChecked == true)
            {
                argments = CreateHighArgments();
            } else if (GifQualityGod.IsChecked == true)
            {
                argments = CreateGodArgments();
            }
            pInfo.Arguments = argments;

            var process = Process.Start(pInfo);
            process.EnableRaisingEvents = true;
            process.Exited += new EventHandler(onFFMPEGExit);
        }

        private void onFFMPEGExit(object sender, EventArgs e)
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                if (OptimizeGif.IsChecked == true)
                {
                    if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + GIFSICLE_PAHT))
                    {
                        return;
                    }
                    ProcessStartInfo optimizePInfo = new ProcessStartInfo();
                    optimizePInfo.FileName = AppDomain.CurrentDomain.BaseDirectory + GIFSICLE_PAHT;
                    optimizePInfo.Arguments = "-O3 --lossy=80 -o \"" + optimizeOutPaht + "\" " + "\"" + viewModel.OutputPath + "\"";
                    var optimizeProcess = Process.Start(optimizePInfo);
                    optimizeProcess.EnableRaisingEvents = true;
                    optimizeProcess.Exited += new EventHandler(onGifsicleExit);
                } 
                else
                {
                    nowProcessing = false;
                }
            }));
        }

        private void onGifsicleExit(object sender, EventArgs args)
        {
            File.Delete(viewModel.OutputPath);
            viewModel.OutputPath = optimizeOutPaht;
            nowProcessing = false;
        }

        private String CreateLowArgments()
        {
            return "-y" +
                " -i " + "\"" + viewModel.InpuPath + "\"" +
                " -r " + viewModel.GifFps +
                " -vf scale=" + viewModel.GifWidth + ":-1" +
                " \"" + viewModel.OutputPath + "\"";
        }

        private String CreateMediumArgments()
        {
            return "-y" +
                " -i " + "\"" + viewModel.InpuPath + "\"" +
                " -filter_complex \"[0:v] fps = " + viewModel.GifFps + ",scale = " + viewModel.GifWidth + ":-1,split[a][b];[a] palettegen[p];[b] [p] paletteuse=dither=none\"" +
                " \"" + viewModel.OutputPath + "\"";
        }

        private String CreateHighArgments()
        {
            return "-y" +
                " -i " + "\"" + viewModel.InpuPath + "\"" +
                " -filter_complex \"[0:v] fps = " + viewModel.GifFps + ",scale = " + viewModel.GifWidth + ":-1,split[a][b];[a] palettegen[p];[b] [p] paletteuse=dither=floyd_steinberg\"" +
                " \"" + viewModel.OutputPath + "\"";
        }

        private String CreateGodArgments()
        {
            return "-y" +
                " -i " + "\"" + viewModel.InpuPath + "\"" +
                " -filter_complex \"[0:v] fps = " + viewModel.GifFps + ",scale = " + viewModel.GifWidth + ":-1,split[a][b];[a] palettegen=stats_mode=single [p];[b] [p] paletteuse=new=1\"" +
                " \"" + viewModel.OutputPath + "\"";
        }

        private void InputPathTextBox_PreviewDragOver(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Copy;
            e.Handled = e.Data.GetDataPresent(DataFormats.FileDrop);
        }

        private void InputPathTextBox_Drop(object sender, DragEventArgs e)
        {
            InputPathTextBox.Text = String.Empty;
            string[] files = (string[]) e.Data.GetData(DataFormats.FileDrop);

            if(files != null)
            {
                InputPathTextBox.Text = files[0];

                OutputPathTextBox.Text = files[0].Replace(Path.GetExtension(files[0]), ".gif");
            }
        }
    }
}
//clr-namespace:EasyGifer