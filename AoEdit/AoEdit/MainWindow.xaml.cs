using Microsoft.Win32;
using System;
using System.Collections.Generic;
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
        WAV wav = new WAV();
        string Filename { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            //wav.Buffer = wav.File.OpenFile("./Ressources/WAV/good_bad_ugly.wav");
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            //txtBoxLog.Text = wav.Log;
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
                txtBoxLog.Text = "Fichier " + Filename + " selectionné";
                wav.Buffer = wav.File.OpenFile(Filename);
            }
        }
    }
}
