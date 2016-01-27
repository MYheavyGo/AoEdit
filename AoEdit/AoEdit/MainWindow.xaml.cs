using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace AoEdit
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        WAVFile wavFile;
        List<WAV> wavs;
        WAV wav;
        string Filename { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            wavFile = new WAVFile();
            wavs = new List<WAV>();
        }

        private void Open_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void Open_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.DefaultExt = ".wav";
            dlg.Filter = "WAV Files (*.wav)|*.wav";

            bool? result = dlg.ShowDialog();

            if (result == true)
            {
                Filename = dlg.FileName;
                txtBlockLog.Text = "Fichier " + Filename + " selectionné";
                wavs.Add(new WAV(wavFile.OpenFile(Filename)));
                wav = wavs.Last();
                updateInfos(wav.Header);
            }
        }

        private void ResetContent()
        {
            lblFormat.Content = "";
            lblSize.Content = "";
            lblAudioFormat.Content = "";
            lblChannels.Content = "";
            lblBitrate.Content = "";
            lblSamplingRate.Content = "";
        }

        private void updateInfos(wavfile header)
        {
            if (!wav.Passed)
            {
                MessageBox.Show(wav.Log + ".\nVeuillez sélectionné un autre fichier.", "Format du fichier", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            lblFileName.Content = Filename.Substring(Filename.LastIndexOf('\\') + 1);
            lblFormat.Content = new string(header.wavefmt, 0, 4);
            lblSize.Content = header.totalLenght;
            lblAudioFormat.Content = header.pcm;
            lblChannels.Content = header.channels;
            lblBitrate.Content = header.bytesPerSecond;
            lblSamplingRate.Content = header.bitsPerSample;
            txtBlockLog.Text = wav.Log;

            DrawSignal(wav.Buffer);
        }

        private void DrawSignal(byte[] buffer)
        {
            /*var frames = samples / 2;
            var max = 0.0f;
            var min = 0.0f;
            var batch = (int)Math.Max(40, samples / 4000);
            var mid = Signal.ActualHeight / 2;
            var yScale = 100;
            var xPos = 0;

            var bytes = wav.Header.bitsPerSample + 7 >> 3;

            var frameLenght = wav.Header.bytesByCapture;
            var bufferLenght = wav.Header.channels * bytes * 1024;

            var samples = new float[frameLenght];
            var buf = new byte[bufferLenght];

            for (int i = 0; i < batch; i++)
            {
                for(int n = batch * (i + 1); n > 0; n--)
                {
                    max = Math.Max(max, Math.Abs(buffer[n]));
                    if (wav.Header.channels > 1)
                    {
                        min = Math.Min(min, Math.Abs(buffer[n]));
                    }
                }

                Line line = new Line();
                line.X1 = xPos;
                line.X2 = xPos;
                line.Y1 = mid + (max * yScale);
                line.Y2 = mid - (max * yScale);
                line.StrokeThickness = 1;
                line.Stroke = new SolidColorBrush(Colors.IndianRed);
                Signal.Children.Add(line);
                max = 0;
                xPos++;
            }

            Signal.Width = xPos;*/ 
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "WAV File (*.wav)|*.wav";
            if (sfd.ShowDialog() == true)
            {
                wavFile = new WAVFile();
                wavFile.Path = sfd.FileName;
            } else
            {
                return;
            }

            // Génère l'en-tête du fichier WAV par une configuration fixe
            wavfile header = new wavfile();
            header.id = new char[] { 'R', 'I', 'F', 'F' };
            header.wavefmt = new char[] { 'W', 'A', 'V', 'E', 'f', 'm', 't', ' '};
            header.data = new char[] { 'd', 'a', 't', 'a'};
            header.format = 16;
            header.pcm = 1;
            header.channels = 2;
            header.frequency = 44100;
            header.bitsPerSample = 16;
            header.bytesByCapture = (short) (header.channels * (header.bitsPerSample / 8));
            header.bytesPerSecond = header.frequency * header.bytesByCapture;

            var startTime = DateTime.Now;
            var endTime = 0;

            //Remplie les données audio du fichier
            uint time = 20;
            uint numSamples = (uint) (header.frequency * header.channels) * time;
            short[] buffer = new short[numSamples];
            int amplitude = 32760;
            double freq = 17000.0f;

            double t = (Math.PI * 2 * freq) / (header.frequency * header.channels);
            Random rnd = new Random();

            for (uint i = 0; i < numSamples - 1; i++)
            {
                for (int channel = 0; channel < header.channels; channel++)
                {
                    //buffer[i + channel] = Convert.ToInt16(amplitude * Math.Sin(t * i));
                    //buffer[i + channel] = Convert.ToInt16(amplitude * Math.Sign(Math.Sin(t * i)));
                    buffer[i + channel] = Convert.ToInt16(rnd.Next(-amplitude, amplitude));
                }
            }
            header.bytesInData = buffer.Length * (header.bitsPerSample / 8);
            header.totalLenght = 4 + (8 + header.format) + (8 + header.bytesInData);

            wavFile.WriteFile(buffer, header);

            endTime = DateTime.Now - startTime;
            Console.WriteLine(endTime);
        }
    }
}
