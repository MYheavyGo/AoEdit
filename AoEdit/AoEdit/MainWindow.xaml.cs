using AoEdit.Utils;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
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
                wav = new WAV(Filename, wavFile.OpenFile(Filename));
                if(wav.Passed == true)
                {
                    if (wavs.Count == 0)
                    {
                        wavs.Add(wav);
                    }

                    wav = verifyWAVS(wav);
                }
                else
                {
                    if (wavs.Count > 0)
                    {
                        wav = wavs.Last();  
                    }
                }
                updateInfos(wav.Header);
            }
        }

        private WAV verifyWAVS(WAV w)
        {
            foreach (WAV item in wavs)
            {
                if(w.Name == item.Name)
                {
                    return item;
                }
            }

            wavs.Add(w);
            return w;
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
            lblSize.Content = Constants.SizeSuffix(header.totalLenght);
            lblAudioFormat.Content = header.pcm;
            lblChannels.Content = header.channels;
            lblBitrate.Content = Constants.SizeSuffix(header.bytesPerSecond) + "/S";
            lblSamplingRate.Content = header.bitsPerSample;
            txtBlockLog.Text = wav.Log;

            DrawSignal(wav.Buffer);
        }

        private void DrawSignal(byte[] buffer)
        {
            canvas.Children.Clear();

            float[] output = buffer.Select(b => (float)b).ToArray();

            var blockWidth = 3;
            var posX = 0;
            var numSubsets = (int)canvas.ActualWidth / blockWidth;
            var subsetLenght = output.Length / numSubsets;

            float[] subsets = new float[numSubsets];

            var s = 0;
            for (int i = 0; i < subsets.Length; i++)
            {
                double sum = 0;
                for(int k = 0; k < subsetLenght; k++)
                {
                    sum += Math.Abs(output[s++]);
                }

                subsets[i] = (float)(sum / subsetLenght);
            }

            float normal = 0;
            foreach(float sample in subsets)
            {
                if (sample > normal)
                {
                    normal = sample;
                }
            }

            normal = 32768.0f / normal;
            for (int i = 0; i < subsets.Length; i++)
            {
                subsets[i] *= normal;
                subsets[i] = (float)((subsets[i] / 32768.0f) * (canvas.ActualHeight / 2));
            }

            for (int i = 0; i < subsets.Length; i++)
            {
                var sample = subsets[i];

                float posY = (float)((canvas.ActualHeight / 2) - sample);
                float negY = (float)((canvas.ActualHeight / 2) + sample);

                posX = i * blockWidth;

                Line l = new Line();
                l.X1 = posX;
                l.X2 = posX;
                l.Y1 = posY;
                l.Y2 = negY;
                l.StrokeThickness = 3;
                l.Stroke = new SolidColorBrush(Colors.DarkOrange);

                canvas.Children.Add(l);
            }
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

            //Remplie les données audio du fichier
            uint time = 1;
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
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (wav != null)
                DrawSignal(wav.Buffer);
        }
    }
}
