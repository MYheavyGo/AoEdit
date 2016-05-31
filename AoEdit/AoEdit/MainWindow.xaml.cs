using AoEdit.Audio;
using AoEdit.Utils;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace AoEdit
{
    public enum RenderWAV
    {
        Null = -1,
        Line,
        Fill,
        Largers,
        Bars
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Constantes
        Color renderColor = Colors.DarkOrange;
        Color lineTime = Colors.Red;

        List<WAV> wavs;
        WAV wav;
        string Filename { get; set; }
        Polyline pl;
        Line timeLine;
        int posX;
        int blockWidth;
        RenderWAV render;
        int formWAV = 0;

        public MainWindow()
        {
            InitializeComponent();

            wavs = new List<WAV>();
            pl = new Polyline();
            timeLine = new Line();

            posX = 1;
            blockWidth = 4;
            render = RenderWAV.Bars;
        }

        private void Open_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void Open_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            //Ouvrir un fichier WAV
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.DefaultExt = ".wav";
            dlg.Filter = "WAV Files (*.wav)|*.wav";

            bool? result = dlg.ShowDialog();

            if (result == true)
            {
                Filename = dlg.FileName;
                wav = WAVFile.OpenFile(Filename);

                //Système qui vérifie si mon fichier est déjà chargé dans le programme
                if (wav.Passed == true)
                {
                    if (wavs.Count == 0)
                    {
                        wavs.Add(wav);
                    }

                    verifyWAVS(ref wav);
                }
                else
                {
                    if (wavs.Count > 0)
                    {
                        wav = wavs.Last();
                    }
                }

                //Ajoute les events au player Audio
                progressBarVolume.Value = wav.Player.Volume;
                wav.Player.TimerFile.Tick += TimerFile_Tick;
                wav.Player.Element.MediaEnded += Element_MediaEnded;

                //Mets à jour les labels
                updateInfos(ref wav.Header);

                //Log pour l'ouverture correcte du fichier
                txtBlockLog.Text = "Fichier " + Filename + " selectionné et chargé";
            }
        }

        //Vérifie si le WAV a déjà été chargé
        private void verifyWAVS(ref WAV w)
        {
            foreach (WAV item in wavs)
            {
                if (w.Name == item.Name)
                {
                    w = item;
                }
            }

            wavs.Add(w);
        }

        //Reset le contenu des labels
        private void ResetContent()
        {
            lblFormat.Content = "";
            lblSize.Content = "";
            lblAudioFormat.Content = "";
            lblChannels.Content = "";
            lblBitrate.Content = "";
            lblSamplingRate.Content = "";
        }

        //Mets à jour les infos du WAV
        private void updateInfos(ref DataHeader header)
        {
            if (!wav.Passed)
            {
                MessageBox.Show(wav.Log + ".\nVeuillez sélectionné un autre fichier.", "Format du fichier", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            lblFileName.Content = Filename.Substring(Filename.LastIndexOf('\\') + 1);
            lblFormat.Content = new string(header.MediaTypedID, 0, 4);
            lblSize.Content = Constants.SizeSuffix(header.FileLenght);
            lblAudioFormat.Content = "PCM";
            if (header.Channels > 1)
                lblChannels.Content = "Stéréo";
            else
                lblChannels.Content = "Mono";
            lblBitrate.Content = Constants.SizeSuffix(header.AverageBytesPerSec) + "/s";
            lblSamplingRate.Content = header.BitsPerSample;
            txtBlockLog.Text = wav.Log;

            DrawSignal();
        }

        //Vide les lines et la polyline
        private void ResetSignal()
        {
            canvas.Children.Clear();
            //pl.Points.Clear();
        }

        //Dessine en forme de Box
        private void DrawBox(float[] subsets)
        {
            double height = canvas.ActualHeight;

            for (int i = 1; i <= subsets.Length; i++)
            {
                double sample = subsets[i - 1];
                sample *= height;

                double posY = (height / 2) + sample / 1.3;
                double negY = (height / 2) - sample / 1.3;

                posX = i * blockWidth;
                //pl.Points.Add(p);

                Line l = new Line();
                l.X1 = posX;
                l.X2 = posX;
                l.Y1 = posY;
                l.Y2 = negY;
                l.StrokeThickness = 1.5;
                l.Stroke = new SolidColorBrush(renderColor);

                //Ajoute les lignes dans le canvas
                canvas.Children.Add(l);
            }
        }

        //Dessine une ligne
        private void DrawStraightLine(float[] subsets)
        {
            List<Point> pointsTop = new List<Point>();
            float height = (float)canvas.ActualHeight;

            for (int i = 1; i <= subsets.Length; i++)
            {
                var sample = subsets[i - 1];
                sample *= height;

                float posY = height - (height / 2 - sample);

                posX = i * blockWidth;

                pointsTop.Add(new Point(posX, posY));
            }

            if (pointsTop.Count < 1)
                return;

            Point firstPoint = pointsTop[0];
            foreach (Point p in pointsTop)
            {
                if (p == firstPoint)
                    continue;

                Line l = new Line();
                l.X1 = firstPoint.X;
                l.X2 = p.X;
                l.Y1 = firstPoint.Y;
                l.Y2 = p.Y;
                l.Stroke = new SolidColorBrush(renderColor);
                canvas.Children.Add(l);

                firstPoint = p;
            }
        }

        //Prépare les données et dessine dans le format voulu le WAV
        private void DrawSignal()
        {  
            ResetSignal();

            //Largeur du bloc
            blockWidth = 6;
            //Position de départ
            posX = 1;
            //Nombre max de bloc possible à dessiner sur le canvas
            var numSubsets = (int)canvas.ActualWidth / blockWidth;
            //La longueur de données à prendre pour chaque bloc
            var subsetLenght = wav.Samples.Length / numSubsets;

            float[] subsets = new float[numSubsets];

            //Moyenne les valeurs pour chaque bloc
            var s = 0;
            for (int i = 0; i < subsets.Length; i++)
            {
                double sum = 0;
                for (int k = 0; k < subsetLenght; k++)
                {
                    if (render == RenderWAV.Bars)
                        sum += Math.Abs(wav.Samples[s++]);
                    else
                        sum += wav.Samples[s++];
                }

                subsets[i] = (float)(sum / subsetLenght);
            }

            float maxValue = short.MaxValue;
            for (int i = 0; i < subsets.Length; i++)
            {
                subsets[i] = subsets[i] / maxValue;
            }

            switch (render)
            {
                case RenderWAV.Bars:
                    DrawBox(subsets);
                    break;
                case RenderWAV.Fill:
                    break;
                case RenderWAV.Largers:
                    break;
                case RenderWAV.Line:
                    DrawStraightLine(subsets);
                    break;
            }
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "WAV File (*.wav)|*.wav";
            if (sfd.ShowDialog() == false)
            {
                return;
            }

            //Infos utiles
            uint time = 1;  //Par défaut

            // Génère l'en-tête du fichier WAV par une configuration fixe
            WAVCreator wavCreate = new WAVCreator(1, 2, 44100, 16, time, formWAV, sfd.FileName);

            MessageBox.Show("Réussite de l'écriture");
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //Redessine le Signal
            if (wav != null)
                DrawSignal();
        }

        private void GenerateSpectrogram_Click(object sender, RoutedEventArgs e)
        {
            if (wavs.Count == 0)
            {
                MessageBox.Show("Pas de fichier chargés");
                return;
            }

            Button btn = sender as Button;
            if (btn.Tag.Equals("1"))
            {
                new Spectrogram(wav.Samples);
            }
            else if (btn.Tag.Equals("2"))
            {
                //new WindowsRender(ne(800, 600, new GraphicsMode(32, 24, 0, 8)));
            }
        }

        //Récupère tous les items au même niveau
        private List<MenuItem> GetGroupItems(MenuItem currentItem)
        {
            var parentItem = currentItem.Parent as MenuItem;
            if (!(parentItem.Items.Count > 0))
            {
                return null;
            }

            List<MenuItem> items = new List<MenuItem>();
            foreach (var item in parentItem.Items)
            {
                MenuItem container = item as MenuItem;
                if (container != null)
                {
                    items.Add(container);
                }
            }

            return items;
        }

        //Choisir un rendu pour le canvas
        private void Render_Click(object sender, RoutedEventArgs e)
        {
            var currentItem = e.OriginalSource as MenuItem;

            if (currentItem.IsCheckable && currentItem.Tag != null)
            {
                var items = GetGroupItems(currentItem);

                if (items != null)
                {
                    for (int i = 0; i < items.Count; i++)
                    {
                        if (items[i] != currentItem)
                        {
                            items[i].IsChecked = false;
                        }
                        else
                        {
                            switch (i)
                            {
                                case 0:
                                    render = RenderWAV.Bars;
                                    break;
                                case 1:
                                    render = RenderWAV.Fill;
                                    break;
                                case 2:
                                    render = RenderWAV.Largers;
                                    break;
                                case 3:
                                    render = RenderWAV.Line;
                                    break;
                            }

                            if (!currentItem.IsChecked)
                                currentItem.IsChecked = true;
                        }
                    }
                }
            }

            if (wav != null)
                DrawSignal();
        }

        //Choisir une forme pour le WAV
        private void Form_Click(object sender, RoutedEventArgs e)
        {
            var currentItem = e.OriginalSource as MenuItem;

            if (currentItem.IsCheckable && currentItem.Tag != null)
            {
                var items = GetGroupItems(currentItem);

                if (items != null)
                {
                    for (int i = 0; i < items.Count; i++)
                    {
                        if (items[i] != currentItem)
                        {
                            items[i].IsChecked = false;
                        }
                        else
                        {
                            if (currentItem.IsChecked)
                                formWAV = Convert.ToInt32(currentItem.Tag);
                            else
                            {
                                currentItem.IsChecked = true;
                                formWAV = Convert.ToInt32(currentItem.Tag);
                            }
                        }
                    }
                }
            }
        }

        //Fenêtre A propos
        private void About_Click(object sender, RoutedEventArgs e)
        {

        }

        //Event for the Player (Play the WAV file selected)
        private void Play_Click(object sender, RoutedEventArgs e)
        {
            if (wav != null)
            {
                if (wav.Player.Element.NaturalDuration.HasTimeSpan && wav.Player.Element.NaturalDuration.TimeSpan.Seconds == wav.Player.Element.Position.Seconds)
                {
                    wav.Player.Element.Stop();
                }
                wav.Player.Element.Play();
                wav.Player.TimerFile.Start();
            }
        }

        private void Pause_Click(object sender, RoutedEventArgs e)
        {
            if (wav != null)
            {
                wav.Player.Element.Pause();
            }
        }

        private void Window_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (wav != null)
            {
                wav.Player.Volume += (e.Delta > 0) ? 0.1 : -0.1;
                progressBarVolume.Value = wav.Player.Volume;
            }
        }

        private void TimerFile_Tick(object sender, EventArgs e)
        {
            var timePlay = wav.Player.Element.Position.Minutes * 60000 + wav.Player.Element.Position.Seconds * 1000 + wav.Player.Element.Position.Milliseconds;
            double pixelToMove = 0;

            if (timePlay == 0)
            {
                return;
            }

            if (wav.Player.Element.NaturalDuration.HasTimeSpan)
            {
                pixelToMove = canvas.ActualWidth / wav.Player.Element.NaturalDuration.TimeSpan.TotalMilliseconds;
            }

            canvas.Children.Remove(timeLine);

            timeLine = new Line();
            timeLine.X1 = pixelToMove * timePlay;
            timeLine.X2 = pixelToMove * timePlay;
            timeLine.Y1 = 0;
            timeLine.Y2 = canvas.ActualHeight;
            timeLine.Stroke = new SolidColorBrush(lineTime);
            timeLine.StrokeThickness = 1.2f;

            canvas.Children.Add(timeLine);
        }

        private void Element_MediaEnded(object sender, RoutedEventArgs e)
        {
            canvas.Children.Remove(timeLine);
            wav.Player.TimerFile.Stop();
        }
    }
}
