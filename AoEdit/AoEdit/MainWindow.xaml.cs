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
                updateInfos(wav);
            }
        }

        private void updateInfos(WAV wav)
        {
            lblFormat.Content = wav.Header.Format;
            lblSize.Content = wav.Header.ChunkSize + "";
            lblAudioFormat.Content = wav.Fmt.getNameAudioFormat();
            lblChannels.Content = wav.Fmt.NumChannels;
            lblBitrate.Content = wav.Fmt.ByteRate;
            lblSamplingRate.Content = wav.Fmt.SampleRate;
            txtBlockLog.Text = wav.Log;
        }
    }
}
