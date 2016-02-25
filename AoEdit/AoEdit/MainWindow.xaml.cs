using AoEdit.Audio;
using AoEdit.GUI;
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

    public enum FormWAV
    {
        Null = -1,
        Sin,
        Square,
        Triangle,
        Sawtooth,
        WhiteNoise
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Constantes
        Color renderColor = Colors.DarkOrange;
        Color lineTime = Colors.Red;

        WAVFile wavFile;
        List<WAV> wavs;
        WAV wav;
        string Filename { get; set; }
        Polyline pl;
        Line timeLine;
        int posX;
        int blockWidth;
        RenderWAV render;
        FormWAV form;

        public MainWindow()
        {
            InitializeComponent();

            wavFile = new WAVFile();
            wavs = new List<WAV>();
            pl = new Polyline();
            timeLine = new Line();

            posX = 1;
            blockWidth = 6;
            render = RenderWAV.Bars;
            form = FormWAV.Sin;
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
                if (wav.Passed == true)
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
                progressBarVolume.Value = wav.Player.Volume;

                wav.Player.TimerFile.Tick += TimerFile_Tick;
                wav.Player.Element.MediaEnded += Element_MediaEnded;

                updateInfos(wav.Header);
            }
        }

        //Vérifie si le WAV a déjà été chargé
        private WAV verifyWAVS(WAV w)
        {
            foreach (WAV item in wavs)
            {
                if (w.Name == item.Name)
                {
                    return item;
                }
            }

            wavs.Add(w);
            return w;
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
            lblAudioFormat.Content = "PCM";
            if (header.channels > 1)
                lblChannels.Content = "Stéréo";
            else
                lblChannels.Content = "Mono";
            lblBitrate.Content = Constants.SizeSuffix(header.bytesPerSecond) + "/s";
            lblSamplingRate.Content = header.bitsPerSample;
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
            float height = (float)canvas.ActualHeight;

            for (int i = 1; i <= subsets.Length; i++)
            {
                var sample = subsets[i - 1];
                sample *= height;

                float posY = height / 2 + sample;
                float negY = height / 2 - sample;

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
            var subsetLenght = wav.output.Length / numSubsets;

            float[] subsets = new float[numSubsets];

            //Moyenne les valeurs pour chaque bloc
            var s = 0;
            for (int i = 0; i < subsets.Length; i++)
            {
                double sum = 0;
                for (int k = 0; k < subsetLenght; k++)
                {
                    if (render == RenderWAV.Bars)
                        sum += Math.Abs(wav.output[s++]);
                    else
                        sum += wav.output[s++];
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
            if (form == FormWAV.Null)
                return;

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "WAV File (*.wav)|*.wav";
            if (sfd.ShowDialog() == true)
            {
                wavFile = new WAVFile();
                wavFile.Path = sfd.FileName;
            }
            else
            {
                return;
            }

            // Génère l'en-tête du fichier WAV par une configuration fixe
            wavfile header = CreateHeader(1, 2, 44100, 16);
            header.bytesByCapture = (short)(header.channels * (header.bitsPerSample / 8));
            header.bytesPerSecond = header.frequency * header.bytesByCapture;

            //Remplie les données audio du fichier
            uint time = 1;
            uint numSamples = (uint)(header.frequency * header.channels) * time;
            short[] buffer = new short[numSamples];
            short amplitude = 32760;//short.MaxValue;
            double freq = 440.0f;

            double t = (Math.PI * 2 * freq) / (header.frequency * header.channels);
            int samplesPerWaveLenght = Convert.ToInt32(header.frequency / (freq / header.channels));

            switch (form)
            {
                case FormWAV.Sin:
                    buffer = Sin(numSamples, header.channels, amplitude, t);
                    break;
                case FormWAV.Square:
                    buffer = Square(numSamples, header.channels, amplitude, t);
                    break;
                case FormWAV.Sawtooth:
                    buffer = Sawtooth(numSamples, header.channels, samplesPerWaveLenght, amplitude);
                    break;
                case FormWAV.Triangle:
                    buffer = Triangle(numSamples, header.channels, samplesPerWaveLenght, amplitude);
                    break;
                case FormWAV.WhiteNoise:
                    buffer = WhiteNoise(numSamples, header.channels, amplitude);
                    break;
            }

            header.bytesInData = buffer.Length * (header.bitsPerSample / 8);
            header.totalLenght = 4 + (8 + header.format) + (8 + header.bytesInData);

            wavFile.WriteFile(buffer, header);

            MessageBox.Show("Réussite de l'écriture");
        }

        private short[] Sin(uint numSamples, int channels, int amplitude, double t)
        {
            short[] tmp = new short[numSamples];
            short tempSample = 0;

            for (int i = 0; i < numSamples; i += channels)
            {
                tempSample = Convert.ToInt16(amplitude * Math.Sin(t * i));
                for (int channel = 0; channel < channels; channel++)
                {
                    tmp[i + channel] = tempSample;
                }
            }
            return tmp;
        }

        private short[] Square(uint numSamples, short channels, int amplitude, double t)
        {
            short[] tmp = new short[numSamples];
            for (uint i = 0; i < numSamples - 1; i++)
            {
                for (int channel = 0; channel < channels; channel++)
                {
                    tmp[i] = Convert.ToInt16(amplitude * Math.Sign(Math.Sin(t * i)));
                }
            }
            return tmp;
        }

        private short[] Sawtooth(uint numSamples, short channels, int samplesPerWaveLenght, int amplitude)
        {
            short[] tmp = new short[numSamples];
            short ampStep = Convert.ToInt16((amplitude * 2) / samplesPerWaveLenght);
            short tempSample;
            int totalSamplesWritten = 0;

            while (totalSamplesWritten < numSamples)
            {
                tempSample = (short)-amplitude;

                for (uint i = 0; i < samplesPerWaveLenght && totalSamplesWritten < numSamples; i++)
                {
                    tempSample += ampStep;
                    for (int channel = 0; channel < channels; channel++)
                    {
                        tmp[totalSamplesWritten] = tempSample;

                        totalSamplesWritten++;
                    }
                }
            }

            return tmp;
        }

        // A terminer 
        private short[] Triangle(uint numSamples, short channels, int samplesPerWaveLenght, short amplitude)
        {
            short[] tmp = new short[numSamples];
            short ampStep = Convert.ToInt16((amplitude * 2) / samplesPerWaveLenght);
            short tempSample = (short)-amplitude;

            for (uint i = 0; i < numSamples - 1; i++)
            {
                if (Math.Abs(tempSample) > amplitude)
                {
                    ampStep = (short)-ampStep;
                }

                if (i % 200 == 0)
                    Console.WriteLine("");

                if (tempSample + ampStep > -amplitude && tempSample + ampStep < amplitude)
                    tempSample += ampStep;
                else
                    tempSample = (short)(tempSample > 0 ? (amplitude + 1) : (-amplitude - 1));

                for (int channel = 0; channel < channels; channel++)
                {
                    tmp[i + channel] = tempSample;
                }
            }

            return tmp;
        }

        private short[] WhiteNoise(uint numSamples, short channels, int amplitude)
        {
            short[] tmp = new short[numSamples];
            Random rnd = new Random();
            for (uint i = 0; i < numSamples - 1; i++)
            {
                for (int channel = 0; channel < channels; channel++)
                {
                    tmp[i + channel] = Convert.ToInt16(rnd.Next(-amplitude, amplitude));
                }
            }
            return tmp;
        }

        private wavfile CreateHeader(short formatAudio, short channels, int frequency, short bitspersample)
        {
            wavfile tmp = new wavfile();
            tmp.id = new char[] { 'R', 'I', 'F', 'F' };
            tmp.wavefmt = new char[] { 'W', 'A', 'V', 'E', 'f', 'm', 't', ' ' };
            tmp.data = new char[] { 'd', 'a', 't', 'a' };
            tmp.format = 16;
            tmp.pcm = formatAudio;
            tmp.channels = channels;
            tmp.frequency = frequency;
            tmp.bitsPerSample = bitspersample;
            return tmp;
        }

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
                if (container == null || container.Tag == null)
                {
                    continue;
                }
                if (container.Tag.Equals(currentItem.Tag))
                {
                    items.Add(container);
                }
            }

            return items;
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //Redessine le Signal
            if (wav != null)
                DrawSignal();
        }

        private void GenerateSpectrogram_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn.Tag.Equals("1"))
            {
                new Spectrogram(wav.output);
            }
            else if (btn.Tag.Equals("2"))
            {
                /*if (wavs.Count == 0)
                {
                    MessageBox.Show("Pas de fichier chargés");
                    return;
                }*/
                //new WindowsRender(ne(800, 600, new GraphicsMode(32, 24, 0, 8)));
            }
        }

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
                                case -1:
                                    render = RenderWAV.Null;
                                    break;
                            }
                        }
                    }
                }
            }

            if (wav != null)
                DrawSignal();
        }

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
                            switch (i)
                            {
                                case 0:
                                    form = FormWAV.Sin;
                                    break;
                                case 1:
                                    form = FormWAV.Square;
                                    break;
                                case 2:
                                    form = FormWAV.Triangle;
                                    break;
                                case 3:
                                    form = FormWAV.Sawtooth;
                                    break;
                                case 4:
                                    form = FormWAV.WhiteNoise;
                                    break;
                                case -1:
                                    form = FormWAV.Null;
                                    break;
                            }
                        }
                    }
                }
            }
        }

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
            var timePlay = wav.Player.Element.Position.Seconds * 1000 + wav.Player.Element.Position.Milliseconds;
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
