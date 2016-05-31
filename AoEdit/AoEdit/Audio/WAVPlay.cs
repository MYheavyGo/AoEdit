using System;
using System.IO;
using System.Media;
using System.Windows.Controls;
using System.Windows.Threading;

namespace AoEdit.Audio
{
    class WAVPlay
    {
        public DispatcherTimer TimerFile { get; set; }
        public MediaElement Element { get; set; }
        public double Volume
        {
            get { return volume; }
            set
            {
                volume = value;
                Element.Volume = value;
            }
        }

        private double volume;
        //private bool isDraggingSlider = false;

        public WAVPlay(string path)
        {
            Element = new MediaElement();
            Element.LoadedBehavior = MediaState.Manual;
            Element.UnloadedBehavior = MediaState.Manual;
            Volume = 1;

            TimerFile = new DispatcherTimer();
            TimerFile.Interval = TimeSpan.FromMilliseconds(17);

            Element.Source = new Uri(path, UriKind.Absolute);
        }
    }
}
